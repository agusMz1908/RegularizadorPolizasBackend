using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Models;
using RegularizadorPolizas.Application.Mappers;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services
{
    /// <summary>
    /// Servicio especializado para maestros/catálogos en Velneo API
    /// 
    /// ✅ ELIMINA DUPLICACIÓN: Reemplaza 8+ métodos idénticos del monolito
    /// ✅ USA BaseVelneoService: Código súper limpio sin repetición
    /// ✅ PERFORMANCE: Método genérico optimizado GetMaestroAsync<T>()
    /// 
    /// 📋 MAESTROS SOPORTADOS:
    /// - Combustibles, Destinos, Categorías, Calidades, Monedas
    /// - Secciones, Coberturas, Departamentos
    /// 
    /// 🔄 MIGRADO DESDE: TenantAwareVelneoApiService.cs (líneas 500-1200)
    /// </summary>
    public class VelneoMaestrosService : BaseVelneoService, IVelneoMaestrosService
    {
        public VelneoMaestrosService(
            IVelneoHttpService httpService,
            ITenantService tenantService,
            ILogger<VelneoMaestrosService> logger)
            : base(httpService, tenantService, logger)
        {
        }

        // ===========================
        // MAESTROS BÁSICOS DE VEHÍCULOS
        // ===========================

        /// <summary>
        /// ✅ ANTES: 25 líneas duplicadas en TenantAwareVelneoApiService
        /// ✅ AHORA: 1 línea usando BaseVelneoService.GetMaestroAsync<T>()
        /// </summary>
        public async Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync()
        {
            return await GetMaestroAsync<VelneoCombustible, VelneoCombustiblesResponse, CombustibleDto>(
                endpoint: "v1/combustibles",
                entityName: "combustibles",
                extractFromWrapper: response => response.Combustibles,
                mapToDto: combustibles => combustibles.ToCombustibleDtos()
            );
        }

        /// <summary>
        /// ✅ ANTES: 25 líneas duplicadas en TenantAwareVelneoApiService  
        /// ✅ AHORA: 1 línea usando BaseVelneoService.GetMaestroAsync<T>()
        /// </summary>
        public async Task<IEnumerable<DestinoDto>> GetAllDestinosAsync()
        {
            return await GetMaestroAsync<VelneoDestino, VelneoDestinosResponse, DestinoDto>(
                endpoint: "v1/destinos",
                entityName: "destinos",
                extractFromWrapper: response => response.Destinos,
                mapToDto: destinos => destinos.ToDestinoDtos()
            );
        }

        /// <summary>
        /// ✅ ANTES: 25 líneas duplicadas en TenantAwareVelneoApiService
        /// ✅ AHORA: 1 línea usando BaseVelneoService.GetMaestroAsync<T>()
        /// </summary>
        public async Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync()
        {
            return await GetMaestroAsync<VelneoCategoria, VelneoCategoriasResponse, CategoriaDto>(
                endpoint: "v1/categorias",
                entityName: "categorias",
                extractFromWrapper: response => response.Categorias,
                mapToDto: categorias => categorias.ToCategoriaDtos()
            );
        }

        /// <summary>
        /// ✅ ANTES: 25 líneas duplicadas en TenantAwareVelneoApiService
        /// ✅ AHORA: 1 línea usando BaseVelneoService.GetMaestroAsync<T>()
        /// </summary>
        public async Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync()
        {
            return await GetMaestroAsync<VelneoCalidad, VelneoCalidadesResponse, CalidadDto>(
                endpoint: "v1/calidades",
                entityName: "calidades",
                extractFromWrapper: response => response.Calidades,
                mapToDto: calidades => calidades.ToCalidadDtos()
            );
        }

        /// <summary>
        /// ✅ ANTES: 25 líneas duplicadas en TenantAwareVelneoApiService
        /// ✅ AHORA: 1 línea usando BaseVelneoService.GetMaestroAsync<T>()
        /// </summary>
        public async Task<IEnumerable<MonedaDto>> GetAllMonedasAsync()
        {
            return await GetMaestroAsync<VelneoMoneda, VelneeMonedasResponse, MonedaDto>(
                endpoint: "v1/monedas",
                entityName: "monedas",
                extractFromWrapper: response => response.Monedas,
                mapToDto: monedas => monedas.ToMonedaDtos()
            );
        }

        // ===========================
        // MAESTROS DE SEGUROS
        // ===========================

        /// <summary>
        /// ✅ SECCIONES: Implementación completa usando BaseVelneoService
        /// </summary>
        public async Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync()
        {
            return await GetMaestroAsync<VelneoSeccion, VelneoSeccionesResponse, SeccionDto>(
                endpoint: "v1/secciones",
                entityName: "secciones",
                extractFromWrapper: response => response.Secciones?.Where(s => s.Activo),
                mapToDto: secciones => secciones.ToSeccionDtos()
            );
        }

        public async Task<SeccionDto?> GetSeccionAsync(int id)
        {
            return await GetEntityByIdAsync<VelneoSeccion, VelneoSeccionResponse, SeccionDto>(
                id: id,
                endpoint: "v1/secciones",
                entityName: "seccion",
                mapToDto: seccion => seccion.ToSeccionDto()
                // extractFromWrapper: response => response.Seccion // Si hay wrapper
            );
        }

        public async Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId)
        {
            // ✅ NOTA: SeccionDto actual no tiene CompanyId, retornar todas las activas
            _logger.LogWarning("⚠️ GetSeccionesByCompanyAsync - SeccionDto doesn't have CompanyId, returning all active secciones for company {CompanyId}", companyId);
            return await GetActiveSeccionesAsync();
        }

        public async Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm)
        {
            return await SearchAsync(
                getAllMethod: GetActiveSeccionesAsync,
                searchTerm: searchTerm,
                searchFilter: (seccion, term) => SearchTextFields(term, seccion.Seccion, seccion.Icono),
                entityName: "secciones"
            );
        }

        public async Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync()
        {
            return await GetLookupAsync(
                getAllMethod: GetActiveSeccionesAsync,
                mapToLookup: seccion => seccion.ToSeccionLookupDto(),
                entityName: "secciones lookup"
            );
        }

        /// <summary>
        /// ✅ COBERTURAS: Usando el mapper que ya tienes en VelneoMappingExtensions
        /// </summary>
        public async Task<IEnumerable<CoberturaDto>> GetAllCoberturasAsync()
        {
            return await GetMaestroAsync<VelneoCobertura, VelneoCoberturasResponse, CoberturaDto>(
                endpoint: "v1/coberturas",
                entityName: "coberturas",
                extractFromWrapper: response => response.Coberturas,
                mapToDto: coberturas => coberturas.ToCoberturasDtos()
            );
        }

        /// <summary>
        /// ✅ DEPARTAMENTOS: Usando el mapper que ya tienes en VelneoMappingExtensions
        /// </summary>
        public async Task<IEnumerable<DepartamentoDto>> GetAllDepartamentosAsync()
        {
            return await GetMaestroAsync<VelneoDepartamento, VelneoDepartamentosResponse, DepartamentoDto>(
                endpoint: "v1/departamentos",
                entityName: "departamentos",
                extractFromWrapper: response => response.Departamentos,
                mapToDto: departamentos => departamentos.ToDepartamentosDtos()
            );
        }
    }
}