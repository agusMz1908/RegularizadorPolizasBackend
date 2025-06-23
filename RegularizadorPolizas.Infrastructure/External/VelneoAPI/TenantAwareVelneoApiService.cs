using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI
{
    public class TenantAwareVelneoApiService : IVelneoApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantAwareVelneoApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public TenantAwareVelneoApiService(
            IHttpClientFactory httpClientFactory,
            ITenantService tenantService,
            ILogger<TenantAwareVelneoApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tenantService = tenantService;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        private async Task<HttpClient> GetConfiguredHttpClientAsync()
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            _logger.LogDebug("🔧 Creating HttpClient for tenant {TenantId} with BaseUrl: {BaseUrl}",
                tenantConfig.TenantId, tenantConfig.BaseUrl);

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri(tenantConfig.BaseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(tenantConfig.TimeoutSeconds);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RegularizadorPolizas-API/1.0");

            if (!string.IsNullOrEmpty(tenantConfig.Key))
            {
                httpClient.DefaultRequestHeaders.Add("ApiKey", tenantConfig.Key);
            }

            if (!string.IsNullOrEmpty(tenantConfig.ApiVersion))
            {
                httpClient.DefaultRequestHeaders.Add("Api-Version", tenantConfig.ApiVersion);
            }

            _logger.LogInformation("🌐 HttpClient configured for tenant {TenantId}: {BaseUrl} (Timeout: {Timeout}s)",
                tenantConfig.TenantId, tenantConfig.BaseUrl, tenantConfig.TimeoutSeconds);

            return httpClient;
        }

        private async Task<string> BuildUrlWithApiKeyAsync(string endpoint)
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
            return $"{endpoint}?api_key={tenantConfig.Key}";
        }

        #region Client Operations

        public async Task<ClientDto?> GetClientAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ClientDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId} from Velneo API", id);
                throw;
            }
        }

        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating client in Velneo API: {ClientName} for tenant {TenantId}", clientDto.Clinom, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var json = JsonSerializer.Serialize(clientDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/clientes");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ClientDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize created client from Velneo API");
                }

                _logger.LogInformation("Successfully created client {ClientId} in Velneo API for tenant {TenantId}", result.Id, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client in Velneo API");
                throw;
            }
        }

        public async Task UpdateClientAsync(ClientDto clientDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(clientDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/{clientDto.Id}");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated client {ClientId} in Velneo API for tenant {TenantId}",
                    clientDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {ClientId} in Velneo API", clientDto.Id);
                throw;
            }
        }

        public async Task DeleteClientAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Deleting client {ClientId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted client {ClientId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {ClientId} in Velneo API", id);
                throw;
            }
        }

        public async Task<ClientDto?> GetClientByEmailAsync(string email)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/email/{Uri.EscapeDataString(email)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ClientDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client by email from Velneo API");
                throw;
            }
        }

        public async Task<ClientDto?> GetClientByDocumentAsync(string document)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/document/{Uri.EscapeDataString(document)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ClientDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client by document from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<ClientDto>>(_jsonOptions);
                return result ?? new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients in Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/clientes");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<ClientDto>>(_jsonOptions);
                return result ?? new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all clients from Velneo API");
                throw;
            }
        }

        #endregion

        #region Broker Operations

        public async Task<BrokerDto?> GetBrokerAsync(int id)
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/corredores/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<BrokerDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting broker {BrokerId} from Velneo API", id);
                throw;
            }
        }

        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(brokerDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/corredores");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<BrokerDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize created broker from Velneo API");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating broker in Velneo API");
                throw;
            }
        }

        public async Task UpdateBrokerAsync(BrokerDto brokerDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(brokerDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/corredores/{brokerDto.Id}");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated broker {BrokerId} in Velneo API for tenant {TenantId}",
                    brokerDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating broker {BrokerId} in Velneo API", brokerDto.Id);
                throw;
            }
        }

        public async Task DeleteBrokerAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/corredores/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting broker {BrokerId} in Velneo API", id);
                throw;
            }
        }

        public async Task<BrokerDto?> GetBrokerByCodeAsync(string code)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/corredores/code/{Uri.EscapeDataString(code)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<BrokerDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting broker by code from Velneo API");
                throw;
            }
        }

        public async Task<BrokerDto?> GetBrokerByEmailAsync(string email)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/corredores/email/{Uri.EscapeDataString(email)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<BrokerDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting broker by email from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/corredores/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<BrokerDto>>(_jsonOptions);
                return result ?? new List<BrokerDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching brokers in Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<BrokerDto>> GetAllBrokersAsync()
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/corredores");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<BrokerDto>>(_jsonOptions);
                return result ?? new List<BrokerDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all brokers from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/corredores/lookup");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<BrokerLookupDto>>(_jsonOptions);
                return result ?? new List<BrokerLookupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brokers for lookup from Velneo API");
                throw;
            }
        }

        #endregion

        #region Currency Operations

        public async Task<CurrencyDto?> GetCurrencyAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/monedas/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CurrencyDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currency {CurrencyId} from Velneo API", id);
                throw;
            }
        }

        public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating currency in Velneo API: {CurrencyCode} for tenant {TenantId}", currencyDto.Codigo, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var json = JsonSerializer.Serialize(currencyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/monedas");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<CurrencyDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize created currency from Velneo API");
                }

                _logger.LogInformation("Successfully created currency {CurrencyId} in Velneo API for tenant {TenantId}", result.Id, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating currency in Velneo API");
                throw;
            }
        }

        public async Task UpdateCurrencyAsync(CurrencyDto currencyDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(currencyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/monedas/{currencyDto.Id}");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated currency {CurrencyId} in Velneo API for tenant {TenantId}",
                    currencyDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating currency {CurrencyId} in Velneo API", currencyDto.Id);
                throw;
            }
        }

        public async Task DeleteCurrencyAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Deleting currency {CurrencyId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/monedas/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted currency {CurrencyId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting currency {CurrencyId} in Velneo API", id);
                throw;
            }
        }

        public async Task<CurrencyDto?> GetCurrencyByCodeAsync(string code)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/monedas/code/{Uri.EscapeDataString(code)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CurrencyDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currency by code from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/monedas");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CurrencyDto>>(_jsonOptions);
                return result ?? new List<CurrencyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all currencies from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/monedas/lookup");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CurrencyLookupDto>>(_jsonOptions);
                return result ?? new List<CurrencyLookupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currencies for lookup from Velneo API");
                throw;
            }
        }

        public async Task<CurrencyDto?> GetDefaultCurrencyAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/monedas/default");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CurrencyDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default currency from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/monedas/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CurrencyDto>>(_jsonOptions);
                return result ?? new List<CurrencyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching currencies in Velneo API");
                throw;
            }
        }

        #endregion

        #region Company Operations

        public async Task<CompanyDto?> GetCompanyAsync(int id)
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/companias/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {CompanyId} from Velneo API", id);
                throw;
            }
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating company in Velneo API: {CompanyName} for tenant {TenantId}", companyDto.Nombre, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var json = JsonSerializer.Serialize(companyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/companias");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize created company from Velneo API");
                }

                _logger.LogInformation("Successfully created company {CompanyId} in Velneo API for tenant {TenantId}", result.Id, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company in Velneo API");
                throw;
            }
        }

        public async Task UpdateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(companyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/companias/{companyDto.Id}");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated company {CompanyId} in Velneo API for tenant {TenantId}",
                    companyDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company {CompanyId} in Velneo API", companyDto.Id);
                throw;
            }
        }

        public async Task DeleteCompanyAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Deleting company {CompanyId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/companias/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted company {CompanyId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company {CompanyId} in Velneo API", id);
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByCodeAsync(string code)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/companias/code/{Uri.EscapeDataString(code)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by code from Velneo API");
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/companias/alias/{Uri.EscapeDataString(alias)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by alias from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/companias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>(_jsonOptions);
                return result ?? new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all companies from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/companias/lookup");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyLookupDto>>(_jsonOptions);
                return result ?? new List<CompanyLookupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies for lookup from Velneo API");
                throw;
            }
        }
        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/companias/active");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>(_jsonOptions);
                return result ?? new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active companies from Velneo API");
                throw;
            }
        }
        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/companias/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>(_jsonOptions);
                return result ?? new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching companies in Velneo API");
                throw;
            }
        }

        #endregion

        #region Poliza Operations

        public async Task<PolizaDto?> GetPolizaAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting poliza {PolizaId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Poliza {PolizaId} not found in Velneo API for tenant {TenantId}", id, tenantId);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);

                _logger.LogDebug("Successfully retrieved poliza {PolizaId} from Velneo API for tenant {TenantId}", id, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza {PolizaId} from Velneo API", id);
                throw;
            }
        }

        public async Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating poliza in Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var json = JsonSerializer.Serialize(polizaDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/polizas");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize created poliza from Velneo API");
                }

                _logger.LogInformation("Successfully created poliza {PolizaId} in Velneo API for tenant {TenantId}", result.Id, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating poliza in Velneo API");
                throw;
            }
        }

        public async Task UpdatePolizaAsync(PolizaDto polizaDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Updating poliza {PolizaId} in Velneo API for tenant {TenantId}", polizaDto.Id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var json = JsonSerializer.Serialize(polizaDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Cambio: POST en lugar de PUT para updates
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/{polizaDto.Id}");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated poliza {PolizaId} in Velneo API for tenant {TenantId}", polizaDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating poliza {PolizaId} in Velneo API", polizaDto.Id);
                throw;
            }
        }

        public async Task DeletePolizaAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Deleting poliza {PolizaId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted poliza {PolizaId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting poliza {PolizaId} in Velneo API", id);
                throw;
            }
        }

        public async Task<PolizaDto?> GetPolizaByNumberAsync(string policyNumber)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/number/{Uri.EscapeDataString(policyNumber)}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza by number from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clientId)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/client/{clientId}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas by client {ClientId} from Velneo API", clientId);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching polizas in Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetAllPolizasAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/polizas");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all polizas from Velneo API");
                throw;
            }
        }

        public async Task<PolizaDto> RenewPolizaAsync(int polizaId, RenovationDto renovationData)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Renewing poliza {PolizaId} in Velneo API for tenant {TenantId}", polizaId, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var json = JsonSerializer.Serialize(renovationData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/{polizaId}/renew");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize renewed poliza from Velneo API");
                }

                _logger.LogInformation("Successfully renewed poliza {PolizaId} in Velneo API for tenant {TenantId}", polizaId, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing poliza {PolizaId} in Velneo API", polizaId);
                throw;
            }
        }

        #endregion

        #region User Operations

        public async Task<UserDto?> GetUsersAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/users/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/users");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>(_jsonOptions);
                return result ?? new List<UserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users from Velneo API");
                throw;
            }
        }

        #endregion

        #region System Operations

        public async Task<bool> TestConnectivityAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/system/health");
                var response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connectivity to Velneo API");
                return false;
            }
        }

        public async Task<Dictionary<string, object>> GetSystemInfoAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/system/info");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
                return result ?? new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system info from Velneo API");
                throw;
            }
        }

        public async Task<Dictionary<string, object>> GetHealthStatusAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync("v1/system/health");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
                return result ?? new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting health status from Velneo API");
                throw;
            }
        }

        #endregion

        #region Batch Operations

        public async Task<IEnumerable<ClientDto>> GetClientsBatchAsync(IEnumerable<int> ids)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var idsString = string.Join(",", ids);
                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/batch?ids={idsString}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<ClientDto>>(_jsonOptions);
                return result ?? new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients batch from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasBatchAsync(IEnumerable<int> ids)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var idsString = string.Join(",", ids);
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/batch?ids={idsString}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas batch from Velneo API");
                throw;
            }
        }

        public async Task<BatchOperationResult<ClientDto>> CreateClientsBatchAsync(IEnumerable<ClientDto> clients)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(clients, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/clientes/batch");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<BatchOperationResult<ClientDto>>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize batch operation result from Velneo API");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clients batch in Velneo API");
                throw;
            }
        }

        public async Task<BatchOperationResult<PolizaDto>> CreatePolizasBatchAsync(IEnumerable<PolizaDto> polizas)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var json = JsonSerializer.Serialize(polizas, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/polizas/batch");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<BatchOperationResult<PolizaDto>>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize batch operation result from Velneo API");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating polizas batch in Velneo API");
                throw;
            }
        }

        #endregion

        #region Search and Filter Operations

        public async Task<PagedResult<ClientDto>> SearchClientsPagedAsync(string searchTerm, int page = 1, int pageSize = 50)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/search/paged?term={Uri.EscapeDataString(searchTerm)}&page={page}&pageSize={pageSize}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PagedResult<ClientDto>>(_jsonOptions);
                return result ?? new PagedResult<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients paged in Velneo API");
                throw;
            }
        }

        public async Task<PagedResult<PolizaDto>> SearchPolizasPagedAsync(string searchTerm, int page = 1, int pageSize = 50)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/search/paged?term={Uri.EscapeDataString(searchTerm)}&page={page}&pageSize={pageSize}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PagedResult<PolizaDto>>(_jsonOptions);
                return result ?? new PagedResult<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching polizas paged in Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> GetClientsModifiedSinceAsync(DateTime since)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var sinceString = since.ToString("yyyy-MM-ddTHH:mm:ss");
                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/modified-since?since={Uri.EscapeDataString(sinceString)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<ClientDto>>(_jsonOptions);
                return result ?? new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients modified since {Since} from Velneo API", since);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasModifiedSinceAsync(DateTime since)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var sinceString = since.ToString("yyyy-MM-ddTHH:mm:ss");
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/modified-since?since={Uri.EscapeDataString(sinceString)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas modified since {Since} from Velneo API", since);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasExpiringAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                using var httpClient = await GetConfiguredHttpClientAsync();

                var fromString = fromDate.ToString("yyyy-MM-dd");
                var toString = toDate.ToString("yyyy-MM-dd");
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/expiring?from={fromString}&to={toString}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas expiring between {FromDate} and {ToDate} from Velneo API", fromDate, toDate);
                throw;
            }
        }

        #endregion
    }
}