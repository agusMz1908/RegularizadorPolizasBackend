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

            _logger.LogDebug("Creating HttpClient for tenant {TenantId} with BaseUrl: {BaseUrl}",
                tenantConfig.TenantId, tenantConfig.BaseUrl);

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.Timeout = TimeSpan.FromSeconds(tenantConfig.TimeoutSeconds);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RegularizadorPolizas-API/1.0");

            _logger.LogInformation("HttpClient configured for tenant {TenantId}: {BaseUrl} (Timeout: {Timeout}s)",
                tenantConfig.TenantId, tenantConfig.BaseUrl, tenantConfig.TimeoutSeconds);

            return httpClient;
        }

        private async Task<string> BuildVelneoUrlAsync(string endpoint)
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            // Construir URL completa según formato Velneo
            // https://app.uruguaycom.com/apid/Seguros_dat/v1/clientes?api_key=XXX
            var baseUrl = tenantConfig.BaseUrl.TrimEnd('/');
            var fullUrl = $"{baseUrl}/{endpoint}?api_key={tenantConfig.Key}";

            _logger.LogDebug("Built Velneo URL: {Url}", fullUrl);
            return fullUrl;
        }

        #region Métodos de Clientes

        public async Task<ClientDto> GetClienteAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting cliente {ClienteId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync($"v1/clientes/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Velneo cliente response: {Response}", jsonContent.Substring(0, Math.Min(200, jsonContent.Length)));

                try
                {
                    var velneoCliente = await response.Content.ReadFromJsonAsync<VelneoCliente>(_jsonOptions);
                    if (velneoCliente != null)
                    {
                        var result = velneoCliente.ToClienteDto();
                        _logger.LogInformation("Successfully retrieved cliente {ClienteId} from Velneo API", id);
                        return result;
                    }
                }
                catch (JsonException)
                {
                    var velneoResponse = JsonSerializer.Deserialize<VelneoClientResponse>(jsonContent, _jsonOptions);
                    if (velneoResponse?.Cliente != null)
                    {
                        var result = velneoResponse.Cliente.ToClienteDto();
                        _logger.LogInformation("Successfully retrieved cliente {ClienteId} from Velneo API (wrapped)", id);
                        return result;
                    }
                }

                throw new KeyNotFoundException($"Cliente with ID {id} not found in Velneo API");
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
                var url = await BuildVelneoUrlAsync("v1/clientes");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await response.Content.ReadFromJsonAsync<VelneoClientsResponse>(_jsonOptions);

                if (velneoResponse?.Clientes == null || !velneoResponse.Clientes.Any())
                {
                    _logger.LogWarning("No clientes received from Velneo API for tenant {TenantId}", tenantId);
                    return new List<ClientDto>();
                }

                var clientes = velneoResponse.Clientes.ToClienteDtos().ToList();

                _logger.LogInformation("Successfully retrieved {Count} clientes from Velneo API (total: {Total})",
                    clientes.Count, velneoResponse.TotalCount);

                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting clientes from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm)
        {
            try
            {
                var allClientes = await GetClientesAsync();
                var filtered = allClientes.Where(c =>
                    c.Clinom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Cliced?.Contains(searchTerm) == true ||
                    c.Cliruc?.Contains(searchTerm) == true
                ).ToList();

                _logger.LogInformation("Found {Count} clientes matching '{SearchTerm}'", filtered.Count, searchTerm);
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clientes with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        #endregion

        #region Métodos de Compañías

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all companies from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/companias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Velneo companies response length: {Length}", jsonContent.Length);

                // Intentar deserializar como array directo primero
                try
                {
                    var velneoCompanies = await response.Content.ReadFromJsonAsync<List<VelneoCompany>>(_jsonOptions);
                    if (velneoCompanies != null && velneoCompanies.Any())
                    {
                        var companies = velneoCompanies.ToCompanyDtos().ToList();
                        _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API", companies.Count);
                        return companies;
                    }
                }
                catch (JsonException)
                {
                    // Si falla, intentar con wrapper
                    var velneoResponse = JsonSerializer.Deserialize<VelneoCompaniesResponse>(jsonContent, _jsonOptions);
                    if (velneoResponse?.Companias != null)
                    {
                        var companies = velneoResponse.Companias.ToCompanyDtos().ToList();
                        _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (wrapped)", companies.Count);
                        return companies;
                    }
                }

                return new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            return await GetAllCompaniesAsync();
        }

        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                var companies = await GetAllCompaniesAsync();
                var lookupDtos = companies.Select(c => new CompanyLookupDto
                {
                    Id = c.Id,
                    Comnom = c.Comnom,
                    Comalias = c.Comalias,
                    Cod_srvcompanias = c.Cod_srvcompanias
                });

                return lookupDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies for lookup from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm)
        {
            try
            {
                var allCompanies = await GetAllCompaniesAsync();
                var filtered = allCompanies.Where(c =>
                    c.Comnom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Comalias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching companies");
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            try
            {
                var companies = await GetAllCompaniesAsync();
                return companies.FirstOrDefault(c =>
                    string.Equals(c.Comalias, alias, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by alias from Velneo API");
                throw;
            }
        }

        #endregion

        #region Métodos de Contratos/Pólizas

        public async Task<PolizaDto> GetPolizaAsync(int id)
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync($"v1/contratos/{id}");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Contrato response: {Response}", jsonContent.Substring(0, Math.Min(200, jsonContent.Length)));

                throw new NotImplementedException("Mapeo de contratos a pólizas pendiente de implementación");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza {PolizaId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasAsync()
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/contratos");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Contratos list response length: {Length}", jsonContent.Length);

                throw new NotImplementedException("Mapeo de contratos a pólizas pendiente de implementación");
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
                using var httpClient = await GetConfiguredHttpClientAsync();
                var baseUrl = await BuildVelneoUrlAsync("v1/contratos");
                var url = baseUrl.Replace("?api_key=", $"?filter%5Bclientes%5D={clienteId}&api_key=");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Contratos by client response length: {Length}", jsonContent.Length);

                throw new NotImplementedException("Mapeo de contratos por cliente pendiente de implementación");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas for client {ClienteId} from Velneo API", clienteId);
                throw;
            }
        }

        #endregion

        #region Métodos NO IMPLEMENTADOS

        public async Task<ClientDto> CreateClienteAsync(ClientDto clienteDto)
        {
            throw new NotImplementedException("CreateCliente no está implementado en Velneo API aún");
        }

        public async Task UpdateClienteAsync(ClientDto clienteDto)
        {
            throw new NotImplementedException("UpdateCliente no está implementado en Velneo API aún");
        }

        public async Task DeleteClienteAsync(int id)
        {
            throw new NotImplementedException("DeleteCliente no está implementado en Velneo API aún");
        }

        public async Task<PolizaDto> GetPolizaByNumberAsync(string numeroPoliza)
        {
            throw new NotImplementedException("GetPolizaByNumber no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchPolizas no está implementado en Velneo API aún");
        }

        public async Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto)
        {
            throw new NotImplementedException("CreatePoliza no está implementado en Velneo API aún");
        }

        public async Task UpdatePolizaAsync(PolizaDto polizaDto)
        {
            throw new NotImplementedException("UpdatePoliza no está implementado en Velneo API aún");
        }

        public async Task DeletePolizaAsync(int id)
        {
            throw new NotImplementedException("DeletePoliza no está implementado en Velneo API aún");
        }

        public async Task<SeccionDto> GetSeccionAsync(int id)
        {
            throw new NotImplementedException("GetSeccion no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync()
        {
            throw new NotImplementedException("GetActiveSecciones no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId)
        {
            throw new NotImplementedException("GetSeccionesByCompany no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchSecciones no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync()
        {
            throw new NotImplementedException("GetSeccionesForLookup no está implementado en Velneo API aún");
        }

        public async Task<SeccionDto> CreateSeccionAsync(SeccionDto seccionDto)
        {
            throw new NotImplementedException("CreateSeccion no está implementado en Velneo API aún");
        }

        public async Task UpdateSeccionAsync(SeccionDto seccionDto)
        {
            throw new NotImplementedException("UpdateSeccion no está implementado en Velneo API aún");
        }

        public async Task DeleteSeccionAsync(int id)
        {
            throw new NotImplementedException("DeleteSeccion no está implementado en Velneo API aún");
        }

        public async Task<BrokerDto> GetBrokerAsync(int id)
        {
            throw new NotImplementedException("GetBroker no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<BrokerDto>> GetBrokersAsync()
        {
            throw new NotImplementedException("GetBrokers no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchBrokers no está implementado en Velneo API aún");
        }

        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            throw new NotImplementedException("CreateBroker no está implementado en Velneo API aún");
        }

        public async Task UpdateBrokerAsync(BrokerDto brokerDto)
        {
            throw new NotImplementedException("UpdateBroker no está implementado en Velneo API aún");
        }

        public async Task DeleteBrokerAsync(int id)
        {
            throw new NotImplementedException("DeleteBroker no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchCurrencies no está implementado en Velneo API aún");
        }

        public async Task<CurrencyDto?> GetDefaultCurrencyAsync()
        {
            throw new NotImplementedException("GetDefaultCurrency no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync()
        {
            throw new NotImplementedException("GetCurrenciesForLookup no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
        {
            throw new NotImplementedException("GetAllCurrencies no está implementado en Velneo API aún");
        }

        #endregion

        #region Test Connection

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Testing connection to Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/clientes");
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Velneo API connection test SUCCESSFUL for tenant {TenantId}", tenantId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("⚠Velneo API connection test failed for tenant {TenantId} with status {StatusCode}",
                        tenantId, response.StatusCode);
                    return false;
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogWarning("Velneo API connection test timed out for tenant {TenantId}", _tenantService.GetCurrentTenantId());
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