using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Configuration;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVelneoApiService
    {
        #region Client Operations
        Task<ClientDto?> GetClientAsync(int id);
        Task<ClientDto> CreateClientAsync(ClientDto clientDto);
        Task UpdateClientAsync(ClientDto clientDto);
        Task DeleteClientAsync(int id);
        Task<ClientDto?> GetClientByEmailAsync(string email);
        Task<ClientDto?> GetClientByDocumentAsync(string document);
        Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        #endregion

        #region Broker Operations
        Task<BrokerDto?> GetBrokerAsync(int id);
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id);
        Task<BrokerDto?> GetBrokerByCodeAsync(string code);
        Task<BrokerDto?> GetBrokerByEmailAsync(string email);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
        Task<IEnumerable<BrokerDto>> GetAllBrokersAsync();
        Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync();
        #endregion

        #region Currency Operations
        Task<CurrencyDto?> GetCurrencyAsync(int id);
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task DeleteCurrencyAsync(int id);
        Task<CurrencyDto?> GetCurrencyByCodeAsync(string code);
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync();
        Task<CurrencyDto?> GetDefaultCurrencyAsync();
        #endregion

        #region Company Operations
        Task<CompanyDto?> GetCompanyAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);
        Task<CompanyDto?> GetCompanyByCodeAsync(string code);
        Task<CompanyDto?> GetCompanyByAliasAsync(string alias);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
        #endregion

        #region Poliza Operations
        Task<PolizaDto?> GetPolizaAsync(int id);
        Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto);
        Task UpdatePolizaAsync(PolizaDto polizaDto);
        Task DeletePolizaAsync(int id);
        Task<PolizaDto?> GetPolizaByNumberAsync(string policyNumber);
        Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clientId);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);
        Task<IEnumerable<PolizaDto>> GetAllPolizasAsync();
        Task<PolizaDto> RenewPolizaAsync(int polizaId, RenovationDto renovationData);
        #endregion

        #region User Operations (if needed)
        Task<UserDto?> GetUsersAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        #endregion

        #region System Operations
        Task<bool> TestConnectivityAsync();
        Task<Dictionary<string, object>> GetSystemInfoAsync();
        Task<Dictionary<string, object>> GetHealthStatusAsync();
        #endregion

        #region Batch Operations
        Task<IEnumerable<ClientDto>> GetClientsBatchAsync(IEnumerable<int> ids);
        Task<IEnumerable<PolizaDto>> GetPolizasBatchAsync(IEnumerable<int> ids);
        Task<BatchOperationResult<ClientDto>> CreateClientsBatchAsync(IEnumerable<ClientDto> clients);
        Task<BatchOperationResult<PolizaDto>> CreatePolizasBatchAsync(IEnumerable<PolizaDto> polizas);
        #endregion

        #region Search and Filter Operations
        Task<PagedResult<ClientDto>> SearchClientsPagedAsync(string searchTerm, int page = 1, int pageSize = 50);
        Task<PagedResult<PolizaDto>> SearchPolizasPagedAsync(string searchTerm, int page = 1, int pageSize = 50);
        Task<IEnumerable<ClientDto>> GetClientsModifiedSinceAsync(DateTime since);
        Task<IEnumerable<PolizaDto>> GetPolizasModifiedSinceAsync(DateTime since);
        Task<IEnumerable<PolizaDto>> GetPolizasExpiringAsync(DateTime fromDate, DateTime toDate);
        #endregion
    }

    public class BatchOperationResult<T>
    {
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<T> SuccessfulItems { get; set; } = new();
        public List<BatchOperationError> Errors { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        public double SuccessRate => TotalRequested > 0 ? (double)SuccessCount / TotalRequested * 100 : 0;
        public bool AllSucceeded => ErrorCount == 0;
    }

    public class BatchOperationError
    {
        public int Index { get; set; }
        public string Error { get; set; } = string.Empty;
        public object? Item { get; set; }
        public string? ErrorCode { get; set; }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    // Configuración específica para Velneo
    public class VelneoApiConfiguration
    {
        public const string SectionName = "VelneoAPI";

        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public bool EnableRetryPolicy { get; set; } = true;
        public RetryPolicyConfiguration RetryPolicy { get; set; } = new();
        public bool EnableLogging { get; set; } = true;
        public bool LogRequestResponse { get; set; } = false;
        public string ApiVersion { get; set; } = "v1";

        // Headers adicionales si son necesarios
        public Dictionary<string, string> DefaultHeaders { get; set; } = new();

        // Configuración de endpoints específicos
        public VelneoEndpointsConfiguration Endpoints { get; set; } = new();
    }

    public class RetryPolicyConfiguration
    {
        public int MaxRetries { get; set; } = 3;
        public int BaseDelaySeconds { get; set; } = 1;
        public int MaxDelaySeconds { get; set; } = 30;
        public bool UseExponentialBackoff { get; set; } = true;
    }

    public class VelneoEndpointsConfiguration
    {
        public string Clients { get; set; } = "clients";
        public string Brokers { get; set; } = "brokers";
        public string Currencies { get; set; } = "currencies";
        public string Companies { get; set; } = "companies";
        public string Polizas { get; set; } = "polizas";
        public string Users { get; set; } = "users";
        public string Health { get; set; } = "health";
        public string System { get; set; } = "system";
    }
}