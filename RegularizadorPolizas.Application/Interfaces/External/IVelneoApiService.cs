using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces.External
{
    public interface IVelneoApiService
    {
        // ===========================
        // MÉTODOS DE CLIENTES
        // ===========================
        Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(int page = 1, int pageSize = 50, string? search = null);
        Task<ClientDto> GetClienteAsync(int id);
        Task<IEnumerable<ClientDto>> GetClientesAsync();
        Task<IEnumerable<ClientDto>> SearchClientesDirectAsync(string searchTerm);
        Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm);

        // ===========================
        // MÉTODOS DE PÓLIZAS
        // ===========================
        Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasPaginatedAsync(int page = 1, int pageSize = 50, string? search = null);
        Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasByClientPaginatedAsync(int clienteId, int page = 1, int pageSize = 25, string? search = null);
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request);
        Task<IEnumerable<PolizaDto>> GetPolizasAsync();
        Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId);

        // ===========================
        // MÉTODOS DE COMPAÑÍAS
        // ===========================
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo);
        Task<CompanyDto?> GetCompanyByAliasAsync(string alias);

        // ===========================
        // MÉTODOS DE SECCIONES
        // ===========================
        Task<SeccionDto> GetSeccionAsync(int id);
        Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
        Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId);
        Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
        Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();

        // ===========================
        // MÉTODOS DE CATÁLOGOS/LOOKUPS
        // ===========================
        Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync();
        Task<IEnumerable<DestinoDto>> GetAllDestinosAsync();
        Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
        Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync();
        Task<IEnumerable<MonedaDto>> GetAllMonedasAsync();

        // ============================================================================
        // 🆕 NUEVOS MÉTODOS PARA MAESTROS DINÁMICOS - AGREGAR ESTOS AL FINAL
        // ============================================================================

        Task<IEnumerable<CoberturaDto>> GetAllCoberturasAsync();
        Task<IEnumerable<DepartamentoDto>> GetAllDepartamentosAsync();
        Task<IEnumerable<FormaPagoDto>> GetAllFormasPagoAsync();
    }
}