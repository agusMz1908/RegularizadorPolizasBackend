// RegularizadorPolizas.Application/Interfaces/External/Velneo/IVelneoMaestrosService.cs
using RegularizadorPolizas.Application.DTOs;

public interface IVelneoMaestrosService
{
    // ===========================
    // MAESTROS BÁSICOS DE VEHÍCULOS
    // ===========================
    Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync();
    Task<IEnumerable<DestinoDto>> GetAllDestinosAsync();
    Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
    Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync();
    Task<IEnumerable<MonedaDto>> GetAllMonedasAsync();

    // ===========================
    // MAESTROS DE SEGUROS
    // ===========================
    Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
    Task<SeccionDto> GetSeccionAsync(int id);
    Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();
    Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId);
    Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
    Task<IEnumerable<CoberturaDto>> GetAllCoberturasAsync();
    Task<IEnumerable<DepartamentoDto>> GetAllDepartamentosAsync();
    Task<IEnumerable<TarifaDto>> GetAllTarifasAsync();
}