using RegularizadorPolizas.Application.DTOs;

public interface IVelneoMaestrosService
{
    Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
    Task<IEnumerable<DestinoDto>> GetAllDestinosAsync();
    Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync();
    Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync();
    Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
    Task<SeccionDto> GetSeccionAsync(int id);
    Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();
    Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId);
    Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
    Task<IEnumerable<MonedaDto>> GetAllMonedasAsync();
    Task<IEnumerable<CoberturaDto>> GetAllCoberturasAsync();
    Task<IEnumerable<DepartamentoDto>> GetAllDepartamentosAsync();
}