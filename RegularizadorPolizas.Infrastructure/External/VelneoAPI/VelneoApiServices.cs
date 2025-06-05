using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI
{
    public class VelneoApiService : IVelneoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VelneoApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public VelneoApiService(
            HttpClient httpClient,
            ILogger<VelneoApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        #region Client Operations

        public async Task<ClientDto?> GetClientAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting client {ClientId} from Velneo API", id);

                var response = await _httpClient.GetAsync($"clients/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Client {ClientId} not found in Velneo API", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<ClientDto>(_jsonOptions);

                _logger.LogDebug("Successfully retrieved client {ClientId} from Velneo API", id);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error getting client {ClientId} from Velneo API", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting client {ClientId} from Velneo API", id);
                throw;
            }
        }

        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto)
        {
            try
            {
                _logger.LogDebug("Creating client in Velneo API: {ClientName}", clientDto.Clinom);

                var json = JsonSerializer.Serialize(clientDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("clients", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ClientDto>(_jsonOptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize created client from Velneo API");
                }

                _logger.LogInformation("Successfully created client {ClientId} in Velneo API", result.Id);
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
                _logger.LogDebug("Updating client {ClientId} in Velneo API", clientDto.Id);

                var json = JsonSerializer.Serialize(clientDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"clients/{clientDto.Id}", content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated client {ClientId} in Velneo API", clientDto.Id);
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
                _logger.LogDebug("Deleting client {ClientId} in Velneo API", id);

                var response = await _httpClient.DeleteAsync($"clients/{id}");
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted client {ClientId} in Velneo API", id);
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
                var response = await _httpClient.GetAsync($"clients/email/{Uri.EscapeDataString(email)}");

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
                var response = await _httpClient.GetAsync($"clients/document/{Uri.EscapeDataString(document)}");

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
                var response = await _httpClient.GetAsync($"clients/search?term={Uri.EscapeDataString(searchTerm)}");
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
                var response = await _httpClient.GetAsync("clients");
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
                var response = await _httpClient.GetAsync($"brokers/{id}");

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
                var json = JsonSerializer.Serialize(brokerDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("brokers", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<BrokerDto>(_jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize created broker");
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
                var json = JsonSerializer.Serialize(brokerDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"brokers/{brokerDto.Id}", content);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.DeleteAsync($"brokers/{id}");
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
                var response = await _httpClient.GetAsync($"brokers/code/{Uri.EscapeDataString(code)}");

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
                var response = await _httpClient.GetAsync($"brokers/email/{Uri.EscapeDataString(email)}");

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
                var response = await _httpClient.GetAsync($"brokers/search?term={Uri.EscapeDataString(searchTerm)}");
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
                var response = await _httpClient.GetAsync("brokers");
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
                var response = await _httpClient.GetAsync("brokers/lookup");
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
                var response = await _httpClient.GetAsync($"currencies/{id}");

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
                var json = JsonSerializer.Serialize(currencyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("currencies", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<CurrencyDto>(_jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize created currency");
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
                var json = JsonSerializer.Serialize(currencyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"currencies/{currencyDto.Id}", content);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.DeleteAsync($"currencies/{id}");
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.GetAsync($"currencies/code/{Uri.EscapeDataString(code)}");

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
                var response = await _httpClient.GetAsync("currencies");
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
                var response = await _httpClient.GetAsync("currencies/lookup");
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
                var response = await _httpClient.GetAsync("currencies/default");

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
                var allCurrencies = await GetAllCurrenciesAsync();

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return allCurrencies;

                var normalizedSearchTerm = searchTerm.Trim().ToLower();

                return allCurrencies.Where(c =>
                    (c.Nombre?.ToLower().Contains(normalizedSearchTerm) ?? false) ||
                    (c.Moneda?.ToLower().Contains(normalizedSearchTerm) ?? false) ||
                    (c.Simbolo?.ToLower().Contains(normalizedSearchTerm) ?? false)
                );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error searching currencies in Velneo API with term: {SearchTerm}", searchTerm);
                throw;
            }
        }
        #endregion

        #region Company Operations

        public async Task<CompanyDto?> GetCompanyAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"companies/{id}");

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
                var json = JsonSerializer.Serialize(companyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("companies", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize created company");
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
                var json = JsonSerializer.Serialize(companyDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"companies/{companyDto.Id}", content);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.DeleteAsync($"companies/{id}");
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.GetAsync($"companies/code/{Uri.EscapeDataString(code)}");

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
                var response = await _httpClient.GetAsync($"companies/alias/{Uri.EscapeDataString(alias)}");

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
                var response = await _httpClient.GetAsync("companies");
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
                var response = await _httpClient.GetAsync("companies/lookup");
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

        #endregion

        #region Poliza Operations

        public async Task<PolizaDto?> GetPolizaAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"polizas/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);
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
                var json = JsonSerializer.Serialize(polizaDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("polizas", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize created poliza");
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
                var json = JsonSerializer.Serialize(polizaDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"polizas/{polizaDto.Id}", content);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.DeleteAsync($"polizas/{id}");
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.GetAsync($"polizas/number/{Uri.EscapeDataString(policyNumber)}");

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
                var response = await _httpClient.GetAsync($"polizas/client/{clientId}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PolizaDto>>(_jsonOptions);
                return result ?? new List<PolizaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas by client from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"polizas/search?term={Uri.EscapeDataString(searchTerm)}");
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
                var response = await _httpClient.GetAsync("polizas");
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
                var json = JsonSerializer.Serialize(renovationData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"polizas/{polizaId}/renew", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PolizaDto>(_jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize renewed poliza");
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
                var response = await _httpClient.GetAsync($"users/{id}");

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
                var response = await _httpClient.GetAsync("users");
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
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, object>> GetSystemInfoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("system/info");
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
                var response = await _httpClient.GetAsync("health");
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
            var results = new List<ClientDto>();
            foreach (var id in ids)
            {
                try
                {
                    var client = await GetClientAsync(id);
                    if (client != null)
                    {
                        results.Add(client);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get client {ClientId} in batch operation", id);
                }
            }
            return results;
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasBatchAsync(IEnumerable<int> ids)
        {
            var results = new List<PolizaDto>();
            foreach (var id in ids)
            {
                try
                {
                    var poliza = await GetPolizaAsync(id);
                    if (poliza != null)
                    {
                        results.Add(poliza);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get poliza {PolizaId} in batch operation", id);
                }
            }
            return results;
        }

        public async Task<BatchOperationResult<ClientDto>> CreateClientsBatchAsync(IEnumerable<ClientDto> clients)
        {
            var result = new BatchOperationResult<ClientDto>
            {
                TotalRequested = clients.Count()
            };

            var startTime = DateTime.UtcNow;

            foreach (var (client, index) in clients.Select((c, i) => (c, i)))
            {
                try
                {
                    var created = await CreateClientAsync(client);
                    result.SuccessfulItems.Add(created);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BatchOperationError
                    {
                        Index = index,
                        Error = ex.Message,
                        Item = client
                    });
                    result.ErrorCount++;
                }
            }

            result.Duration = DateTime.UtcNow - startTime;
            return result;
        }

        public async Task<BatchOperationResult<PolizaDto>> CreatePolizasBatchAsync(IEnumerable<PolizaDto> polizas)
        {
            var result = new BatchOperationResult<PolizaDto>
            {
                TotalRequested = polizas.Count()
            };

            var startTime = DateTime.UtcNow;

            foreach (var (poliza, index) in polizas.Select((p, i) => (p, i)))
            {
                try
                {
                    var created = await CreatePolizaAsync(poliza);
                    result.SuccessfulItems.Add(created);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BatchOperationError
                    {
                        Index = index,
                        Error = ex.Message,
                        Item = poliza
                    });
                    result.ErrorCount++;
                }
            }

            result.Duration = DateTime.UtcNow - startTime;
            return result;
        }

        #endregion

        #region Search and Filter Operations

        public async Task<PagedResult<ClientDto>> SearchClientsPagedAsync(string searchTerm, int page = 1, int pageSize = 50)
        {
            try
            {
                var allClients = await SearchClientsAsync(searchTerm);
                var clientsList = allClients.ToList();

                var totalCount = clientsList.Count;
                var skip = (page - 1) * pageSize;
                var pagedItems = clientsList.Skip(skip).Take(pageSize);

                return new PagedResult<ClientDto>
                {
                    Items = pagedItems,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in paged client search");
                throw;
            }
        }

        public async Task<PagedResult<PolizaDto>> SearchPolizasPagedAsync(string searchTerm, int page = 1, int pageSize = 50)
        {
            try
            {
                var allPolizas = await SearchPolizasAsync(searchTerm);
                var polizasList = allPolizas.ToList();

                var totalCount = polizasList.Count;
                var skip = (page - 1) * pageSize;
                var pagedItems = polizasList.Skip(skip).Take(pageSize);

                return new PagedResult<PolizaDto>
                {
                    Items = pagedItems,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in paged poliza search");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> GetClientsModifiedSinceAsync(DateTime since)
        {
            _logger.LogWarning("GetClientsModifiedSinceAsync not fully implemented - returning all clients");
            return await GetAllClientsAsync();
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasModifiedSinceAsync(DateTime since)
        {
            _logger.LogWarning("GetPolizasModifiedSinceAsync not fully implemented - returning all polizas");
            return await GetAllPolizasAsync();
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasExpiringAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var allPolizas = await GetAllPolizasAsync();
                return allPolizas.Where(p =>
                    p.Confchhas.HasValue &&
                    p.Confchhas.Value.Date >= fromDate.Date &&
                    p.Confchhas.Value.Date <= toDate.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring polizas");
                throw;
            }
        }

        #endregion
    }
}