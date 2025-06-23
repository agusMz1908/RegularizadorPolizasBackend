using RegularizadorPolizas.Application.DTOs;

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
        Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm);
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
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
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

        #region User Operations
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
}