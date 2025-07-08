using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

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
            var baseUrl = tenantConfig.BaseUrl.TrimEnd('/') + "/";
            httpClient.BaseAddress = new Uri(baseUrl);

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
                tenantConfig.TenantId, baseUrl, tenantConfig.TimeoutSeconds);

            return httpClient;
        }

        private async Task<string> BuildUrlWithApiKeyAsync(string endpoint)
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
            return $"{endpoint}?api_key={tenantConfig.Key}";
        }


        #region Métodos de Clientes

        public async Task<ClientDto> GetClienteAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting cliente {ClienteId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoClienteResponse>(_jsonOptions);

                if (velneoResponse?.Cliente == null)
                {
                    throw new KeyNotFoundException($"Cliente with ID {id} not found in Velneo API");
                }

                var result = velneoResponse.Cliente.ToClienteDto();
                _logger.LogInformation("Successfully retrieved cliente {ClienteId} from Velneo API for tenant {TenantId}", id, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cliente {ClienteId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> GetClientesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all clientes from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/clientes");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoClientesResponse>(_jsonOptions);

                if (velneoResponse?.Clientes == null || !velneoResponse.Clientes.Any())
                {
                    _logger.LogWarning("No clientes received from Velneo API for tenant {TenantId}", tenantId);
                    return new List<ClientDto>();
                }

                var clientes = velneoResponse.Clientes.ToClienteDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} clientes from Velneo API for tenant {TenantId}",
                    clientes.Count, tenantId);

                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clientes from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Searching clientes with term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoClientesResponse>(_jsonOptions);

                if (velneoResponse?.Clientes == null || !velneoResponse.Clientes.Any())
                {
                    _logger.LogWarning("No clientes found for search term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);
                    return new List<ClientDto>();
                }

                var clientes = velneoResponse.Clientes.ToClienteDtos().ToList();
                _logger.LogInformation("Successfully found {Count} clientes for search term '{SearchTerm}' in Velneo API for tenant {TenantId}",
                    clientes.Count, searchTerm, tenantId);

                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clientes with term '{SearchTerm}' in Velneo API", searchTerm);
                throw;
            }
        }

        public async Task<ClientDto> CreateClienteAsync(ClientDto clienteDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating cliente in Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // Convertir a formato Velneo
                var velneoCliente = clienteDto.ToVelneoClienteDto();
                var json = JsonSerializer.Serialize(velneoCliente, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/clientes");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoClienteResponse>(_jsonOptions);

                if (velneoResponse?.Cliente == null)
                {
                    throw new InvalidOperationException("Failed to create cliente in Velneo API - no response data");
                }

                var result = velneoResponse.Cliente.ToClienteDto();
                _logger.LogInformation("Successfully created cliente {ClienteId} in Velneo API for tenant {TenantId}", result.Id, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cliente in Velneo API");
                throw;
            }
        }

        public async Task UpdateClienteAsync(ClientDto clienteDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Updating cliente {ClienteId} in Velneo API for tenant {TenantId}", clienteDto.Id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // Convertir a formato Velneo
                var velneoCliente = clienteDto.ToVelneoClienteDto();
                var json = JsonSerializer.Serialize(velneoCliente, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/{clienteDto.Id}");
                var response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated cliente {ClienteId} in Velneo API for tenant {TenantId}", clienteDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cliente {ClienteId} in Velneo API", clienteDto.Id);
                throw;
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Deleting cliente {ClienteId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/clientes/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted cliente {ClienteId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cliente {ClienteId} from Velneo API", id);
                throw;
            }
        }

        #endregion

        #region Broker Operations

        public async Task<BrokerDto> GetBrokerAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting broker {BrokerId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/brokers/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoBrokerResponse>(_jsonOptions);

                if (velneoResponse?.Broker == null)
                {
                    throw new KeyNotFoundException($"Broker with ID {id} not found in Velneo API");
                }

                var result = velneoResponse.Broker.ToBrokerDto();
                _logger.LogInformation("Successfully retrieved broker {BrokerId} from Velneo API for tenant {TenantId}", id, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting broker {BrokerId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<BrokerDto>> GetBrokersAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all brokers from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/brokers");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoBrokersResponse>(_jsonOptions);

                if (velneoResponse?.Brokers == null || !velneoResponse.Brokers.Any())
                {
                    _logger.LogWarning("No brokers received from Velneo API for tenant {TenantId}", tenantId);
                    return new List<BrokerDto>();
                }

                var brokers = velneoResponse.Brokers.ToBrokerDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} brokers from Velneo API for tenant {TenantId}",
                    brokers.Count, tenantId);

                return brokers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brokers from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Searching brokers with term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/brokers/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoBrokersResponse>(_jsonOptions);

                if (velneoResponse?.Brokers == null || !velneoResponse.Brokers.Any())
                {
                    _logger.LogWarning("No brokers found for search term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);
                    return new List<BrokerDto>();
                }

                var brokers = velneoResponse.Brokers.ToBrokerDtos().ToList();
                _logger.LogInformation("Successfully found {Count} brokers for search term '{SearchTerm}' in Velneo API for tenant {TenantId}",
                    brokers.Count, searchTerm, tenantId);

                return brokers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching brokers with term '{SearchTerm}' in Velneo API", searchTerm);
                throw;
            }
        }

        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating broker in Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                var velneoBroker = brokerDto.ToVelneoBrokerDto();
                var json = JsonSerializer.Serialize(velneoBroker, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/brokers");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoBrokerResponse>(_jsonOptions);

                if (velneoResponse?.Broker == null)
                {
                    throw new InvalidOperationException("Failed to create broker in Velneo API - no response data");
                }

                var result = velneoResponse.Broker.ToBrokerDto();
                _logger.LogInformation("Successfully created broker {BrokerId} in Velneo API for tenant {TenantId}", result.Id, tenantId);

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
                _logger.LogDebug("Updating broker {BrokerId} in Velneo API for tenant {TenantId}", brokerDto.Id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                var velneoBroker = brokerDto.ToVelneoBrokerDto();
                var json = JsonSerializer.Serialize(velneoBroker, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/brokers/{brokerDto.Id}");
                var response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated broker {BrokerId} in Velneo API for tenant {TenantId}", brokerDto.Id, tenantId);
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
                _logger.LogDebug("Deleting broker {BrokerId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/brokers/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted broker {BrokerId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting broker {BrokerId} from Velneo API", id);
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

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoCompaniesResponse>(_jsonOptions);

                if (velneoResponse?.Companias == null)
                {
                    return new List<CompanyDto>();
                }

                var companies = velneoResponse.Companias.ToCompanyDtos().ToList();

                _logger.LogInformation("Successfully retrieved {Count} total companies from Velneo API", companies.Count);
                return companies;
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

                var url = await BuildUrlWithApiKeyAsync("v1/companias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoCompaniesResponse>(_jsonOptions);

                if (velneoResponse?.Companias == null || !velneoResponse.Companias.Any())
                {
                    _logger.LogWarning("No companies received from Velneo API");
                    return new List<CompanyDto>();
                }

                var companies = velneoResponse.Companias.ToCompanyDtos().ToList();

                _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API for tenant {TenantId}",
                    companies.Count, tenantId);

                return companies;
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

        #region Métodos de Secciones

        public async Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting active secciones from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/secciones");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoSeccionesResponse>(_jsonOptions);

                if (velneoResponse?.Secciones == null || !velneoResponse.Secciones.Any())
                {
                    _logger.LogWarning("No secciones received from Velneo API for tenant {TenantId}", tenantId);
                    return new List<SeccionDto>();
                }

                var secciones = velneoResponse.Secciones.ToSeccionDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} secciones from Velneo API for tenant {TenantId}",
                    secciones.Count, tenantId);

                return secciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active secciones from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting secciones for company {CompanyId} from Velneo API for tenant {TenantId}",
                    companyId, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/secciones/compania/{companyId}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoSeccionesResponse>(_jsonOptions);

                if (velneoResponse?.Secciones == null || !velneoResponse.Secciones.Any())
                {
                    _logger.LogWarning("No secciones found for company {CompanyId} in Velneo API for tenant {TenantId}",
                        companyId, tenantId);
                    return new List<SeccionDto>();
                }

                var secciones = velneoResponse.Secciones.ToSeccionDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} secciones for company {CompanyId} from Velneo API for tenant {TenantId}",
                    secciones.Count, companyId, tenantId);

                return secciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for company {CompanyId} from Velneo API", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting secciones for lookup from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/secciones/lookup");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoSeccionesLookupResponse>(_jsonOptions);

                if (velneoResponse?.Secciones == null || !velneoResponse.Secciones.Any())
                {
                    _logger.LogWarning("No secciones received for lookup from Velneo API for tenant {TenantId}", tenantId);
                    return new List<SeccionLookupDto>();
                }

                var secciones = velneoResponse.Secciones.ToSeccionLookupDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} secciones for lookup from Velneo API for tenant {TenantId}",
                    secciones.Count, tenantId);

                return secciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for lookup from Velneo API");
                throw;
            }
        }

        public async Task<SeccionDto> GetSeccionAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting seccion {SeccionId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/secciones/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoSeccionResponse>(_jsonOptions);

                if (velneoResponse?.Seccion == null)
                {
                    throw new KeyNotFoundException($"Seccion with ID {id} not found in Velneo API");
                }

                var result = velneoResponse.Seccion.ToSeccionDto();
                _logger.LogInformation("Successfully retrieved seccion {SeccionId} from Velneo API for tenant {TenantId}", id, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seccion {SeccionId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Searching secciones with term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/secciones/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoSeccionesResponse>(_jsonOptions);

                if (velneoResponse?.Secciones == null || !velneoResponse.Secciones.Any())
                {
                    _logger.LogWarning("No secciones found for search term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);
                    return new List<SeccionDto>();
                }

                var secciones = velneoResponse.Secciones.ToSeccionDtos().ToList();
                _logger.LogInformation("Successfully found {Count} secciones for search term '{SearchTerm}' in Velneo API for tenant {TenantId}",
                    secciones.Count, searchTerm, tenantId);

                return secciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching secciones with term '{SearchTerm}' in Velneo API", searchTerm);
                throw;
            }
        }

        public async Task<SeccionDto> CreateSeccionAsync(SeccionDto seccionDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Creating seccion in Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // Convertir a formato Velneo
                var velneoSeccion = seccionDto.ToVelneoSeccionDto();
                var json = JsonSerializer.Serialize(velneoSeccion, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/secciones");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoSeccionResponse>(_jsonOptions);

                if (velneoResponse?.Seccion == null)
                {
                    throw new InvalidOperationException("Failed to create seccion in Velneo API - no response data");
                }

                var result = velneoResponse.Seccion.ToSeccionDto();
                _logger.LogInformation("Successfully created seccion {SeccionId} in Velneo API for tenant {TenantId}", result.Id, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seccion in Velneo API");
                throw;
            }
        }

        public async Task UpdateSeccionAsync(SeccionDto seccionDto)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Updating seccion {SeccionId} in Velneo API for tenant {TenantId}", seccionDto.Id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // Convertir a formato Velneo
                var velneoSeccion = seccionDto.ToVelneoSeccionDto();
                var json = JsonSerializer.Serialize(velneoSeccion, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/secciones/{seccionDto.Id}");
                var response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated seccion {SeccionId} in Velneo API for tenant {TenantId}", seccionDto.Id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating seccion {SeccionId} in Velneo API", seccionDto.Id);
                throw;
            }
        }

        public async Task DeleteSeccionAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Deleting seccion {SeccionId} in Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/secciones/{id}");
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted seccion {SeccionId} in Velneo API for tenant {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seccion {SeccionId} from Velneo API", id);
                throw;
            }
        }

        #endregion

        #region Métodos de Pólizas

        public async Task<PolizaDto> GetPolizaAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting poliza {PolizaId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoPolizaResponse>(_jsonOptions);

                if (velneoResponse?.Poliza == null)
                {
                    throw new KeyNotFoundException($"Poliza with ID {id} not found in Velneo API");
                }

                var result = velneoResponse.Poliza.ToPolizaDto();
                _logger.LogInformation("Successfully retrieved poliza {PolizaId} from Velneo API for tenant {TenantId}", id, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza {PolizaId} from Velneo API", id);
                throw;
            }
        }

        public async Task<PolizaDto> GetPolizaByNumberAsync(string numeroPoliza)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting poliza by number {NumeroPoliza} from Velneo API for tenant {TenantId}", numeroPoliza, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/numero/{Uri.EscapeDataString(numeroPoliza)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoPolizaResponse>(_jsonOptions);

                if (velneoResponse?.Poliza == null)
                {
                    throw new KeyNotFoundException($"Poliza with number {numeroPoliza} not found in Velneo API");
                }

                var result = velneoResponse.Poliza.ToPolizaDto();
                _logger.LogInformation("Successfully retrieved poliza {NumeroPoliza} from Velneo API for tenant {TenantId}", numeroPoliza, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza by number {NumeroPoliza} from Velneo API", numeroPoliza);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all polizas from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/polizas");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoPolizasResponse>(_jsonOptions);

                if (velneoResponse?.Polizas == null || !velneoResponse.Polizas.Any())
                {
                    _logger.LogWarning("No polizas received from Velneo API for tenant {TenantId}", tenantId);
                    return new List<PolizaDto>();
                }

                var polizas = velneoResponse.Polizas.ToPolizaDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} polizas from Velneo API for tenant {TenantId}", polizas.Count, tenantId);

                return polizas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting polizas for client {ClienteId} from Velneo API for tenant {TenantId}", clienteId, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/cliente/{clienteId}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoPolizasResponse>(_jsonOptions);

                if (velneoResponse?.Polizas == null || !velneoResponse.Polizas.Any())
                {
                    _logger.LogWarning("No polizas found for client {ClienteId} in Velneo API for tenant {TenantId}", clienteId, tenantId);
                    return new List<PolizaDto>();
                }

                var polizas = velneoResponse.Polizas.ToPolizaDtos().ToList();
                _logger.LogInformation("Successfully retrieved {Count} polizas for client {ClienteId} from Velneo API for tenant {TenantId}",
                    polizas.Count, clienteId, tenantId);

                return polizas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas for client {ClienteId} from Velneo API", clienteId);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Searching polizas with term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/search?term={Uri.EscapeDataString(searchTerm)}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoPolizasResponse>(_jsonOptions);

                if (velneoResponse?.Polizas == null || !velneoResponse.Polizas.Any())
                {
                    _logger.LogWarning("No polizas found for search term '{SearchTerm}' in Velneo API for tenant {TenantId}", searchTerm, tenantId);
                    return new List<PolizaDto>();
                }

                var polizas = velneoResponse.Polizas.ToPolizaDtos().ToList();
                _logger.LogInformation("Successfully found {Count} polizas for search term '{SearchTerm}' in Velneo API for tenant {TenantId}",
                    polizas.Count, searchTerm, tenantId);

                return polizas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching polizas with term '{SearchTerm}' in Velneo API", searchTerm);
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

                var velneoPoliza = polizaDto.ToVelneoPolizaDto();
                var json = JsonSerializer.Serialize(velneoPoliza, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync("v1/polizas");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoPolizaResponse>(_jsonOptions);

                if (velneoResponse?.Poliza == null)
                {
                    throw new InvalidOperationException("Failed to create poliza in Velneo API - no response data");
                }

                var result = velneoResponse.Poliza.ToPolizaDto();
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

                var velneoPoliza = polizaDto.ToVelneoPolizaDto();
                var json = JsonSerializer.Serialize(velneoPoliza, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = await BuildUrlWithApiKeyAsync($"v1/polizas/{polizaDto.Id}");
                var response = await httpClient.PutAsync(url, content);
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
                _logger.LogError(ex, "Error deleting poliza {PolizaId} from Velneo API", id);
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
    

        #region Métodos de Conectividad

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Testing connection to Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildUrlWithApiKeyAsync("v1/health");
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var healthResponse = await response.Content.ReadFromJsonAsync<VelneoHealthResponse>(_jsonOptions);
                    var isHealthy = healthResponse?.Success == true && healthResponse?.Status?.ToLower() == "healthy";

                    _logger.LogInformation("Velneo API health check for tenant {TenantId}: {Status}",
                        tenantId, isHealthy ? "HEALTHY" : "UNHEALTHY");

                    return isHealthy;
                }
                else
                {
                    _logger.LogWarning("Velneo API health check failed for tenant {TenantId} with status {StatusCode}",
                        tenantId, response.StatusCode);
                    return false;
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogWarning("Velneo API health check timed out for tenant {TenantId}", _tenantService.GetCurrentTenantId());
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection to Velneo API for tenant {TenantId}", _tenantService.GetCurrentTenantId());
                return false;
            }
        }

        #endregion
    }
}