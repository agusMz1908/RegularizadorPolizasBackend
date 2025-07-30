using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace RegularizadorPolizas.Application.External.Velneo
{
    public class VelneoHttpService : IVelneoHttpService
    {
        private readonly ITenantService _tenantService;
        private readonly ILogger<VelneoHttpService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public VelneoHttpService(
            ITenantService tenantService,
            ILogger<VelneoHttpService> logger)
        {
            _tenantService = tenantService;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<HttpClient> GetConfiguredHttpClientAsync()
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            if (tenantConfig == null)
            {
                throw new InvalidOperationException("Tenant configuration not found");
            }

            var httpClient = new HttpClient();
            if (tenantConfig.TimeoutSeconds > 0)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(tenantConfig.TimeoutSeconds);
            }
            else
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5); 
            }

            httpClient.DefaultRequestHeaders.Add("User-Agent", "RegularizadorPolizas/1.0");

            _logger.LogDebug("✅ HttpClient configured for tenant {TenantId} with timeout {Timeout}ms",
                _tenantService.GetCurrentTenantId(), httpClient.Timeout.TotalMilliseconds);

            return httpClient;
        }

        public async Task<string> BuildVelneoUrlAsync(string endpoint)
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            if (tenantConfig == null)
            {
                throw new InvalidOperationException("Tenant configuration not found");
            }

            var baseUrl = tenantConfig.BaseUrl?.TrimEnd('/');
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Velneo BaseUrl not configured for tenant");
            }

            var separator = endpoint.Contains("?") ? "&" : "?";
            var fullUrl = $"{baseUrl}/{endpoint}{separator}api_key={tenantConfig.Key}";

            _logger.LogDebug("🔗 Built Velneo URL for endpoint: {Endpoint}", endpoint);
            return fullUrl;
        }

        public async Task<IEnumerable<TEntity>> DeserializeWithFallbackAsync<TResponse, TEntity>(
            HttpResponseMessage response,
            Func<TResponse, IEnumerable<TEntity>?> extractFromWrapper,
            string entityName)
            where TResponse : class
            where TEntity : class
        {
            _logger.LogDebug("🔄 Attempting deserialization with fallback for {EntityName}", entityName);

            try
            {
                var wrapperResponse = await DeserializeResponseAsync<TResponse>(response);
                if (wrapperResponse != null)
                {
                    var entitiesFromWrapper = extractFromWrapper(wrapperResponse);
                    if (entitiesFromWrapper != null && entitiesFromWrapper.Any())
                    {
                        _logger.LogInformation("✅ Successfully deserialized {EntityName} from wrapper format ({Count} items)",
                            entityName, entitiesFromWrapper.Count());
                        return entitiesFromWrapper;
                    }
                }

                _logger.LogWarning("⚠️ Wrapper format failed for {EntityName}, trying direct array format", entityName);

                using var newHttpClient = await GetConfiguredHttpClientAsync();
                var newResponse = await newHttpClient.GetAsync(response.RequestMessage?.RequestUri);
                newResponse.EnsureSuccessStatusCode();

                var directEntities = await DeserializeResponseAsync<List<TEntity>>(newResponse);
                if (directEntities != null && directEntities.Any())
                {
                    _logger.LogInformation("✅ Successfully deserialized {EntityName} from direct array format ({Count} items)",
                        entityName, directEntities.Count);
                    return directEntities;
                }

                _logger.LogWarning("⚠️ No {EntityName} found in either format", entityName);
                return new List<TEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in fallback deserialization for {EntityName}", entityName);
                throw new ApplicationException($"Error deserializing {entityName} with fallback: {ex.Message}", ex);
            }
        }

        public async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            var jsonContent = string.Empty;
            try
            {
                jsonContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("⚠️ Empty JSON response from Velneo API");
                    return null;
                }

                _logger.LogDebug("📄 JSON Response length: {Length} chars", jsonContent.Length);
                return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "❌ JSON parsing error. Content preview: {Content}",
                    jsonContent?.Substring(0, Math.Min(500, jsonContent.Length)));
                throw new ApplicationException($"Error parsing Velneo API response: {ex.Message}", ex);
            }
        }
    }
}