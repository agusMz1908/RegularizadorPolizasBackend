using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces.External.Velneo
{

    public interface IVelneoMaestrosService
    {
        Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync();
        Task<IEnumerable<DestinoDto>> GetAllDestinosAsync();
        Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
        Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync();
        Task<IEnumerable<MonedaDto>> GetAllMonedasAsync();
        Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
        Task<SeccionDto> GetSeccionAsync(int id);
        Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();
        Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId);
        Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
        Task<IEnumerable<CoberturaDto>> GetAllCoberturasAsync();
        Task<IEnumerable<DepartamentoDto>> GetAllDepartamentosAsync();
        Task<IEnumerable<TarifaDto>> GetAllTarifasAsync();
        Task<ClientDto> GetClienteAsync(int id);
        Task<IEnumerable<ClientDto>> GetClientesAsync();
        Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(
            int page = 1,
            int pageSize = 50,
            string? search = null);
        Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm);
        Task<IEnumerable<ClientDto>> SearchClientesDirectAsync(string searchTerm);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<CompanyDto?> GetCompanyByAliasAsync(string alias);
        Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo);
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<IEnumerable<PolizaDto>> GetPolizasAsync();
        Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId);
        //Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasPaginatedAsync(
        //    int page = 1,
        //    int pageSize = 50,
        //    string? search = null);
        //Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasByClientPaginatedAsync(
        //    int clienteId,
        //    int page = 1,
        //    int pageSize = 25,
        //    string? search = null);
        Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request);
    }
}