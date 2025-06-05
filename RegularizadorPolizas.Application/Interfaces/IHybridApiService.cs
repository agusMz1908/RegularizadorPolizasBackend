using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IHybridApiService
    {
        // Client operations
        Task<ClientDto?> GetClientAsync(int id);
        Task<ClientDto> CreateClientAsync(ClientDto clientDto);
        Task UpdateClientAsync(ClientDto clientDto);
        Task DeleteClientAsync(int id);
        Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);

        // Broker operations
        Task<BrokerDto?> GetBrokerAsync(int id);
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);

        // Currency operations
        Task<CurrencyDto?> GetCurrencyAsync(int id);
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task DeleteCurrencyAsync(int id);
        Task<CurrencyDto?> GetCurrencyByCodigoAsync(string codigo);
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync();
        Task<CurrencyDto?> GetDefaultCurrencyAsync();
        Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm);

        // Company operations
        Task<CompanyDto?> GetCompanyAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);

        // Poliza operations
        Task<PolizaDto?> GetPolizaAsync(int id);
        Task<PolizaCreationResult> CreatePolizaAsync(PolizaDto polizaDto);
        Task UpdatePolizaAsync(PolizaDto polizaDto);
        Task DeletePolizaAsync(int id);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);

        // System operations
        Task<Dictionary<string, object>> GetSystemHealthAsync();
        Task<bool> TestVelneoConnectivityAsync();
    }
}