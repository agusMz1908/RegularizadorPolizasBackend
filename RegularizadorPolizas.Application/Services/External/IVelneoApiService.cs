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
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request);
        Task<IEnumerable<PolizaDto>> GetPolizasAsync();
        Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId);

        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();

        Task<SeccionDto> GetSeccionAsync(int id);
        Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
        Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId);
        Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
        Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();

        Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync();
        Task<IEnumerable<DestinoDto>> GetAllDestinosAsync();
        Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
        Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync();
    }
}