using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Services.External
{
    public interface IVelneoApiService
    {
        #region Currency Operations
        Task<CurrencyDto?> GetCurrencyAsync(int id);
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task DeleteCurrencyAsync(int id);
        Task<CurrencyDto?> GetCurrencyByCodeAsync(string codigo);
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
        #endregion

        #region Poliza Operations
        Task<PolizaDto?> GetPolizaAsync(int id);
        Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto);
        Task UpdatePolizaAsync(PolizaDto polizaDto);
        Task DeletePolizaAsync(int id);
        Task<PolizaDto?> GetPolizaByNumberAsync(string policyNumber);
        Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clientId);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);
        #endregion

        #region Client Operations
        Task<ClientDto?> GetClientAsync(int id);
        Task<ClientDto> CreateClientAsync(ClientDto clientDto);
        Task UpdateClientAsync(ClientDto clientDto);
        Task DeleteClientAsync(int id);
        Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        #endregion

        #region Broker Operations
        Task<BrokerDto?> GetBrokerAsync(int id);
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
        Task<BrokerDto?> GetBrokerByEmailAsync(string email); 
        Task<BrokerDto?> GetBrokerByCodigoAsync(string codigo); 
        Task<IEnumerable<BrokerDto>> GetAllBrokersAsync();
        Task<IEnumerable<BrokerDto>> GetActiveBrokersAsync();
        Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync();
        #endregion

        #region Health Check
        Task<bool> TestConnectivityAsync();
        #endregion
    }
}