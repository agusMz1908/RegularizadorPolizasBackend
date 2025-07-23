using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVelneoApiService
    {
        Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(int page = 1, int pageSize = 50, string? search = null);
        Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasPaginatedAsync(int page = 1, int pageSize = 50, string? search = null);
        Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasByClientPaginatedAsync(int clienteId, int page = 1, int pageSize = 25, string? search = null);
        Task<ClientDto> GetClienteAsync(int id);
        Task<IEnumerable<ClientDto>> GetClientesAsync();
        Task<IEnumerable<ClientDto>> SearchClientesDirectAsync(string searchTerm);
        Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm);
        Task<ClientDto> CreateClienteAsync(ClientDto clienteDto);
        Task UpdateClienteAsync(ClientDto clienteDto);
        Task DeleteClienteAsync(int id);
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<PolizaDto> GetPolizaByNumberAsync(string numeroPoliza);
        Task<IEnumerable<PolizaDto>> GetPolizasAsync();
        Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);
        Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto);
        Task UpdatePolizaAsync(PolizaDto polizaDto);
        Task DeletePolizaAsync(int id);

        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();

        Task<SeccionDto> GetSeccionAsync(int id);
        Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
        Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId);
        Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
        Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();
        Task<SeccionDto> CreateSeccionAsync(SeccionDto seccionDto);
        Task UpdateSeccionAsync(SeccionDto seccionDto);
        Task DeleteSeccionAsync(int id);

        Task<BrokerDto> GetBrokerAsync(int id);
        Task<IEnumerable<BrokerDto>> GetBrokersAsync();
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id);
        Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm);
        Task<CurrencyDto?> GetDefaultCurrencyAsync();
        Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync();
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync();
        Task<IEnumerable<DestinoDto>> GetAllDestinosAsync();
        Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
        Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync();
    }
}