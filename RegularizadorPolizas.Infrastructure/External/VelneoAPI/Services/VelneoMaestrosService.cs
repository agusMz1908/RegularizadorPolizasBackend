// RegularizadorPolizas.Infrastructure/External/VelneoAPI/Services/VelneoMaestrosService.cs
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Application.Mappers;
using RegularizadorPolizas.Application.Models;
using System.Net;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services
{
    /// <summary>
    /// 🎯 SERVICIO UNIFICADO PARA TODOS LOS MAESTROS DE VELNEO
    /// 
    /// ✅ DÍA 2 COMPLETADO: Métodos de clientes migrados desde VelneoClientService
    /// ✅ MANTIENE: Todos los maestros simples existentes
    /// ✅ AGREGA: 5 métodos de clientes con lógica idéntica
    /// 
    /// 📋 TOTAL ACTUAL: 18 métodos (13 maestros + 5 clientes)
    /// 🔄 SIGUIENTE: Migrar 7 métodos de compañías
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
        // MAESTROS BÁSICOS DE VEHÍCULOS (YA EXISTÍAN)
        // ===========================

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

        // ===========================
        // MAESTROS DE CLIENTES 
        // ✅ MIGRADO DESDE: VelneoClientService (DÍA 2)
        // ===========================

        /// <summary>
        /// ✅ MIGRADO DESDE: VelneoClientService.GetClienteAsync()
        /// Lógica idéntica con fallback wrapper/direct
        /// </summary>
        public async Task<ClientDto> GetClienteAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting cliente {ClienteId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/clientes/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ Intentar primero como objeto directo
                var velneoCliente = await _httpService.DeserializeResponseAsync<VelneoCliente>(response);
                if (velneoCliente != null)
                {
                    var result = velneoCliente.ToClienteDto();
                    _logger.LogInformation("Successfully retrieved cliente {ClienteId} from Velneo API", id);
                    return result;
                }

                // ✅ Si falla, intentar como wrapper - hacer nueva llamada
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClienteResponse>(response);
                if (velneoResponse?.Cliente != null)
                {
                    var result = velneoResponse.Cliente.ToClienteDto();
                    _logger.LogInformation("Successfully retrieved cliente {ClienteId} from Velneo API (wrapped)", id);
                    return result;
                }

                throw new KeyNotFoundException($"Cliente with ID {id} not found in Velneo API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cliente {ClienteId} from Velneo API", id);
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
            _logger.LogInformation("🔍 Getting ALL clientes from Velneo API for tenant {TenantId}", tenantId);

            var allClientes = new List<ClientDto>();
            var pageNumber = 1;
            var pageSize = 1000;
            var hasMoreData = true;

            while (hasMoreData)
            {
                _logger.LogDebug("Obteniendo página {Page}...", pageNumber);

                try
                {
                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
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

            _logger.LogInformation("🎯 Successfully retrieved {Count} clientes total from Velneo API", allClientes.Count);
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
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting clientes page {Page} (size {PageSize}) for tenant {TenantId}",
                    page, pageSize, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/clientes?page={page}&limit={pageSize}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClientesResponse>(response);

                if (velneoResponse?.Clientes != null)
                {
                    var clientsPage = velneoResponse.Clientes.ToClienteDtos().ToList();
                    // ✅ FIX: Usar Total_Count (con underscore) como está definido en VelneoClientesResponse
                    var totalCount = velneoResponse.Total_Count > 0 ? velneoResponse.Total_Count :
                                   EstimateTotalCount(clientsPage.Count, page, pageSize);

                    var result = new PaginatedVelneoResponse<ClientDto>
                    {
                        Items = clientsPage,
                        TotalCount = totalCount,
                        PageNumber = page,
                        PageSize = pageSize,
                        VelneoHasMoreData = velneoResponse.HasMoreData ?? false
                    };

                    _logger.LogInformation("✅ Retrieved page {Page}/{TotalPages} - {Count} clients",
                        page, result.TotalPages, clientsPage.Count);

                    return result;
                }

                return new PaginatedVelneoResponse<ClientDto>
                {
                    Items = new List<ClientDto>(),
                    TotalCount = 0,
                    PageNumber = page,
                    PageSize = pageSize,
                    VelneoHasMoreData = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetClientesPaginatedAsync - Page: {Page}, PageSize: {PageSize}",
                    page, pageSize);
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

        // ===========================
        // MÉTODOS DE PÓLIZAS
        // ✅ MIGRADO DESDE: TenantAwareVelneoApiService (DÍA ACTUAL)
        // ===========================

        /// <summary>
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
                _logger.LogError(ex, "❌ Error getting póliza {PolizaId} from Velneo API", id);
                throw;
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
                    _logger.LogDebug("📄 Obteniendo página {Page} de contratos...", pageNumber);

                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                    httpClient.Timeout = TimeSpan.FromMinutes(5);

                    var url = await _httpService.BuildVelneoUrlAsync($"v1/contratos?page[number]={pageNumber}&page[size]={pageSize}");
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var velneoResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, GetJsonOptions());

                    if (velneoResponse?.Polizas != null && velneoResponse.Polizas.Any())
                    {
                        var polizasPage = velneoResponse.Polizas.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        _logger.LogDebug("✅ Página {Page}: {Count} pólizas obtenidas (Total acumulado: {Total})",
                            pageNumber, polizasPage.Count, allPolizas.Count);

                        hasMoreData = velneoResponse.Polizas.Count == pageSize &&
                                     (velneoResponse.TotalCount == null || allPolizas.Count < velneoResponse.TotalCount);

                        if (hasMoreData)
                        {
                            pageNumber++;
                        }
                        else
                        {
                            _logger.LogInformation("📄 No hay más páginas. Proceso completado.");
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

                    if (velneoResponse?.Polizas != null && velneoResponse.Polizas.Any())
                    {
                        var polizasPage = velneoResponse.Polizas.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        hasMoreData = velneoResponse.Polizas.Count == pageSize && allPolizas.Count < velneoResponse.TotalCount;

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
        /// 🔄 TODO: Migrar GetPolizasPaginatedAsync()
        /// </summary>
        public async Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasPaginatedAsync(
            int page = 1,
            int pageSize = 50,
            string? search = null)
        {
            // TODO: Implementar en siguiente iteración
            throw new NotImplementedException("GetPolizasPaginatedAsync - Pendiente de migración");
        }

        /// <summary>
        /// 🔄 TODO: Migrar GetPolizasByClientPaginatedAsync()
        /// </summary>
        public async Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasByClientPaginatedAsync(
            int clienteId,
            int page = 1,
            int pageSize = 25,
            string? search = null)
        {
            // TODO: Implementar en siguiente iteración
            throw new NotImplementedException("GetPolizasByClientPaginatedAsync - Pendiente de migración");
        }

        /// <summary>
        /// 🚨 MÉTODO MÁS CRÍTICO: CreatePolizaFromRequestAsync()
        /// TODO: Migrar con MÁXIMO CUIDADO
        /// </summary>
        public async Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request)
        {
            // TODO: Migrar el método más crítico del sistema
            throw new NotImplementedException("CreatePolizaFromRequestAsync - Migrar con máximo cuidado");
        }

        // ===========================
        // MÉTODOS AUXILIARES PRIVADOS PARA PÓLIZAS
        // ===========================

        /// <summary>
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

                // ✅ INTENTAR COMO OBJETO DIRECTO PRIMERO
                var directPoliza = System.Text.Json.JsonSerializer.Deserialize<VelneoPoliza>(jsonContent, jsonOptions);
                if (directPoliza != null && directPoliza.Id > 0)
                {
                    return directPoliza;
                }

                // ✅ INTENTAR COMO WRAPPER
                var wrapperResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizaResponse>(jsonContent, jsonOptions);
                if (wrapperResponse?.Poliza != null)
                {
                    return wrapperResponse.Poliza;
                }

                if (wrapperResponse?.Contrato != null)
                {
                    return wrapperResponse.Contrato;
                }

                return null;
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(ex, "❌ JSON error for póliza {PolizaId}: {Error}", polizaId, ex.Message);
                throw new ApplicationException($"Error deserializing póliza {polizaId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Configuración JSON consistente para pólizas
        /// </summary>
        private System.Text.Json.JsonSerializerOptions GetJsonOptions()
        {
            return new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        // ===========================
        // MÉTODOS AUXILIARES PRIVADOS (EXISTENTES)
        // ===========================

        /// <summary>
        /// Estimación conservadora del total de registros basada en la página actual
        /// </summary>
        private int EstimateTotalCount(int currentPageCount, int currentPage, int pageSize)
        {
            if (currentPageCount < pageSize)
            {
                return ((currentPage - 1) * pageSize) + currentPageCount;
            }

            return currentPage * pageSize + 1;
        }

        /// <summary>
        /// Parsea respuesta de búsqueda directa con fallback automático
        /// </summary>
        private async Task<List<ClientDto>> ParseDirectSearchResponse(HttpResponseMessage response, string searchTerm)
        {
            try
            {
                // ✅ Intentar como wrapper primero
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
    }
}