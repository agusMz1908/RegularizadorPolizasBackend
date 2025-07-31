using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Application.Mappers;
using RegularizadorPolizas.Application.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Linq;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services
{
    public class VelneoMaestrosService : BaseVelneoService, IVelneoMaestrosService
    {
        private IHttpClientFactory _httpClientFactory;

        public VelneoMaestrosService(
                IVelneoHttpService httpService,
                ITenantService tenantService,
                ILogger<VelneoMaestrosService> logger,
                IHttpClientFactory httpClientFactory) 
                : base(httpService, tenantService, logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }
        

        public async Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync()
        {
            return await GetMaestroAsync<VelneoCombustible, VelneoCombustiblesResponse, CombustibleDto>(
                endpoint: "v1/combustibles",
                entityName: "combustibles",
                extractFromWrapper: response => response.Combustibles,
                mapToDto: combustibles => combustibles.ToCombustibleDtos()
            );
        }

        public async Task<IEnumerable<DestinoDto>> GetAllDestinosAsync()
        {
            return await GetMaestroAsync<VelneoDestino, VelneoDestinosResponse, DestinoDto>(
                endpoint: "v1/destinos",
                entityName: "destinos",
                extractFromWrapper: response => response.Destinos,
                mapToDto: destinos => destinos.ToDestinoDtos()
            );
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync()
        {
            return await GetMaestroAsync<VelneoCategoria, VelneoCategoriasResponse, CategoriaDto>(
                endpoint: "v1/categorias",
                entityName: "categorias",
                extractFromWrapper: response => response.Categorias,
                mapToDto: categorias => categorias.ToCategoriaDtos()
            );
        }

        public async Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync()
        {
            return await GetMaestroAsync<VelneoCalidad, VelneoCalidadesResponse, CalidadDto>(
                endpoint: "v1/calidades",
                entityName: "calidades",
                extractFromWrapper: response => response.Calidades,
                mapToDto: calidades => calidades.ToCalidadDtos()
            );
        }

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
        // MAESTROS DE SEGUROS (YA EXISTÍAN)
        // ===========================

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
            );
        }

        public async Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId)
        {
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
            var secciones = await GetActiveSeccionesAsync();

            return secciones.Select(seccion => new SeccionLookupDto
            {
                Id = seccion.Id,
                Name = seccion.Seccion,
                Icono = seccion.Icono,
                Activo = seccion.Activo
            });
        }

        public async Task<IEnumerable<CoberturaDto>> GetAllCoberturasAsync()
        {
            return await GetMaestroAsync<VelneoCobertura, VelneoCoberturasResponse, CoberturaDto>(
                endpoint: "v1/coberturas",
                entityName: "coberturas",
                extractFromWrapper: response => response.Coberturas,
                mapToDto: coberturas => coberturas.ToCoberturasDtos()
            );
        }

        public async Task<IEnumerable<DepartamentoDto>> GetAllDepartamentosAsync()
        {
            return await GetMaestroAsync<VelneoDepartamento, VelneoDepartamentosResponse, DepartamentoDto>(
                endpoint: "v1/departamentos",
                entityName: "departamentos",
                extractFromWrapper: response => response.Departamentos,
                mapToDto: departamentos => departamentos.ToDepartamentosDtos()
            );
        }

        public async Task<IEnumerable<TarifaDto>> GetAllTarifasAsync()
        {
            return await GetMaestroAsync<VelneoTarifa, VelneoTarifasResponse, TarifaDto>(
                endpoint: "v1/tarifas",
                entityName: "tarifas",
                extractFromWrapper: response => response.Tarifas,
                mapToDto: tarifas => tarifas.ToTarifaDtos()
            );
        }

        public async Task<ClientDto> GetClienteAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting cliente {ClienteId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/clientes/{id}");

                _logger.LogInformation("🌐 Requesting URL: {Url}", url);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ STRATEGY 1: Try direct VelneoCliente deserialization
                var velneoCliente = await _httpService.DeserializeResponseAsync<VelneoCliente>(response);
                if (velneoCliente != null && velneoCliente.Id > 0)
                {
                    var result = velneoCliente.ToClienteDto();
                    _logger.LogInformation("✅ Successfully retrieved cliente {ClienteId} (direct format)", id);
                    return result;
                }

                // ✅ STRATEGY 2: Try VelneoClienteResponse wrapper {"cliente": {...}}
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClienteResponse>(response);
                if (velneoResponse?.Cliente != null && velneoResponse.Cliente.Id > 0)
                {
                    var result = velneoResponse.Cliente.ToClienteDto();
                    _logger.LogInformation("✅ Successfully retrieved cliente {ClienteId} (wrapper format)", id);
                    return result;
                }

                // ✅ STRATEGY 3: Try VelneoClientesResponse array {"clientes": [...]} - FIX PRINCIPAL
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoArrayResponse = await _httpService.DeserializeResponseAsync<VelneoClientesResponse>(response);
                if (velneoArrayResponse?.Clientes != null && velneoArrayResponse.Clientes.Any())
                {
                    var cliente = velneoArrayResponse.Clientes.FirstOrDefault(c => c.Id == id);
                    if (cliente != null && cliente.Id > 0)
                    {
                        var result = cliente.ToClienteDto();
                        _logger.LogInformation("✅ Successfully retrieved cliente {ClienteId} (array format) - Name: {Name}",
                            id, result.Clinom);
                        return result;
                    }
                }

                throw new KeyNotFoundException($"Cliente with ID {id} not found in Velneo API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting cliente {ClienteId} from Velneo API", id);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoClientService.GetClientesAsync()
        /// Paginación automática hasta obtener todos los clientes
        /// </summary>
        public async Task<IEnumerable<ClientDto>> GetClientesAsync()
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            _logger.LogInformation("👥 Getting ALL clientes from Velneo API for tenant {TenantId} - EXTENDED TIMEOUT", tenantId);

            var allClientes = new List<ClientDto>();
            var pageNumber = 1;
            var pageSize = 500; // ✅ PÁGINAS MÁS GRANDES PARA REDUCIR NÚMERO DE LLAMADAS
            var hasMoreData = true;

            while (hasMoreData)
            {
                _logger.LogDebug("Obteniendo página {Page}...", pageNumber);

                try
                {
                    // ✅ USAR HttpClient CON TIMEOUT EXTENDIDO PARA CLIENTES
                    using var httpClient = await GetConfiguredHttpClientForClientsAsync();
                    var url = await _httpService.BuildVelneoUrlAsync($"v1/clientes?page={pageNumber}&limit={pageSize}");
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClientesResponse>(response);

                    if (velneoResponse?.Clientes != null && velneoResponse.Clientes.Any())
                    {
                        var clientesPage = velneoResponse.Clientes.ToClienteDtos().ToList();
                        allClientes.AddRange(clientesPage);

                        _logger.LogDebug("✅ Página {Page}: {Count} clientes obtenidos (Total acumulado: {Total})",
                            pageNumber, clientesPage.Count, allClientes.Count);

                        hasMoreData = velneoResponse.HasMoreData == true;
                        pageNumber++;
                    }
                    else
                    {
                        hasMoreData = false;
                        _logger.LogDebug("No hay más datos en página {Page}", pageNumber);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error obteniendo página {Page} de clientes", pageNumber);
                    throw;
                }
            }

            _logger.LogInformation("✅ Successfully retrieved {Count} clientes total from Velneo API", allClientes.Count);
            return allClientes;
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoClientService.GetClientesPaginatedAsync()
        /// Paginación manual con control específico de página
        /// </summary>
        public async Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(
    int page = 1,
    int pageSize = 50,
    string? search = null)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("👥 Getting clientes page {Page} (size {PageSize}) for tenant {TenantId} - EXTENDED TIMEOUT",
                    page, pageSize, tenantId);

                // ✅ USAR HttpClient CON TIMEOUT EXTENDIDO PARA CLIENTES
                using var httpClient = await GetConfiguredHttpClientForClientsAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/clientes?page={page}&limit={pageSize}");

                _logger.LogInformation("📤 Client request URL: {Url} (timeout: {Timeout}s)",
                    url.Replace("api_key=", "api_key=***"), httpClient.Timeout.TotalSeconds);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClientesResponse>(response);

                if (velneoResponse?.Clientes != null)
                {
                    var clientsPage = velneoResponse.Clientes.ToClienteDtos().ToList();
                    var totalCount = velneoResponse.Total_Count > 0 ? velneoResponse.Total_Count :
                                   EstimateTotalCount(clientsPage.Count, page, pageSize);

                    var result = new PaginatedVelneoResponse<ClientDto>
                    {
                        Items = clientsPage,
                        TotalCount = totalCount,
                        PageNumber = page,
                        PageSize = pageSize,
                        VelneoHasMoreData = velneoResponse.HasMoreData ?? (clientsPage.Count >= pageSize),
                        RequestDuration = stopwatch.Elapsed
                    };

                    stopwatch.Stop();
                    _logger.LogInformation("✅ CLIENTS PAGINATION SUCCESS: Page {Page}/{EstimatedTotal} - {Count} clients in {Duration}ms",
                        page, result.TotalPages, clientsPage.Count, stopwatch.ElapsedMilliseconds);

                    return result;
                }

                throw new InvalidOperationException("No valid response from Velneo API for clients pagination");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                stopwatch.Stop();
                _logger.LogError("🚨 TIMEOUT en GetClientesPaginatedAsync después de {Duration}ms - Page: {Page}, PageSize: {PageSize}",
                    stopwatch.ElapsedMilliseconds, page, pageSize);

                // ✅ THROW CUSTOM EXCEPTION CON INFORMACIÓN ÚTIL
                throw new TimeoutException($"Velneo API timeout después de {stopwatch.ElapsedMilliseconds}ms para página {page}. " +
                    "Los clientes pueden tardar más en cargar. Intenta reducir el pageSize o usar el endpoint /api/clientes/all", ex);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ Error en GetClientesPaginatedAsync - Page: {Page}, PageSize: {PageSize}, Duration: {Duration}ms",
                    page, pageSize, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoClientService.SearchClientesAsync()
        /// Búsqueda en memoria sobre todos los clientes
        /// </summary>
        public async Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ClientDto>();

            try
            {
                _logger.LogInformation("🔍 Searching clientes with term '{SearchTerm}'", searchTerm);

                var allClientes = await GetClientesAsync();

                var filtered = allClientes.Where(c =>
                    c.Clinom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Cliced?.Contains(searchTerm) == true ||
                    c.Cliruc?.Contains(searchTerm) == true ||
                    c.Cliemail?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                _logger.LogInformation("Found {Count} clientes matching '{SearchTerm}'", filtered.Count, searchTerm);
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clientes with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoClientService.SearchClientesDirectAsync()
        /// Búsqueda directa en API de Velneo (más rápida)
        /// </summary>
        public async Task<IEnumerable<ClientDto>> SearchClientesDirectAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ClientDto>();

            try
            {
                _logger.LogInformation("🔍 BÚSQUEDA DIRECTA VELNEO: {SearchTerm}", searchTerm);

                var endpoint = $"v1/clientes?filter[nombre]={Uri.EscapeDataString(searchTerm)}";

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var url = await _httpService.BuildVelneoUrlAsync(endpoint);
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var clients = await ParseDirectSearchResponse(response, searchTerm);

                _logger.LogInformation("✅ BÚSQUEDA DIRECTA EXITOSA: {Count} clientes encontrados", clients.Count);
                return clients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda directa de clientes: {SearchTerm}", searchTerm);
                return new List<ClientDto>();
            }
        }

        // ===========================
        // MAESTROS DE COMPAÑÍAS 
        // ✅ MIGRADO DESDE: VelneoCompanyService (DÍA 3)
        // ===========================

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.GetAllCompaniesAsync()
        /// Obtiene todas las compañías con fallback wrapper/array
        /// </summary>
        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all companies from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync("v1/companias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (formato esperado de Velneo)
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoCompaniesResponse>(response);
                if (velneoResponse?.Companias != null && velneoResponse.Companias.Any())
                {
                    var companies = velneoResponse.Companias.ToCompanyDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (wrapper format)", companies.Count);
                    return companies;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoCompanies = await _httpService.DeserializeResponseAsync<List<VelneoCompany>>(response);
                if (velneoCompanies != null && velneoCompanies.Any())
                {
                    var companies = velneoCompanies.ToCompanyDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (array format)", companies.Count);
                    return companies;
                }

                _logger.LogWarning("No companies found in Velneo API response");
                return new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies from Velneo API");
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.GetActiveCompaniesAsync()
        /// Por ahora retorna todas las compañías (lógica original)
        /// </summary>
        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            return await GetAllCompaniesAsync();
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.GetCompanyByIdAsync()
        /// Obtiene compañía específica con manejo de 404
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting company {CompanyId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/companias/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Company {CompanyId} not found in Velneo API", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                // ✅ INTENTAR PRIMERO COMO OBJETO DIRECTO
                var velneoCompany = await _httpService.DeserializeResponseAsync<VelneoCompany>(response);
                if (velneoCompany != null)
                {
                    var result = velneoCompany.ToCompanyDto();
                    _logger.LogInformation("Successfully retrieved company {CompanyId} from Velneo API", id);
                    return result;
                }

                // ✅ SI FALLA, INTENTAR COMO WRAPPER - hacer nueva llamada
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoCompanyResponse>(response);
                if (velneoResponse?.Company != null)
                {
                    var result = velneoResponse.Company.ToCompanyDto();
                    _logger.LogInformation("Successfully retrieved company {CompanyId} from Velneo API (wrapped)", id);
                    return result;
                }

                _logger.LogWarning("Company {CompanyId} not found or invalid format in Velneo API", id);
                return null;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning("Company {CompanyId} not found in Velneo API", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {CompanyId} from Velneo API", id);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.GetCompanyByAliasAsync()
        /// Búsqueda en memoria por alias
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    _logger.LogWarning("GetCompanyByAliasAsync called with empty alias");
                    return null;
                }

                _logger.LogDebug("Getting company by alias '{Alias}' from Velneo API", alias);

                var companies = await GetAllCompaniesAsync();
                var company = companies.FirstOrDefault(c =>
                    string.Equals(c.Comalias, alias, StringComparison.OrdinalIgnoreCase));

                if (company != null)
                {
                    _logger.LogInformation("Successfully found company by alias '{Alias}'", alias);
                }
                else
                {
                    _logger.LogWarning("Company with alias '{Alias}' not found", alias);
                }

                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by alias '{Alias}' from Velneo API", alias);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.GetCompanyByCodigoAsync()
        /// Endpoint directo + fallback a búsqueda en memoria
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    _logger.LogWarning("GetCompanyByCodigoAsync called with empty codigo");
                    return null;
                }

                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting company by codigo '{Codigo}' from Velneo API for tenant {TenantId}", codigo, tenantId);

                // ✅ OPCIÓN 1: Si Velneo tiene endpoint específico para búsqueda por código
                try
                {
                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                    var url = await _httpService.BuildVelneoUrlAsync($"v1/companias/codigo/{Uri.EscapeDataString(codigo)}");
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var velneoCompany = await _httpService.DeserializeResponseAsync<VelneoCompany>(response);
                        if (velneoCompany != null)
                        {
                            var result = velneoCompany.ToCompanyDto();
                            _logger.LogInformation("Successfully retrieved company by codigo '{Codigo}' from Velneo API", codigo);
                            return result;
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    // Continuar con búsqueda en la lista completa
                    _logger.LogDebug("Direct endpoint not found, searching in full list for codigo '{Codigo}'", codigo);
                }

                // ✅ OPCIÓN 2: FALLBACK - Buscar en la lista completa de compañías
                var companies = await GetAllCompaniesAsync();
                var company = companies.FirstOrDefault(c =>
                    string.Equals(c.Cod_srvcompanias, codigo, StringComparison.OrdinalIgnoreCase));

                if (company != null)
                {
                    _logger.LogInformation("Successfully found company by codigo '{Codigo}' in full list", codigo);
                    return company;
                }

                _logger.LogWarning("Company with codigo '{Codigo}' not found in Velneo API", codigo);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by codigo '{Codigo}' from Velneo API", codigo);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.GetCompaniesForLookupAsync()
        /// Conversión a DTOs de lookup para dropdowns
        /// </summary>
        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                _logger.LogDebug("Getting companies for lookup from Velneo API");

                var companies = await GetAllCompaniesAsync();
                var lookupDtos = companies.Select(c => new CompanyLookupDto
                {
                    Id = c.Id,
                    Comnom = c.Comnom,
                    Comalias = c.Comalias,
                    Cod_srvcompanias = c.Cod_srvcompanias
                }).ToList();

                _logger.LogInformation("Successfully retrieved {Count} companies for lookup", lookupDtos.Count);
                return lookupDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies for lookup from Velneo API");
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoCompanyService.SearchCompaniesAsync()
        /// Búsqueda en memoria por múltiples campos
        /// </summary>
        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogWarning("SearchCompaniesAsync called with empty search term");
                    return new List<CompanyDto>();
                }

                _logger.LogDebug("Searching companies with term '{SearchTerm}'", searchTerm);

                var allCompanies = await GetAllCompaniesAsync();
                var filtered = allCompanies.Where(c =>
                    c.Comnom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Comalias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Cod_srvcompanias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                _logger.LogInformation("Found {Count} companies matching '{SearchTerm}'", filtered.Count, searchTerm);
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching companies with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        // <summary>
        /// ✅ MIGRADO DESDE: TenantAwareVelneoApiService.GetPolizaAsync()
        /// Obtiene una póliza específica por ID
        /// </summary>
        public async Task<PolizaDto> GetPolizaAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting póliza {PolizaId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                httpClient.Timeout = TimeSpan.FromMinutes(5);

                var url = await _httpService.BuildVelneoUrlAsync($"v1/contratos/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("🚫 Póliza {PolizaId} not found in Velneo API", id);
                    throw new KeyNotFoundException($"Póliza with ID {id} not found in Velneo API");
                }

                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("📄 Contrato response length: {Length} chars", jsonContent.Length);

                // ✅ DESERIALIZACIÓN COMPLETA IMPLEMENTADA
                var velneoPoliza = await DeserializePolizaResponse(jsonContent, id);

                if (velneoPoliza != null)
                {
                    var result = velneoPoliza.ToPolizaDto();
                    _logger.LogInformation("✅ Successfully retrieved póliza {PolizaId} from Velneo API", id);
                    return result;
                }

                throw new ApplicationException($"Unable to deserialize póliza {id} from Velneo API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR getting póliza {PolizaId} from Velneo API", id);
                throw;
            }
        }

        // <summary>
        /// Helper para deserializar respuestas de póliza con manejo robusto
        /// </summary>
        private async Task<VelneoPoliza?> DeserializePolizaResponse(string jsonContent, int polizaId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("⚠️ Empty JSON response for póliza {PolizaId}", polizaId);
                    return null;
                }

                var jsonOptions = GetJsonOptions();

                // ✅ OPCIÓN 1: INTENTAR COMO OBJETO DIRECTO PRIMERO
                try
                {
                    var directPoliza = System.Text.Json.JsonSerializer.Deserialize<VelneoPoliza>(jsonContent, jsonOptions);
                    if (directPoliza != null && directPoliza.Id > 0)
                    {
                        _logger.LogDebug("✅ Póliza {PolizaId} deserializada directamente", polizaId);
                        return directPoliza;
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogDebug("⚠️ No se pudo deserializar como objeto directo: {Error}", ex.Message);
                }

                // ✅ OPCIÓN 2: INTENTAR COMO WRAPPER - Probar múltiples propiedades
                try
                {
                    var wrapperResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizaResponse>(jsonContent, jsonOptions);

                    // Probar propiedad "poliza"
                    if (wrapperResponse?.Poliza != null && wrapperResponse.Poliza.Id > 0)
                    {
                        _logger.LogDebug("✅ Póliza {PolizaId} deserializada desde wrapper.Poliza", polizaId);
                        return wrapperResponse.Poliza;
                    }

                    // Probar propiedad "contrato" (alternativa común en Velneo)
                    if (wrapperResponse?.Contrato != null && wrapperResponse.Contrato.Id > 0)
                    {
                        _logger.LogDebug("✅ Póliza {PolizaId} deserializada desde wrapper.Contrato", polizaId);
                        return wrapperResponse.Contrato;
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogDebug("⚠️ No se pudo deserializar como wrapper: {Error}", ex.Message);
                }

                // ✅ OPCIÓN 3: LOG PARA DEBUG Y RETORNAR NULL
                _logger.LogWarning("⚠️ No se pudo deserializar póliza {PolizaId}. JSON preview: {JsonPreview}",
                    polizaId, jsonContent.Substring(0, Math.Min(200, jsonContent.Length)));

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado deserializando póliza {PolizaId}", polizaId);
                return null;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: TenantAwareVelneoApiService.GetPolizasAsync()
        /// Obtiene todas las pólizas con paginación automática
        /// </summary>
        public async Task<IEnumerable<PolizaDto>> GetPolizasAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting ALL pólizas from Velneo API for tenant {TenantId}", tenantId);

                var allPolizas = new List<PolizaDto>();
                var pageNumber = 1;
                var pageSize = 1000;
                var hasMoreData = true;

                while (hasMoreData)
                {
                    _logger.LogDebug("Obteniendo página {Page} de pólizas...", pageNumber);

                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                    httpClient.Timeout = TimeSpan.FromMinutes(5);

                    var url = await _httpService.BuildVelneoUrlAsync($"v1/contratos?page[number]={pageNumber}&page[size]={pageSize}");
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();

                    // ✅ USAR EL MODELO CORRECTO VelneoPolizasResponse
                    var velneoResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, GetJsonOptions());

                    // ✅ VERIFICAR AMBAS PROPIEDADES: Polizas Y Contratos
                    var polizasFromResponse = velneoResponse?.Polizas?.Any() == true
                        ? velneoResponse.Polizas
                        : velneoResponse?.Contratos ?? new List<VelneoPoliza>();

                    if (polizasFromResponse.Any())
                    {
                        var polizasPage = polizasFromResponse.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        _logger.LogDebug("✅ Página {Page}: {Count} pólizas obtenidas (Total acumulado: {Total})",
                            pageNumber, polizasPage.Count, allPolizas.Count);

                        // ✅ VERIFICAR SI HAY MÁS DATOS
                        hasMoreData = polizasFromResponse.Count == pageSize &&
                                     (velneoResponse?.HasMoreData == true || allPolizas.Count < (velneoResponse?.Total_Count ?? 0));

                        if (hasMoreData)
                        {
                            pageNumber++;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Página {Page} vacía. Finalizando paginación.", pageNumber);
                        hasMoreData = false;
                    }
                }

                _logger.LogInformation("✅ COMPLETADO: {TotalRetrieved} pólizas obtenidas en total", allPolizas.Count);
                return allPolizas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR en GetPolizasAsync: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: TenantAwareVelneoApiService.GetPolizasByClientAsync()
        /// Obtiene pólizas por cliente específico
        /// </summary>
        public async Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting pólizas for client {ClienteId} for tenant {TenantId}", clienteId, tenantId);

                var allPolizas = new List<PolizaDto>();
                var pageNumber = 1;
                var pageSize = 1000;
                var hasMoreData = true;

                while (hasMoreData)
                {
                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                    httpClient.Timeout = TimeSpan.FromMinutes(5);

                    var url = await _httpService.BuildVelneoUrlAsync($"v1/contratos?filter[clientes]={clienteId}&page[number]={pageNumber}&page[size]={pageSize}");
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var velneoResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, GetJsonOptions());

                    // ✅ VERIFICAR AMBAS PROPIEDADES: Polizas Y Contratos
                    var polizasFromResponse = velneoResponse?.Polizas?.Any() == true
                        ? velneoResponse.Polizas
                        : velneoResponse?.Contratos ?? new List<VelneoPoliza>();

                    if (polizasFromResponse.Any())
                    {
                        var polizasPage = polizasFromResponse.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        hasMoreData = polizasFromResponse.Count == pageSize &&
                                     (velneoResponse?.HasMoreData == true || allPolizas.Count < (velneoResponse?.Total_Count ?? 0));

                        if (hasMoreData)
                        {
                            pageNumber++;
                        }
                    }
                    else
                    {
                        hasMoreData = false;
                    }
                }

                _logger.LogInformation("✅ COMPLETADO: {TotalRetrieved} contratos obtenidos para cliente {ClienteId}",
                    allPolizas.Count, clienteId);
                return allPolizas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR en GetPolizasByClientAsync para cliente {ClienteId}: {Message}", clienteId, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// ✅ MIGRADO DESDE: TenantAwareVelneoApiService.CreatePolizaFromRequestAsync()
        /// 🚨 MÉTODO MÁS CRÍTICO DEL SISTEMA - Crea una nueva póliza en Velneo
        /// </summary>
        public async Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🚀 CREANDO PÓLIZA EN VELNEO: Número={NumeroPoliza}, Cliente={ClienteId}, Tenant={TenantId}",
                    request.Conpol, request.Clinro, tenantId);

                // ✅ VALIDAR CAMPOS CRÍTICOS ANTES DE ENVIAR
                ValidarCamposRequeridos(request);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();

                // ✅ MAPEAR COMPLETAMENTE A ESTRUCTURA VELNEO (USANDO MAESTROS INTERNOS)
                var velneoContrato = await MapearCreateRequestAVelneoCompleto(request);

                // Serializar el payload
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(velneoContrato, GetJsonOptions());
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("📤 PAYLOAD VELNEO: {JsonLength} caracteres para póliza {NumeroPoliza}",
                    jsonPayload.Length, request.Conpol);

                // ✅ ENVIAR A VELNEO
                var url = await _httpService.BuildVelneoUrlAsync("v1/contratos");
                var response = await httpClient.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    stopwatch.Stop();
                    _logger.LogInformation("✅ PÓLIZA CREADA EXITOSAMENTE en Velneo: {NumeroPoliza} - Duration: {Duration}ms",
                        request.Conpol, stopwatch.ElapsedMilliseconds);

                    var result = System.Text.Json.JsonSerializer.Deserialize<object>(responseContent, GetJsonOptions());
                    return result ?? throw new ApplicationException("Respuesta de Velneo vacía");
                }
                else
                {
                    var errorMessage = await ExtraerMensajeError(response.StatusCode, responseContent);
                    stopwatch.Stop();
                    _logger.LogError("❌ ERROR creando póliza {NumeroPoliza}: {Error} - Duration: {Duration}ms",
                        request.Conpol, errorMessage, stopwatch.ElapsedMilliseconds);

                    throw new ApplicationException($"Error creando póliza en Velneo: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "💥 EXCEPCIÓN creando póliza {NumeroPoliza} - Duration: {Duration}ms",
                    request.Conpol, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        // ============================================================================
        // 🔧 MÉTODOS AUXILIARES PARA CreatePolizaFromRequestAsync()
        // AGREGAR AL VelneoMaestrosService.cs EN LA SECCIÓN PRIVATE
        // ============================================================================

        /// <summary>
        /// ✅ MIGRADO: Validación de campos requeridos
        /// </summary>
        private void ValidarCamposRequeridos(PolizaCreateRequest request)
        {
            var errores = new List<string>();

            if (request.Comcod <= 0)
                errores.Add("Código de compañía debe ser mayor a 0");

            if (request.Clinro <= 0)
                errores.Add("Código de cliente debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(request.Conpol))
                errores.Add("Número de póliza es requerido");

            if (string.IsNullOrWhiteSpace(request.Confchdes))
                errores.Add("Fecha de inicio es requerida");

            if (string.IsNullOrWhiteSpace(request.Confchhas))
                errores.Add("Fecha de fin es requerida");

            if (request.Conpremio <= 0)
                errores.Add("Premio debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(request.Asegurado))
                errores.Add("Nombre del asegurado es requerido");

            if (errores.Any())
            {
                var mensajeError = string.Join(", ", errores);
                _logger.LogError("❌ VALIDACIÓN FALLIDA: {Errores}", mensajeError);
                throw new ArgumentException($"Datos requeridos faltantes: {mensajeError}");
            }

            _logger.LogInformation("✅ Validación de campos requeridos exitosa");
        }

        /// <summary>
        /// ✅ MIGRADO: Mapeo completo del request a estructura Velneo
        /// MEJORADO: Usa métodos internos de maestros en lugar de llamadas HTTP separadas
        /// </summary>
        private async Task<object> MapearCreateRequestAVelneoCompleto(PolizaCreateRequest request)
        {
            var now = DateTime.UtcNow;
            var nowLocal = DateTime.Now;

            var velneoContrato = new
            {
                id = 0,
                comcod = request.Comcod,
                seccod = ResolverSeccion(request),
                clinro = request.Clinro,
                condom = ResolverDireccion(request),
                conmaraut = ResolverMarca(request),
                conanioaut = ResolverAnio(request),
                concodrev = 0,
                conmataut = ResolverMatricula(request),
                conficto = 0,
                conmotor = ResolverMotor(request),
                conpadaut = request.Conpadaut ?? "",
                conchasis = ResolverChasis(request),
                conclaaut = request.Conclaaut ?? 0,
                condedaut = request.Condedaut ?? 0,
                conresciv = request.Conresciv ?? 0,
                conbonnsin = request.Conbonnsin ?? 0,
                conbonant = request.Conbonant ?? 0,
                concaraut = request.Concaraut ?? 0,
                concesnom = request.Concesnom ?? "",
                concestel = request.Concestel ?? "",
                concapaut = request.Concapaut ?? 0,
                conpremio = request.Conpremio,
                contot = ResolverTotal(request),
                moncod = ResolverMoneda(request),
                concuo = ResolverCuotas(request),
                concomcorr = 0,

                // ✅ CAMPOS CON MAPEO DINÁMICO ASYNC - USANDO MÉTODOS INTERNOS DE MAESTROS
                catdsc = await ResolverCategoria(request),
                desdsc = await ResolverDestino(request),
                caldsc = await ResolverCalidad(request),
                combustibles = await ResolverCombustible(request),

                flocod = request.Flocod ?? 0,
                concar = ResolverCertificado(request),
                conpol = request.Conpol ?? "",
                conend = request.Conend ?? "0",

                confchdes = FormatearFecha(request.Confchdes) ?? nowLocal.ToString("yyyy-MM-dd"),
                confchhas = FormatearFecha(request.Confchhas) ?? nowLocal.AddYears(1).ToString("yyyy-MM-dd"),

                conimp = request.Conimp ?? request.Conpremio,
                connroser = 0,
                rieres = "",
                conges = request.Conges ?? "Procesado automáticamente",
                congesti = ResolverTipoGestion(request),
                congesfi = FormatearFecha(request.Congesfi) ?? nowLocal.ToString("yyyy-MM-dd"),
                congeses = ResolverEstadoGestion(request),
                convig = ResolverEstadoPoliza(request),
                consta = ResolverFormaPago(request),
                contra = ResolverTramite(request),
                tposegdsc = ResolverCobertura(request),
                clinom = ResolverNombreCliente(request),
                tarcod = request.Tarcod ?? 0,
                corrnom = request.Corrnom ?? 0,
                observaciones = ResolverObservaciones(request),
                ramo = request.Ramo ?? "AUTOMOVILES",
                clausula = "1",
                terrestre = true,
                coning = "Sistema Automático",
                ingresado = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                last_update = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                app_id = 1,
                update_date = nowLocal.ToString("yyyy-MM-dd")
            };

            return velneoContrato;
        }

        // ============================================================================
        // 🔄 MÉTODOS RESOLVER - USANDO MAESTROS INTERNOS
        // ============================================================================

        /// <summary>
        /// ✅ MEJORADO: ResolverCategoria usando métodos internos de maestros
        /// </summary>
        private async Task<int> ResolverCategoria(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Categoria))
                {
                    // ✅ USAR MÉTODO INTERNO GetAllCategoriasAsync() 
                    var categorias = await GetAllCategoriasAsync();
                    var categoria = BuscarCategoriaPorTexto(categorias, request.Categoria);

                    if (categoria != null && categoria.Id > 0)
                    {
                        _logger.LogInformation("✅ Categoría mapeada: '{Texto}' -> ID {Id}", request.Categoria, categoria.Id);
                        return categoria.Id;
                    }
                }

                var defaultCode = 3; // "Automóvil particular"
                _logger.LogInformation("📋 Usando categoría default: {Codigo}", defaultCode);
                return defaultCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo categoría, usando fallback");
                return 3;
            }
        }

        /// <summary>
        /// ✅ MEJORADO: ResolverDestino usando métodos internos de maestros
        /// </summary>
        private async Task<int> ResolverDestino(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Destino))
                {
                    // ✅ USAR MÉTODO INTERNO GetAllDestinosAsync()
                    var destinos = await GetAllDestinosAsync();
                    var destino = BuscarDestinoPorTexto(destinos, request.Destino);

                    if (destino != null && destino.Id > 0)
                    {
                        _logger.LogInformation("✅ Destino mapeado: '{Texto}' -> ID {Id}", request.Destino, destino.Id);
                        return destino.Id;
                    }
                }

                var defaultCode = 2; // "PARTICULAR"
                _logger.LogInformation("📋 Usando destino default: {Codigo}", defaultCode);
                return defaultCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo destino, usando fallback");
                return 2;
            }
        }

        /// <summary>
        /// ✅ MEJORADO: ResolverCalidad usando métodos internos de maestros
        /// </summary>
        private async Task<int> ResolverCalidad(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Calidad))
                {
                    // ✅ USAR MÉTODO INTERNO GetAllCalidadesAsync()
                    var calidades = await GetAllCalidadesAsync();
                    var calidad = BuscarCalidadPorTexto(calidades, request.Calidad);

                    if (calidad != null && calidad.Id > 0)
                    {
                        _logger.LogInformation("✅ Calidad mapeada: '{Texto}' -> ID {Id}", request.Calidad, calidad.Id);
                        return calidad.Id;
                    }
                }

                var defaultCode = 2; // "PROPIETARIO"
                _logger.LogInformation("📋 Usando calidad default: {Codigo}", defaultCode);
                return defaultCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo calidad, usando fallback");
                return 2;
            }
        }

        /// <summary>
        /// ✅ MEJORADO: ResolverCombustible usando métodos internos de maestros
        /// </summary>
        private async Task<string> ResolverCombustible(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Combustible))
                {
                    // ✅ USAR MÉTODO INTERNO GetAllCombustiblesAsync()
                    var combustibles = await GetAllCombustiblesAsync();
                    var combustible = BuscarCombustiblePorTexto(combustibles, request.Combustible);

                    if (combustible != null && !string.IsNullOrEmpty(combustible.Id))
                    {
                        _logger.LogInformation("✅ Combustible mapeado: '{Texto}' -> ID {Id}", request.Combustible, combustible.Id);
                        return combustible.Id;
                    }
                }

                var defaultValue = "GASOLINA";
                _logger.LogInformation("📋 Usando combustible default: {Valor}", defaultValue);
                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo combustible, usando fallback");
                return "GASOLINA";
            }
        }

        // ============================================================================
        // 🔍 MÉTODOS DE BÚSQUEDA INTELIGENTE (MIGRADOS DEL MONOLITO)
        // ============================================================================

        /// <summary>
        /// ✅ MIGRADO: Búsqueda inteligente de categoría por texto
        /// </summary>
        private static CategoriaDto? BuscarCategoriaPorTexto(IEnumerable<CategoriaDto> categorias, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            // Búsqueda exacta
            var exacta = categorias.FirstOrDefault(c => c.Catdsc?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            // Mapeo específico inteligente
            return textoUpper switch
            {
                "AUTOMOVIL" or "AUTO" or "COCHE" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("Automóvil") == true),
                "CAMIONETA" or "PICKUP" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("Camioneta") == true || c.Catdsc?.Contains("Pick-Up") == true),
                "MOTO" or "MOTOCICLETA" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("MOTOS") == true),
                "JEEP" or "SUV" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("Jeeps") == true),
                _ => categorias.FirstOrDefault(c => c.Catdsc?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        /// <summary>
        /// ✅ MIGRADO: Búsqueda inteligente de destino por texto
        /// </summary>
        private static DestinoDto? BuscarDestinoPorTexto(IEnumerable<DestinoDto> destinos, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            // Búsqueda exacta
            var exacta = destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            // Búsqueda por palabras clave
            return textoUpper switch
            {
                "PARTICULAR" or "PERSONAL" or "PRIVADO" => destinos.FirstOrDefault(d => d.Desnom?.Contains("PARTICULAR") == true),
                "COMERCIAL" or "TRABAJO" or "LABORAL" => destinos.FirstOrDefault(d => d.Desnom?.Contains("TRABAJO") == true),
                "UBER" => destinos.FirstOrDefault(d => d.Desnom?.Contains("UBER") == true),
                _ => destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        /// <summary>
        /// ✅ MIGRADO: Búsqueda inteligente de calidad por texto
        /// </summary>
        private static CalidadDto? BuscarCalidadPorTexto(IEnumerable<CalidadDto> calidades, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            // Búsqueda exacta
            var exacta = calidades.FirstOrDefault(c => c.Caldsc?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            // Mapeo específico
            return textoUpper switch
            {
                "PROPIETARIO" or "DUEÑO" or "OWNER" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("PROPIETARIO") == true),
                "USUARIO" or "USER" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("USUARIO") == true),
                "ARRENDATARIO" or "INQUILINO" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("ARRENDATARIO") == true),
                "COMPRADOR" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("COMPRADOR") == true),
                _ => calidades.FirstOrDefault(c => c.Caldsc?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        /// <summary>
        /// ✅ MIGRADO: Búsqueda inteligente de combustible por texto
        /// </summary>
        private static CombustibleDto? BuscarCombustiblePorTexto(IEnumerable<CombustibleDto> combustibles, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            // Búsqueda exacta
            var exacta = combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            // Búsqueda por palabras clave
            return textoUpper switch
            {
                "NAFTA" or "GASOLINA" or "BENZINA" => combustibles.FirstOrDefault(c => c.Name?.Contains("GASOLINA") == true),
                "DIESEL" or "GASOIL" or "DIS" => combustibles.FirstOrDefault(c => c.Name?.Contains("DISEL") == true),
                "ELECTRICO" or "ELECTRIC" => combustibles.FirstOrDefault(c => c.Name?.Contains("ELECTRICOS") == true),
                "HIBRIDO" or "HYBRID" => combustibles.FirstOrDefault(c => c.Name?.Contains("HYBRIDO") == true),
                _ => combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        // ============================================================================
        // 🛠️ MÉTODOS AUXILIARES DE RESOLUCIÓN
        // ============================================================================

        private int ResolverSeccion(PolizaCreateRequest request)
        {
            return request.Seccod > 0 ? request.Seccod : 0;
        }

        private string ResolverDireccion(PolizaCreateRequest request)
        {
            return request.Condom ?? request.Direccion ?? "";
        }

        private string ResolverMarca(PolizaCreateRequest request)
        {
            return request.Conmaraut ?? request.Marca ?? "";
        }

        private int ResolverAnio(PolizaCreateRequest request)
        {
            if (request.Conanioaut.HasValue && request.Conanioaut.Value > 0)
                return request.Conanioaut.Value;
            if (request.Anio.HasValue && request.Anio.Value > 0)
                return request.Anio.Value;
            return DateTime.Now.Year;
        }

        private string ResolverMatricula(PolizaCreateRequest request)
        {
            return request.Conmataut ?? request.Matricula ?? "";
        }

        private string ResolverMotor(PolizaCreateRequest request)
        {
            return request.Conmotor ?? request.Motor ?? "";
        }

        private string ResolverChasis(PolizaCreateRequest request)
        {
            return request.Conchasis ?? request.Chasis ?? "";
        }

        private decimal ResolverTotal(PolizaCreateRequest request)
        {
            // ✅ USAR CAMPOS QUE SÍ EXISTEN: Contot, PremioTotal, Conpremio
            return request.Contot ?? request.PremioTotal ?? request.Conpremio;
        }

        private int ResolverMoneda(PolizaCreateRequest request)
        {
            // ✅ PRIORIDAD 1: Campo directo Moncod (nullable int)
            if (request.Moncod.HasValue && request.Moncod.Value > 0)
                return request.Moncod.Value;

            // ✅ PRIORIDAD 2: Campo legacy Moneda (string)
            return request.Moneda?.ToUpperInvariant() switch
            {
                "UYU" or "PESOS" => 1,
                "USD" or "DOLARES" => 2,
                "UI" or "UNIDADES INDEXADAS" => 3,
                _ => 1 // Default: Pesos uruguayos
            };
        }

        private int ResolverCuotas(PolizaCreateRequest request)
        {
            if (request.Concuo.HasValue && request.Concuo.Value > 0)
                return Math.Min(request.Concuo.Value, 12);

            if (request.CantidadCuotas.HasValue && request.CantidadCuotas.Value > 0)
                return Math.Min(request.CantidadCuotas.Value, 12);

            return 1;
        }

        private string ResolverCertificado(PolizaCreateRequest request)
        {
            return request.Concar ?? request.Certificado ?? "0";
        }

        private string ResolverTipoGestion(PolizaCreateRequest request)
        {
            return request.Congesti ?? "1";
        }

        private string ResolverEstadoGestion(PolizaCreateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Congeses))
                return request.Congeses;

            if (!string.IsNullOrEmpty(request.Estado))
                return MapearEstado(request.Estado);

            return "1";
        }

        private string ResolverEstadoPoliza(PolizaCreateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Convig))
                return request.Convig;

            if (!string.IsNullOrEmpty(request.EstadoPoliza))
                return MapearEstadoPolizaTextoACodigo(request.EstadoPoliza);

            return "VIG"; // Vigente por defecto
        }

        private string ResolverFormaPago(PolizaCreateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Consta))
                return request.Consta;

            if (!string.IsNullOrEmpty(request.FormaPago))
                return MapearFormaPago(request.FormaPago);

            return "1";
        }

        private string ResolverTramite(PolizaCreateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Contra))
                return request.Contra;

            if (!string.IsNullOrEmpty(request.Tramite))
                return MapearTramite(request.Tramite);

            return "Nuevo";
        }

        private string ResolverCobertura(PolizaCreateRequest request)
        {
            return request.Tposegdsc ?? request.Cobertura ?? "Responsabilidad Civil";
        }

        private string ResolverNombreCliente(PolizaCreateRequest request)
        {
            return request.Clinom ?? request.Asegurado ?? "";
        }

        private string ResolverObservaciones(PolizaCreateRequest request)
        {
            var obs = new List<string>();

            if (!string.IsNullOrEmpty(request.Observaciones))
                obs.Add(request.Observaciones);

            if (request.ProcesadoConIA)
                obs.Add("Procesado automáticamente con Azure AI");

            return string.Join(". ", obs);
        }
        private static string MapearEstado(string? estado)
        {
            return estado?.ToUpperInvariant() switch
            {
                "PENDIENTE" => "1",
                "PENDIENTE C/PLAZO" => "2",
                "PENDIENTE S/PLAZO" => "3",
                "TERMINADO" => "4",
                "EN PROCESO" => "5",
                "MODIFICACIONES" => "6",
                "EN EMISIÓN" => "7",
                "ENVIADO A CIA" => "8",
                "ENVIADO A AGENTE MAIL" => "9",
                "DEVUELTO A EJECUTIVO" => "10",
                "DECLINADO" => "11",
                _ => "1"
            };
        }

        private static string MapearEstadoPolizaTextoACodigo(string estadoPoliza)
        {
            return estadoPoliza?.ToUpperInvariant() switch
            {
                "VIGENTE" => "VIG",
                "ANULADA" => "ANU",
                "VENCIDA" => "VEN",
                "SUSPENDIDA" => "SUS",
                _ => "VIG"
            };
        }

        private static string MapearFormaPago(string? formaPago)
        {
            return formaPago?.ToUpperInvariant() switch
            {
                "CONTADO" => "1",
                "TARJETA" => "2",
                "TARJETA DE CREDITO" => "2",
                "TARJETA DE CRÉDITO" => "2",
                "DEBITO" => "3",
                "DEBITO AUTOMATICO" => "3",
                "DÉBITO AUTOMÁTICO" => "3",
                "CUOTAS" => "4",
                _ => "1"
            };
        }

        private static string MapearTramite(string? tramite)
        {
            return tramite?.ToUpperInvariant() switch
            {
                "NUEVO" => "Nuevo",
                "ALTA" => "Alta",
                "MODIFICACION" => "Modificación",
                "RENOVACION" => "Renovación",
                "ANULACION" => "Anulación",
                _ => "Nuevo"
            };
        }

        private static string? FormatearFecha(object? fecha)
        {
            if (fecha == null) return null;

            if (fecha is DateTime dt)
                return dt.ToString("yyyy-MM-dd");

            if (fecha is string str && DateTime.TryParse(str, out var parsedDate))
                return parsedDate.ToString("yyyy-MM-dd");

            return null;
        }

        private async Task<string> ExtraerMensajeError(HttpStatusCode statusCode, string responseContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(responseContent))
                    return $"Error HTTP {(int)statusCode} - Sin contenido";

                using var doc = System.Text.Json.JsonDocument.Parse(responseContent);

                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    return errorProp.GetString() ?? $"Error HTTP {(int)statusCode}";
                }

                if (doc.RootElement.TryGetProperty("message", out var messageProp))
                {
                    return messageProp.GetString() ?? $"Error HTTP {(int)statusCode}";
                }

                return $"Error HTTP {(int)statusCode}: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}";
            }
            catch (System.Text.Json.JsonException)
            {
                return $"Error HTTP {(int)statusCode}: {responseContent.Substring(0, Math.Min(100, responseContent.Length))}";
            }
            catch (Exception)
            {
                return $"Error HTTP {(int)statusCode}: Error al procesar respuesta";
            }
        }

        private System.Text.Json.JsonSerializerOptions GetJsonOptions()
        {
            return new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
        }
        private int EstimateTotalCount(int currentPageCount, int pageNumber, int pageSize)
        {
            if (currentPageCount < pageSize)
            {
                return ((pageNumber - 1) * pageSize) + currentPageCount;
            }
            else
            {
                return pageNumber * pageSize + 1;
            }
        }

        private async Task<List<ClientDto>> ParseDirectSearchResponse(HttpResponseMessage response, string searchTerm)
        {
            try
            {
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClientesResponse>(response);
                if (velneoResponse?.Clientes != null)
                {
                    return velneoResponse.Clientes.ToClienteDtos().ToList();
                }

                // ✅ Fallback: Si no funciona como wrapper, intentar como array directo
                // NOTE: Para simplificar, retornamos lista vacía si el wrapper falla
                // En producción se podría implementar una nueva llamada HTTP si es necesario

                return new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing direct search response for term '{SearchTerm}'", searchTerm);
                return new List<ClientDto>();
            }
        }

        private async Task<HttpClient> GetConfiguredHttpClientForClientsAsync()
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            if (tenantConfig == null)
            {
                throw new InvalidOperationException("Tenant configuration not found");
            }

            var httpClient = new HttpClient();

            var clientTimeoutSeconds = tenantConfig.TimeoutSeconds > 0 ?
                Math.Max(tenantConfig.TimeoutSeconds * 2, 90) :
                120; 

            httpClient.Timeout = TimeSpan.FromSeconds(clientTimeoutSeconds);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RegularizadorPolizas/1.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _logger.LogInformation("✅ HttpClient configured for CLIENTS with extended timeout: {Timeout}s (tenant: {TenantId})",
                clientTimeoutSeconds, _tenantService.GetCurrentTenantId());

            return httpClient;
        }
    }
}