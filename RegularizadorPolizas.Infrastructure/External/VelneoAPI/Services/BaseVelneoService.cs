using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Extensions;  
using System.Diagnostics;
using System.Net;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services
{
    /// <summary>
    /// Clase base optimizada para todos los servicios especializados de Velneo API
    /// 
    /// ✅ ELIMINA DUPLICACIÓN: Métodos genéricos reutilizables
    /// ✅ LOGGING CONSISTENTE: Emojis y métricas estandardizadas  
    /// ✅ ERROR HANDLING: Manejo robusto de errores HTTP y de red
    /// ✅ PERFORMANCE: Tracking de tiempos y optimizaciones
    /// 
    /// 🎯 USADO POR:
    /// - VelneoClientService
    /// - VelneoCompanyService  
    /// - VelneoPolizaService
    /// - VelneoMaestrosService
    /// </summary>
    public abstract class BaseVelneoService
    {
        protected readonly IVelneoHttpService _httpService;
        protected readonly ITenantService _tenantService;
        protected readonly ILogger _logger;

        protected BaseVelneoService(
            IVelneoHttpService httpService,
            ITenantService tenantService,
            ILogger logger)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Método genérico optimizado para obtener maestros de Velneo API
        /// ✅ ELIMINA DUPLICACIÓN: Reemplaza 8+ métodos idénticos de GetAllXXXAsync()
        /// ✅ SOPORTE FALLBACK: Intenta wrapper primero, luego array directo
        /// ✅ OPTIMIZADO: Solo hace 1 llamada HTTP (no 2 como el código actual)
        /// </summary>
        /// <typeparam name="TVelneo">Tipo de entidad Velneo (ej: VelneoCombustible)</typeparam>
        /// <typeparam name="TResponse">Tipo de respuesta wrapper (ej: VelneoCombustiblesResponse)</typeparam>
        /// <typeparam name="TDto">Tipo de DTO final (ej: CombustibleDto)</typeparam>
        /// <param name="endpoint">Endpoint de la API (ej: "v1/combustibles")</param>
        /// <param name="entityName">Nombre para logging (ej: "combustibles")</param>
        /// <param name="extractFromWrapper">Función para extraer del wrapper</param>
        /// <param name="mapToDto">Función para mapear a DTO</param>
        /// <returns>Lista de DTOs</returns>
        protected async Task<IEnumerable<TDto>> GetMaestroAsync<TVelneo, TResponse, TDto>(
            string endpoint,
            string entityName,
            Func<TResponse, IEnumerable<TVelneo>?> extractFromWrapper,
            Func<IEnumerable<TVelneo>, IEnumerable<TDto>> mapToDto)
            where TResponse : class
            where TVelneo : class
            where TDto : class
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            _logger.LogDebug("📊 Getting all {EntityName} from Velneo API for tenant {TenantId}", entityName, tenantId);

            try
            {
                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync(endpoint);
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var entities = await _httpService.DeserializeWithFallbackAsync<TResponse, TVelneo>(
                    response, extractFromWrapper, entityName);

                if (entities.Any())
                {
                    var dtos = mapToDto(entities).ToList();
                    _logger.LogInformation("✅ Successfully retrieved {Count} {EntityName} from Velneo API",
                        dtos.Count, entityName);
                    return dtos;
                }

                _logger.LogWarning("⚠️ No {EntityName} found in Velneo API response", entityName);
                return new List<TDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting {EntityName} from Velneo API", entityName);
                throw new ApplicationException($"Error retrieving {entityName} from Velneo API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Método genérico para obtener una entidad por ID con soporte de fallback
        /// ✅ REEMPLAZA: Múltiples métodos GetXXXByIdAsync duplicados
        /// ✅ MANEJO 404: Retorna null en lugar de excepción cuando no existe
        /// </summary>
        protected async Task<TDto?> GetEntityByIdAsync<TVelneo, TResponse, TDto>(
            int id,
            string endpoint,
            string entityName,
            Func<TVelneo, TDto> mapToDto,
            Func<TResponse, TVelneo?> extractFromWrapper = null)
            where TVelneo : class
            where TResponse : class
            where TDto : class
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            _logger.LogDebug("🔍 Getting {EntityName} {Id} from Velneo API for tenant {TenantId}",
                entityName, id, tenantId);

            try
            {
                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"{endpoint}/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("🔍 {EntityName} {Id} not found in Velneo API", entityName, id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var directEntity = await _httpService.DeserializeResponseAsync<TVelneo>(response);
                if (directEntity != null)
                {
                    var result = mapToDto(directEntity);
                    _logger.LogInformation("✅ Retrieved {EntityName} {Id} from Velneo API", entityName, id);
                    return result;
                }

                if (extractFromWrapper != null)
                {
                    var wrapperResponse = await _httpService.DeserializeResponseAsync<TResponse>(response);
                    var wrappedEntity = extractFromWrapper(wrapperResponse);
                    if (wrappedEntity != null)
                    {
                        var result = mapToDto(wrappedEntity);
                        _logger.LogInformation("✅ Retrieved {EntityName} {Id} from Velneo API (wrapped)", entityName, id);
                        return result;
                    }
                }

                _logger.LogWarning("⚠️ {EntityName} {Id} not found or invalid format in Velneo API", entityName, id);
                return null;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning("🔍 {EntityName} {Id} not found in Velneo API", entityName, id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting {EntityName} {Id} from Velneo API", entityName, id);
                throw new ApplicationException($"Error retrieving {entityName} {id} from Velneo API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Método genérico para paginación con soporte completo de Velneo API
        /// ✅ TU IMPLEMENTACIÓN ACTUAL: Mantenida intacta con validación mejorada
        /// </summary>
        protected async Task<PaginatedVelneoResponse<TDto>> GetPaginatedAsync<TVelneo, TResponse, TDto>(
            string baseEndpoint,
            string entityName,
            int page = 1,
            int pageSize = 50,
            string? search = null,
            Func<TResponse, IEnumerable<TVelneo>?> extractFromWrapper = null,
            Func<TResponse, int?> extractTotalCount = null,
            Func<IEnumerable<TVelneo>, IEnumerable<TDto>> mapToDto = null,
            Func<TDto, string, bool> searchFilter = null)
            where TVelneo : class
            where TResponse : class
            where TDto : class
        {
            var stopwatch = Stopwatch.StartNew();
            var tenantId = _tenantService.GetCurrentTenantId();
            var (validPage, validPageSize) = ValidatePaginationParams(page, pageSize);

            _logger.LogInformation("📄 Getting {EntityName} page {Page} (size: {PageSize}) for tenant {TenantId}",
                entityName, validPage, validPageSize, tenantId);

            try
            {
                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();

                var endpoint = $"{baseEndpoint}?page[number]={validPage}&page[size]={validPageSize}";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    _logger.LogInformation("🔍 Search requested for {EntityName}: {Search}", entityName, search);
                }

                var url = await _httpService.BuildVelneoUrlAsync(endpoint);
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("📡 Velneo {EntityName} response - Page {Page}: Status {Status}, JSON length: {Length} chars",
                    entityName, validPage, response.StatusCode, jsonContent.Length);

                var itemsPage = new List<TDto>();
                var totalCount = 0;
                var hasMoreData = false;

                if (extractFromWrapper != null && mapToDto != null)
                {
                    var entities = await _httpService.DeserializeWithFallbackAsync<TResponse, TVelneo>(
                        response, extractFromWrapper, entityName);

                    if (entities.Any())
                    {
                        itemsPage = mapToDto(entities).ToList();

                        if (extractTotalCount != null)
                        {
                            var wrapperResponse = await _httpService.DeserializeResponseAsync<TResponse>(response);
                            totalCount = extractTotalCount(wrapperResponse) ?? EstimateTotalCount(itemsPage.Count, validPage, validPageSize);
                        }
                        else
                        {
                            totalCount = response.GetTotalCountFromHeaders() ?? EstimateTotalCount(itemsPage.Count, validPage, validPageSize);
                        }

                        hasMoreData = itemsPage.Count == validPageSize && validPage * validPageSize < totalCount;
                    }
                }

                stopwatch.Stop();

                if (!string.IsNullOrWhiteSpace(search) && searchFilter != null)
                {
                    var originalCount = itemsPage.Count;
                    itemsPage = itemsPage.Where(item => searchFilter(item, search)).ToList();

                    _logger.LogInformation("🔍 Search filter applied to {EntityName}: {FilteredCount} of {OriginalCount}",
                        entityName, itemsPage.Count, originalCount);
                }

                var result = new PaginatedVelneoResponse<TDto>
                {
                    Items = itemsPage,
                    TotalCount = totalCount,
                    PageNumber = validPage,
                    PageSize = validPageSize,
                    VelneoHasMoreData = hasMoreData,
                    RequestDuration = stopwatch.Elapsed
                };

                _logger.LogInformation("✅ Paginación {EntityName} completada: Page {Page}/{TotalPages} - {Count} items in {Duration}ms",
                    entityName, validPage, result.TotalPages, itemsPage.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ Error en paginación {EntityName} - Page: {Page}, PageSize: {PageSize}, Duration: {Duration}ms",
                    entityName, validPage, validPageSize, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        // ===========================
        // 🆕 MÉTODOS ADICIONALES ÚTILES
        // ===========================

        /// <summary>
        /// Método genérico para búsqueda simple en colecciones
        /// ✅ REEMPLAZA: Métodos SearchXXXAsync duplicados
        /// ✅ FILTRO LOCAL: Aplica filtro después de obtener datos (útil cuando Velneo no tiene search)
        /// </summary>
        protected async Task<IEnumerable<TDto>> SearchAsync<TDto>(
            Func<Task<IEnumerable<TDto>>> getAllMethod,
            string searchTerm,
            Func<TDto, string, bool> searchFilter,
            string entityName)
            where TDto : class
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await getAllMethod();
            }

            try
            {
                _logger.LogDebug("🔍 Searching {EntityName} with term: {SearchTerm}", entityName, searchTerm);

                var allItems = await getAllMethod();
                var filteredItems = allItems.Where(item => searchFilter(item, searchTerm)).ToList();

                _logger.LogInformation("✅ Search completed for {EntityName}: {FilteredCount} of {TotalCount} items match '{SearchTerm}'",
                    entityName, filteredItems.Count, allItems.Count(), searchTerm);

                return filteredItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error searching {EntityName} with term: {SearchTerm}", entityName, searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Método genérico para crear lookups optimizados (dropdowns)
        /// ✅ PERFORMANCE: Solo devuelve campos mínimos necesarios para UI
        /// </summary>
        protected async Task<IEnumerable<TLookup>> GetLookupAsync<TDto, TLookup>(
            Func<Task<IEnumerable<TDto>>> getAllMethod,
            Func<TDto, TLookup> mapToLookup,
            string entityName)
            where TDto : class
            where TLookup : class
        {
            try
            {
                _logger.LogDebug("📋 Getting {EntityName} lookup data", entityName);

                var allItems = await getAllMethod();
                var lookupItems = allItems.Select(mapToLookup).ToList();

                _logger.LogInformation("✅ Generated {Count} {EntityName} lookup items", lookupItems.Count, entityName);
                return lookupItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting {EntityName} lookup data", entityName);
                throw;
            }
        }

        // ===========================
        // 🛠 MÉTODOS DE UTILIDAD
        // ===========================

        /// <summary>
        /// Estima el total count cuando no está disponible en headers o response
        /// </summary>
        protected static int EstimateTotalCount(int currentPageCount, int currentPage, int pageSize)
        {
            if (currentPageCount < pageSize)
            {
                return ((currentPage - 1) * pageSize) + currentPageCount;
            }

            return currentPage * pageSize + 1;
        }

        /// <summary>
        /// Método helper para ejecutar operaciones con logging completo y manejo de errores
        /// ✅ MEJORADO: Ahora incluye métricas de tiempo
        /// </summary>
        protected async Task<T> ExecuteWithLoggingAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            object? parameters = null)
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            var parametersStr = parameters != null ? $" with {parameters}" : "";
            var stopwatch = Stopwatch.StartNew();

            _logger.LogDebug("🚀 Starting {OperationName}{Parameters} for tenant {TenantId}",
                operationName, parametersStr, tenantId);

            try
            {
                var result = await operation();
                stopwatch.Stop();
                _logger.LogInformation("✅ Completed {OperationName} successfully in {Duration}ms",
                    operationName, stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ Error in {OperationName}{Parameters} after {Duration}ms",
                    operationName, parametersStr, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Valida y normaliza parámetros de paginación
        /// ✅ SEGURIDAD: Previene ataques de memoria/performance
        /// </summary>
        protected static (int validPage, int validPageSize) ValidatePaginationParams(int page, int pageSize, int maxPageSize = 1000)
        {
            var validPage = Math.Max(1, page);
            var validPageSize = Math.Min(Math.Max(1, pageSize), maxPageSize);

            return (validPage, validPageSize);
        }

        /// <summary>
        /// Construye filtros de búsqueda case-insensitive para texto
        /// ✅ UTILIDAD: Función común para búsquedas en servicios especializados
        /// </summary>
        protected static bool SearchTextFields(string searchTerm, params string?[] fields)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return true;

            var normalizedSearch = searchTerm.Trim().ToLowerInvariant();

            return fields.Any(field =>
                !string.IsNullOrWhiteSpace(field) &&
                field.ToLowerInvariant().Contains(normalizedSearch));
        }
    }
}