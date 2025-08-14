using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Application.Mappers;
using RegularizadorPolizas.Application.Models;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

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

                var velneoCliente = await _httpService.DeserializeResponseAsync<VelneoCliente>(response);
                if (velneoCliente != null && velneoCliente.Id > 0)
                {
                    var result = velneoCliente.ToClienteDto();
                    _logger.LogInformation("✅ Successfully retrieved cliente {ClienteId} (direct format)", id);
                    return result;
                }

                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClienteResponse>(response);
                if (velneoResponse?.Cliente != null && velneoResponse.Cliente.Id > 0)
                {
                    var result = velneoResponse.Cliente.ToClienteDto();
                    _logger.LogInformation("✅ Successfully retrieved cliente {ClienteId} (wrapper format)", id);
                    return result;
                }

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

        public async Task<IEnumerable<ClientDto>> GetClientesAsync()
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            _logger.LogInformation("👥 Getting ALL clientes from Velneo API for tenant {TenantId} - EXTENDED TIMEOUT", tenantId);

            var allClientes = new List<ClientDto>();
            var pageNumber = 1;
            var pageSize = 500; 
            var hasMoreData = true;

            while (hasMoreData)
            {
                _logger.LogDebug("Obteniendo página {Page}...", pageNumber);

                try
                {
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

                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoCompaniesResponse>(response);
                if (velneoResponse?.Companias != null && velneoResponse.Companias.Any())
                {
                    var companies = velneoResponse.Companias.ToCompanyDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (wrapper format)", companies.Count);
                    return companies;
                }

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

        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            return await GetAllCompaniesAsync();
        }

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

                var velneoCompany = await _httpService.DeserializeResponseAsync<VelneoCompany>(response);
                if (velneoCompany != null)
                {
                    var result = velneoCompany.ToCompanyDto();
                    _logger.LogInformation("Successfully retrieved company {CompanyId} from Velneo API", id);
                    return result;
                }

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
                    _logger.LogDebug("Direct endpoint not found, searching in full list for codigo '{Codigo}'", codigo);
                }

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

                try
                {
                    var wrapperResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizaResponse>(jsonContent, jsonOptions);

                    if (wrapperResponse?.Poliza != null && wrapperResponse.Poliza.Id > 0)
                    {
                        _logger.LogDebug("✅ Póliza {PolizaId} deserializada desde wrapper.Poliza", polizaId);
                        return wrapperResponse.Poliza;
                    }

                    if (wrapperResponse?.Contrato != null && wrapperResponse.Contrato.Id > 0)
                    {
                        _logger.LogDebug("✅ Póliza {PolizaId} deserializada desde wrapper.Contrato", polizaId);
                        return wrapperResponse.Contrato;
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogDebug("⚠️ No se pudo deserializar como wrapper simple: {Error}", ex.Message);
                }

                try
                {
                    var listaResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, jsonOptions);

                    if (listaResponse?.Contratos != null && listaResponse.Contratos.Any())
                    {
                        var poliza = listaResponse.Contratos.FirstOrDefault(c => c.Id == polizaId)
                                   ?? listaResponse.Contratos.FirstOrDefault();

                        if (poliza != null && poliza.Id > 0)
                        {
                            _logger.LogDebug("✅ Póliza {PolizaId} deserializada desde array 'contratos'", polizaId);
                            return poliza;
                        }
                    }

                    if (listaResponse?.Polizas != null && listaResponse.Polizas.Any())
                    {
                        var poliza = listaResponse.Polizas.FirstOrDefault(p => p.Id == polizaId)
                                   ?? listaResponse.Polizas.FirstOrDefault();

                        if (poliza != null && poliza.Id > 0)
                        {
                            _logger.LogDebug("✅ Póliza {PolizaId} deserializada desde array 'polizas'", polizaId);
                            return poliza;
                        }
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogDebug("⚠️ No se pudo deserializar como lista: {Error}", ex.Message);
                }

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

                    var velneoResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, GetJsonOptions());

                    var polizasFromResponse = velneoResponse?.Polizas?.Any() == true
                        ? velneoResponse.Polizas
                        : velneoResponse?.Contratos ?? new List<VelneoPoliza>();

                    if (polizasFromResponse.Any())
                    {
                        var polizasPage = polizasFromResponse.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        _logger.LogDebug("✅ Página {Page}: {Count} pólizas obtenidas (Total acumulado: {Total})",
                            pageNumber, polizasPage.Count, allPolizas.Count);

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

        public async Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                if (tenantConfig.Mode?.ToUpper() != "VELNEO")
                {
                    _logger.LogWarning("⚠️ TENANT EN MODO {Mode} - SIMULANDO OPERACIÓN", tenantConfig.Mode);

                    return new
                    {
                        success = true,
                        message = $"Operación simulada en modo {tenantConfig.Mode}",
                        numeroPoliza = request.Conpol,
                        simulated = true,
                        mode = tenantConfig.Mode
                    };
                }

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();

                // 🔍 NUEVO: LOG DEL REQUEST ORIGINAL DEL FRONTEND
                _logger.LogInformation("📤 ===== REQUEST ORIGINAL DEL FRONTEND =====");
                var originalJson = System.Text.Json.JsonSerializer.Serialize(request, GetJsonOptions());
                _logger.LogInformation(originalJson);
                _logger.LogInformation("📤 ===== FIN REQUEST ORIGINAL =====");

                var velneoContrato = await MapearCreateRequestAVelneoCompleto(request);

                // 🔍 NUEVO: LOG DEL REQUEST DESPUÉS DEL MAPEO
                _logger.LogInformation("🔄 ===== REQUEST DESPUÉS DEL MAPEO =====");
                var mappedJson = System.Text.Json.JsonSerializer.Serialize(velneoContrato, GetJsonOptions());
                _logger.LogInformation(mappedJson);
                _logger.LogInformation("🔄 ===== FIN REQUEST MAPEADO =====");

                // 🔍 NUEVO: COMPARAR DIFERENCIAS
                _logger.LogInformation("🆚 ===== ANÁLISIS DE DIFERENCIAS =====");
                _logger.LogInformation("   📏 Tamaño original: {OriginalSize} caracteres", originalJson.Length);
                _logger.LogInformation("   📏 Tamaño mapeado: {MappedSize} caracteres", mappedJson.Length);

                // Verificar si el mapeo agregó campos problemáticos
                if (mappedJson.Contains("\"forpagvid\""))
                {
                    _logger.LogError("❌ PROBLEMA: El mapeo agregó 'forpagvid' (solo para seguros de vida)");
                }

                if (mappedJson.Contains("\"id\":"))
                {
                    _logger.LogError("❌ PROBLEMA: El mapeo agregó campo 'id' (Velneo lo rechaza en POST)");
                }

                _logger.LogInformation("🆚 ===== FIN ANÁLISIS =====");

                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(velneoContrato, GetJsonOptions());

                _logger.LogInformation("📋 ========== PAYLOAD COMPLETO ENVIADO A VELNEO ==========");
                _logger.LogInformation(jsonPayload);
                _logger.LogInformation("📋 ========== FIN PAYLOAD ==========");
                _logger.LogInformation("📊 Longitud del payload: {Length} caracteres", jsonPayload.Length);

                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(jsonPayload);
                    var root = doc.RootElement;

                    _logger.LogInformation("🔍 VALIDACIÓN DEL PAYLOAD:");
                    _logger.LogInformation("   - Tipo de root: {Type}", root.ValueKind);
                    _logger.LogInformation("   - Propiedades totales: {Count}", root.EnumerateObject().Count());

                    if (root.TryGetProperty("comcod", out var comcod))
                        _logger.LogInformation("   ✅ comcod: {Value} (tipo: {Type})", comcod.GetRawText(), comcod.ValueKind);
                    else
                        _logger.LogWarning("   ❌ comcod NO ENCONTRADO");

                    if (root.TryGetProperty("clinro", out var clinro))
                        _logger.LogInformation("   ✅ clinro: {Value} (tipo: {Type})", clinro.GetRawText(), clinro.ValueKind);
                    else
                        _logger.LogWarning("   ❌ clinro NO ENCONTRADO");

                    if (root.TryGetProperty("conpol", out var conpol))
                        _logger.LogInformation("   ✅ conpol: {Value} (tipo: {Type})", conpol.GetRawText(), conpol.ValueKind);
                    else
                        _logger.LogWarning("   ❌ conpol NO ENCONTRADO");

                    if (root.TryGetProperty("conpremio", out var conpremio))
                        _logger.LogInformation("   ✅ conpremio: {Value} (tipo: {Type})", conpremio.GetRawText(), conpremio.ValueKind);
                    else
                        _logger.LogWarning("   ❌ conpremio NO ENCONTRADO");

                    // 🔍 NUEVO: VERIFICAR CAMPOS PROBLEMÁTICOS
                    if (root.TryGetProperty("forpagvid", out var forpagvid))
                    {
                        _logger.LogError("❌ PROBLEMA CRÍTICO: El payload contiene 'forpagvid' (solo para seguros de vida)");
                        _logger.LogError("   💡 SOLUCIÓN: Eliminar 'forpagvid' del método MapearCreateRequestAVelneoCompleto");
                    }

                    // ✅ VALIDACIÓN CRÍTICA: Verificar que NO tenga campo "id"
                    if (root.TryGetProperty("id", out var idProp))
                    {
                        _logger.LogError("❌ PROBLEMA CRÍTICO: El payload contiene campo 'id' que Velneo rechaza en POST");
                        _logger.LogError("   💡 SOLUCIÓN: Eliminar 'id = 0,' del método MapearCreateRequestAVelneoCompleto");
                    }
                    else
                    {
                        _logger.LogInformation("✅ Payload correcto: NO contiene campo 'id'");
                    }
                }
                catch (Exception jsonEx)
                {
                    _logger.LogError(jsonEx, "❌ Error validando estructura JSON del payload");
                }

                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                var url = await _httpService.BuildVelneoUrlAsync("v1/contratos");
                _logger.LogInformation("🌐 URL: {Url}", url);

                _logger.LogInformation("📡 Enviando POST...");
                var response = await httpClient.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📥 RESPUESTA DE VELNEO:");
                _logger.LogInformation("   🔢 StatusCode: {StatusCode} ({StatusCodeText})",
                    (int)response.StatusCode, response.StatusCode);
                _logger.LogInformation("   ✅ IsSuccessStatusCode: {IsSuccess}", response.IsSuccessStatusCode);
                _logger.LogInformation("   📏 Content Length: {Length}", responseContent?.Length ?? 0);
                _logger.LogInformation("   📋 Content-Type: {ContentType}",
                    response.Content.Headers.ContentType?.ToString() ?? "No Content-Type");

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    _logger.LogWarning("   ⚠️ RESPUESTA VACÍA");
                }
                else
                {
                    _logger.LogInformation("   📄 Response Content: {Content}", responseContent);
                }

                var headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value));
                if (headers.Any())
                {
                    _logger.LogInformation("   🏷️ Response Headers: {@Headers}", headers);
                }

                if (response.IsSuccessStatusCode)
                {
                    stopwatch.Stop();
                    _logger.LogInformation("✅ HTTP SUCCESS en {Duration}ms", stopwatch.ElapsedMilliseconds);

                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        // ❌ SIN VERIFICACIÓN - SI VELNEO RETORNA VACÍO, ES UN ERROR
                        _logger.LogError("❌ VELNEO RETORNÓ HTTP 200 PERO RESPUESTA VACÍA");
                        _logger.LogError("   📋 Esto indica que Velneo rechazó el payload silenciosamente");
                        _logger.LogError("   💡 Posibles causas:");
                        _logger.LogError("      - Campo 'id' presente en el payload (debe eliminarse)");
                        _logger.LogError("      - Campo 'forpagvid' presente (solo para seguros de vida)");
                        _logger.LogError("      - Campo obligatorio faltante");
                        _logger.LogError("      - Tipo de dato incorrecto");
                        _logger.LogError("      - Foreign key inválida (comcod, clinro, seccod)");
                        _logger.LogError("      - Constraint de base de datos violado");

                        throw new Exception($"Velneo rechazó el payload para póliza {request.Conpol}. " +
                                          "HTTP 200 con respuesta vacía indica problema en el formato del payload. " +
                                          "Verificar logs para más detalles.");
                    }

                    // Si hay contenido en la respuesta, procesarlo
                    try
                    {
                        var trimmedContent = responseContent.Trim();

                        if (trimmedContent.StartsWith("{") || trimmedContent.StartsWith("["))
                        {
                            _logger.LogInformation("📄 Respuesta es JSON válido");
                            var result = System.Text.Json.JsonSerializer.Deserialize<object>(responseContent, GetJsonOptions());

                            if (result != null)
                            {
                                _logger.LogInformation("✅ Póliza {NumeroPoliza} creada exitosamente", request.Conpol);
                                return result;
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ JSON deserializado como null, pero operación exitosa");
                                return new
                                {
                                    success = true,
                                    message = "Póliza creada exitosamente",
                                    numeroPoliza = request.Conpol,
                                    rawResponse = responseContent
                                };
                            }
                        }
                        else
                        {
                            _logger.LogInformation("📄 Respuesta NO es JSON, pero HTTP 200 = éxito");
                            _logger.LogInformation("   🔤 Primeros caracteres: '{FirstChars}'",
                                trimmedContent.Substring(0, Math.Min(50, trimmedContent.Length)));

                            return new
                            {
                                success = true,
                                message = "Póliza creada exitosamente",
                                numeroPoliza = request.Conpol,
                                rawResponse = responseContent,
                                responseType = "non_json_success"
                            };
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "❌ Error deserializando JSON, pero HTTP 200 = éxito");
                        _logger.LogError("   📄 Contenido problemático: {Content}", responseContent);

                        return new
                        {
                            success = true,
                            message = "Póliza creada exitosamente",
                            numeroPoliza = request.Conpol,
                            rawResponse = responseContent,
                            parseError = jsonEx.Message,
                            responseType = "parse_error_but_success"
                        };
                    }
                }
                else
                {
                    stopwatch.Stop();
                    _logger.LogError("❌ ERROR HTTP DE VELNEO:");
                    _logger.LogError("   🔢 StatusCode: {StatusCode}", response.StatusCode);
                    _logger.LogError("   📄 Response: {Response}", responseContent);
                    _logger.LogError("   ⏱️ Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);

                    var errorMessage = await ExtraerMensajeError(response.StatusCode, responseContent);
                    throw new HttpRequestException($"Error HTTP {response.StatusCode}: {errorMessage}");
                }
            }
            catch (Exception ex) when (!(ex is JsonException || ex is HttpRequestException))
            {
                stopwatch.Stop();
                _logger.LogError(ex, "💥 EXCEPCIÓN INESPERADA creando póliza {NumeroPoliza} - Duration: {Duration}ms",
                    request?.Conpol, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task<object> MapearCreateRequestAVelneoCompleto(PolizaCreateRequest request)
        {
            var now = DateTime.UtcNow;
            var nowLocal = DateTime.Now;

            // 🔍 DEBUG: Log de valores que llegan al método
            _logger.LogInformation("🔍 ===== DEBUG VALORES DE ENTRADA =====");
            _logger.LogInformation("   request.Comcod: {Comcod}", request.Comcod);
            _logger.LogInformation("   request.Clinro: {Clinro}", request.Clinro);
            _logger.LogInformation("   ResolverSeccion(request): {Seccod}", ResolverSeccion(request));
            _logger.LogInformation("   request.Conpol: {Conpol}", request.Conpol);
            _logger.LogInformation("   request.Conpremio: {Conpremio}", request.Conpremio);
            _logger.LogInformation("🔍 ===== FIN DEBUG VALORES =====");

            // 🚨 DETECCIÓN DE PROBLEMA: Si los valores críticos son 0, usar valores de emergencia
            var comcodFinal = request.Comcod;
            var clinroFinal = request.Clinro;
            var seccodFinal = ResolverSeccion(request);

            if (comcodFinal == 0)
            {
                _logger.LogWarning("⚠️ PROBLEMA: request.Comcod es 0, usando BSE por defecto");
                comcodFinal = 1; // BSE por defecto
            }

            if (clinroFinal == 0)
            {
                _logger.LogError("❌ PROBLEMA CRÍTICO: request.Clinro es 0, esto causará falla");
                // Aquí podrías usar un cliente de test o lanzar excepción
                throw new ArgumentException("El ID del cliente no puede ser 0. Verificar mapeo del frontend.");
            }

            if (seccodFinal == 0)
            {
                _logger.LogWarning("⚠️ PROBLEMA: seccod es 0, usando AUTOMOVILES por defecto");
                seccodFinal = 1; // AUTOMOVILES por defecto
            }

            var velneoContrato = new
            {
                comcod = comcodFinal,                          // ✅ Valor verificado
                seccod = seccodFinal,                          // ✅ Valor verificado
                clinro = clinroFinal,                          // ✅ Valor verificado
                conpol = request.Conpol ?? "",
                confchdes = FormatearFecha(request.Confchdes) ?? nowLocal.ToString("yyyy-MM-dd"),
                confchhas = FormatearFecha(request.Confchhas) ?? nowLocal.AddYears(1).ToString("yyyy-MM-dd"),
                conpremio = request.Conpremio,
                condom = request.Condom ?? "",
                conmaraut = ResolverMarca(request),
                conanioaut = ResolverAnio(request),
                conmataut = ResolverMatricula(request),
                conmotor = ResolverMotor(request),
                conchasis = ResolverChasis(request),
                conpadaut = request.Conpadaut ?? "",
                contot = ResolverTotal(request),
                concuo = ResolverCuotas(request),
                conimp = request.Conimp ?? request.Conpremio,
                moncod = request.Moncod ?? 1,
                conviamon = ResolverMonedaCondicionesPago(request),
                catdsc = await ResolverCategoria(request),
                desdsc = await ResolverDestino(request),
                caldsc = await ResolverCalidad(request),
                flocod = 0,
                tarcod = await ResolverTarifa(request),
                corrnom = request.Corrnom ?? 0,
                clinom = ResolverNombreCliente(request),
                clinro1 = ResolverTomador(request),
                tposegdsc = ResolverCobertura(request),
                concar = ResolverCertificado(request),
                conend = request.Conend ?? "0",
                contra = "1",
                congesti = "1",
                congeses = ResolverEstadoGestion(request),
                convig = "1",
                consta = "1",
                congesfi = nowLocal.ToString("yyyy-MM-dd"),
                conges = ResolverGestionado(request),
                conclaaut = request.Conclaaut ?? 0,
                condedaut = request.Condedaut ?? 0,
                conresciv = request.Conresciv ?? 0,
                conbonnsin = request.Conbonnsin ?? 0,
                conbonant = request.Conbonant ?? 0,
                concaraut = request.Concaraut ?? 0,
                concapaut = request.Concapaut ?? 0,
                concesnom = request.Concesnom ?? "",
                concestel = request.Concestel ?? "",
                ramo = "AUTOMOVILES",
                com_alias = "BSE",
                observaciones = ResolverObservaciones(request),
                ingresado = nowLocal.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                last_update = nowLocal.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            return velneoContrato;
        }

        // 🚨 MÉTODO DE BYPASS - Agregar en VelneoMaestrosService.cs

        public async Task<object> CreatePolizaFromRequestAsync_BYPASS(PolizaCreateRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("🚨 ===== USANDO BYPASS - ENVIANDO REQUEST DIRECTO SIN MAPEO =====");

                var tenantId = _tenantService.GetCurrentTenantId();
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                if (tenantConfig.Mode?.ToUpper() != "VELNEO")
                {
                    _logger.LogWarning("⚠️ TENANT EN MODO {Mode} - SIMULANDO OPERACIÓN", tenantConfig.Mode);
                    return new
                    {
                        success = true,
                        message = $"Operación simulada en modo {tenantConfig.Mode}",
                        numeroPoliza = request.Conpol,
                        simulated = true,
                        mode = tenantConfig.Mode
                    };
                }

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();

                // 📤 SERIALIZAR REQUEST ORIGINAL DEL FRONTEND
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(request, GetJsonOptions());

                _logger.LogInformation("📤 REQUEST DIRECTO DEL FRONTEND (SIN MAPEO):");
                _logger.LogInformation(jsonPayload);
                _logger.LogInformation("📊 Tamaño del payload: {Length} caracteres", jsonPayload.Length);

                // 🔍 VALIDAR ESTRUCTURA
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(jsonPayload);
                    var root = doc.RootElement;

                    _logger.LogInformation("🔍 VALIDACIÓN BYPASS:");
                    _logger.LogInformation("   - Propiedades totales: {Count}", root.EnumerateObject().Count());

                    if (root.TryGetProperty("comcod", out var comcod))
                        _logger.LogInformation("   ✅ comcod: {Value}", comcod.GetRawText());

                    if (root.TryGetProperty("clinro", out var clinro))
                        _logger.LogInformation("   ✅ clinro: {Value}", clinro.GetRawText());

                    if (root.TryGetProperty("conpol", out var conpol))
                        _logger.LogInformation("   ✅ conpol: {Value}", conpol.GetRawText());

                    if (root.TryGetProperty("forpagvid", out var forpagvid))
                    {
                        _logger.LogError("❌ PROBLEMA: Frontend envía 'forpagvid' (solo para vida)");
                    }
                    else
                    {
                        _logger.LogInformation("✅ Correcto: NO contiene 'forpagvid'");
                    }

                    if (root.TryGetProperty("id", out var id))
                    {
                        _logger.LogError("❌ PROBLEMA: Frontend envía 'id' (Velneo lo rechaza)");
                    }
                    else
                    {
                        _logger.LogInformation("✅ Correcto: NO contiene 'id'");
                    }
                }
                catch (Exception validationEx)
                {
                    _logger.LogError(validationEx, "❌ Error validando request bypass");
                }

                // 🌐 ENVIAR A VELNEO DIRECTAMENTE
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                var url = await _httpService.BuildVelneoUrlAsync("v1/contratos");

                _logger.LogInformation("🌐 URL: {Url}", url);
                _logger.LogInformation("📡 Enviando POST BYPASS...");

                var response = await httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                stopwatch.Stop();

                _logger.LogInformation("📥 RESPUESTA BYPASS:");
                _logger.LogInformation("   🔢 StatusCode: {StatusCode} ({StatusCodeText})",
                    (int)response.StatusCode, response.StatusCode);
                _logger.LogInformation("   ✅ IsSuccessStatusCode: {IsSuccess}", response.IsSuccessStatusCode);
                _logger.LogInformation("   📏 Content Length: {Length}", responseContent?.Length ?? 0);
                _logger.LogInformation("   ⏱️ Duration: {Duration}ms", stopwatch.ElapsedMilliseconds);

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    _logger.LogWarning("   ⚠️ RESPUESTA VACÍA EN BYPASS");
                }
                else
                {
                    _logger.LogInformation("   📄 Response Content: {Content}", responseContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                        _logger.LogInformation("✅ BYPASS EXITOSO - Velneo aceptó el request directo del frontend");

                        try
                        {
                            var result = System.Text.Json.JsonSerializer.Deserialize<object>(responseContent, GetJsonOptions());
                            return result;
                        }
                        catch (JsonException jsonEx)
                        {
                            _logger.LogWarning(jsonEx, "⚠️ Error deserializando respuesta, pero bypass exitoso");
                            return new
                            {
                                success = true,
                                message = "Póliza creada exitosamente (bypass)",
                                numeroPoliza = request.Conpol,
                                rawResponse = responseContent,
                                bypassSuccess = true
                            };
                        }
                    }
                    else
                    {
                        _logger.LogError("❌ BYPASS FALLÓ - Velneo rechazó también el request directo del frontend");
                        throw new Exception($"Bypass falló: Velneo rechazó el request directo para póliza {request.Conpol}. " +
                                          "Esto indica que el problema está en el request del frontend, no en el mapeo.");
                    }
                }
                else
                {
                    _logger.LogError("❌ ERROR HTTP EN BYPASS: {StatusCode} - {Response}",
                        response.StatusCode, responseContent);
                    throw new HttpRequestException($"Bypass falló con HTTP {response.StatusCode}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "💥 EXCEPCIÓN EN BYPASS creando póliza {NumeroPoliza} - Duration: {Duration}ms",
                    request?.Conpol, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task<int> ResolverCategoria(PolizaCreateRequest request)
        {
            try
            {
                if (request.CategoriaId.HasValue && request.CategoriaId.Value > 0)
                {
                    _logger.LogInformation("✅ Usando CategoriaId existente: {Id}", request.CategoriaId.Value);
                    return request.CategoriaId.Value;
                }

                if (request.Catdsc.HasValue && request.Catdsc.Value > 0)
                {
                    _logger.LogInformation("✅ Usando catdsc existente: {Id}", request.Catdsc.Value);
                    return request.Catdsc.Value;
                }

                if (!string.IsNullOrEmpty(request.Categoria))
                {
                    var categorias = await GetAllCategoriasAsync();
                    var categoria = BuscarCategoriaPorTexto(categorias, request.Categoria);

                    if (categoria != null && categoria.Id > 0)
                    {
                        _logger.LogInformation("✅ Categoría mapeada: '{Texto}' -> ID {Id}", request.Categoria, categoria.Id);
                        return categoria.Id;
                    }
                }

                var defaultId = 20;
                _logger.LogInformation("📋 Usando categoría default para automóviles: {Id}", defaultId);
                return defaultId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo categoría, usando fallback");
                return 20; 
            }
        }

        private async Task<int> ResolverDestino(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Destino))
                {
                    var destinos = await GetAllDestinosAsync();
                    var destino = BuscarDestinoPorTexto(destinos, request.Destino);

                    if (destino != null && destino.Id > 0)
                    {
                        _logger.LogInformation("✅ Destino mapeado: '{Texto}' -> ID {Id}", request.Destino, destino.Id);
                        return destino.Id;
                    }
                }

                var defaultCode = 2; 
                _logger.LogInformation("📋 Usando destino default: {Codigo}", defaultCode);
                return defaultCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo destino, usando fallback");
                return 2;
            }
        }

        private string ResolverGestionado(PolizaCreateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Conges))
            {
                _logger.LogInformation("✅ Usuario gestionado especificado: {Usuario}", request.Conges);
                return request.Conges;
            }

            if (!string.IsNullOrEmpty(request.Conges))
            {
                _logger.LogInformation("✅ Usando campo asignado como usuario: {Usuario}", request.Conges);
                return request.Conges;
            }

            _logger.LogInformation("📋 Usando usuario por defecto: Sistema Automático");
            return "Sistema Automático";
        }

        private string ResolverEstadoGestion(PolizaCreateRequest request)
        {
            var mapeoEstados = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Pendiente", "1"},
                {"Pendiente c/plazo", "2"},
                {"En proceso", "3"},
                {"Terminado", "4"},
                {"Modificaciones", "5"},
                {"En emisión", "6"},
                {"Enviado a cia", "7"},
                {"Enviado a cia x mail", "8"},
                {"Devuelto a ejecutivo", "9"},
                {"Declinado", "10"}
            };

            // Verificar si viene el código directo
            if (!string.IsNullOrEmpty(request.Congeses))
            {
                // Si es un número, usarlo directamente
                if (int.TryParse(request.Congeses, out _))
                {
                    _logger.LogInformation("✅ Estado gestión código directo: {Estado}", request.Congeses);
                    return request.Congeses;
                }

                // Si es texto, mapearlo
                if (mapeoEstados.TryGetValue(request.Congeses, out var codigoEstado))
                {
                    _logger.LogInformation("✅ Estado gestión mapeado: '{Texto}' -> {Codigo}", request.Congeses, codigoEstado);
                    return codigoEstado;
                }
            }

            // Verificar campo alternativo EstadoTramite
            if (!string.IsNullOrEmpty(request.Congeses))
            {
                if (mapeoEstados.TryGetValue(request.Congeses, out var codigoEstado))
                {
                    _logger.LogInformation("✅ Estado trámite mapeado: '{Texto}' -> {Codigo}", request.Congeses, codigoEstado);
                    return codigoEstado;
                }
            }

            // Por defecto: Pendiente
            _logger.LogInformation("📋 Usando estado por defecto: Pendiente (1)");
            return "1";
        }

        private async Task<int> ResolverCalidad(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Calidad))
                {
                    var calidades = await GetAllCalidadesAsync();
                    var calidad = BuscarCalidadPorTexto(calidades, request.Calidad);

                    if (calidad != null && calidad.Id > 0)
                    {
                        _logger.LogInformation("✅ Calidad mapeada: '{Texto}' -> ID {Id}", request.Calidad, calidad.Id);
                        return calidad.Id;
                    }
                }

                var defaultCode = 2; 
                _logger.LogInformation("📋 Usando calidad default: {Codigo}", defaultCode);
                return defaultCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo calidad, usando fallback");
                return 2;
            }
        }

        private int ResolverTomador(PolizaCreateRequest request)
        {
            if (request.Clinro1.HasValue && request.Clinro1.Value > 0)
            {
                _logger.LogInformation("✅ Tomador específico: {TomadorId}", request.Clinro1.Value);
                return request.Clinro1.Value;
            }

            _logger.LogInformation("📋 Tomador = Asegurado: {ClienteId}", request.Clinro);
            return request.Clinro;
        }

        private async Task<int> ResolverTarifa(PolizaCreateRequest request)
        {
            try
            {
                if (request.Tarcod.HasValue && request.Tarcod.Value > 0)
                {
                    _logger.LogInformation("✅ Usando tarifa ID directo: {TarifaId}", request.Tarcod.Value);
                    return request.Tarcod.Value;
                }

                if (request.Tarcod.HasValue && request.Tarcod.Value > 0)
                {
                    _logger.LogInformation("✅ Usando TarifaId: {TarifaId}", request.Tarcod.Value);
                    return request.Tarcod.Value;
                }

                if (!string.IsNullOrEmpty(request.Cobertura) || !string.IsNullOrEmpty(request.Plan))
                {
                    var todasLasTarifas = await GetAllTarifasAsync();
                    var tarifasCompania = todasLasTarifas.Where(t => t.CompaniaId == request.Comcod).ToList();

                    if (!tarifasCompania.Any())
                    {
                        _logger.LogWarning("⚠️ No se encontraron tarifas para la compañía {CompaniaId}", request.Comcod);
                        return 0; 
                    }

                    var nombreBuscar = request.Cobertura ?? request.Plan ?? "";
                    var tarifa = BuscarTarifaPorTexto(tarifasCompania, nombreBuscar);

                    if (tarifa != null && tarifa.Id > 0)
                    {
                        _logger.LogInformation("✅ Tarifa mapeada: '{Texto}' -> ID {Id} (Compañía {CompaniaId})",
                            nombreBuscar, tarifa.Id, request.Comcod);
                        return tarifa.Id;
                    }
                }

                var tarifasDisponibles = await GetAllTarifasAsync();
                var primeraDeCompania = tarifasDisponibles
                    .Where(t => t.CompaniaId == request.Comcod && t.Activa)
                    .FirstOrDefault();

                if (primeraDeCompania != null)
                {
                    _logger.LogInformation("📋 Usando primera tarifa disponible de la compañía: {Id}", primeraDeCompania.Id);
                    return primeraDeCompania.Id;
                }

                _logger.LogInformation("📋 Sin tarifa específica para la compañía {CompaniaId}", request.Comcod);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo tarifa, usando fallback");
                return 0; 
            }
        }

        private async Task<int> ResolverDepartamentoId(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Departamento))
                {
                    var todosLosDepartamentos = await GetAllDepartamentosAsync();
                    var departamento = BuscarDepartamentoPorTexto(todosLosDepartamentos, request.Departamento);

                    if (departamento != null && departamento.Id > 0)
                    {
                        _logger.LogInformation("✅ Departamento mapeado: '{Texto}' -> ID {Id}",
                            request.Departamento, departamento.Id);
                        return departamento.Id;
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Departamento no encontrado: '{Texto}'", request.Departamento);
                    }
                }
                _logger.LogInformation("📋 Usando departamento por defecto: Montevideo (ID=1)");
                return 1; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo departamento, usando fallback Montevideo");
                return 1; 
            }
        }

        private DepartamentoDto? BuscarDepartamentoPorTexto(IEnumerable<DepartamentoDto> departamentos, string texto)
        {
            if (string.IsNullOrEmpty(texto) || departamentos == null) return null;

            var textoNormalizado = texto.Trim().ToUpperInvariant();
            var exacta = departamentos.FirstOrDefault(d =>
                d.Nombre?.ToUpperInvariant() == textoNormalizado);
            if (exacta != null) return exacta;

            var parcial = departamentos.FirstOrDefault(d =>
                !string.IsNullOrEmpty(d.Nombre) &&
                (d.Nombre.ToUpperInvariant().Contains(textoNormalizado) ||
                 textoNormalizado.Contains(d.Nombre.ToUpperInvariant())));

            return parcial;
        }

        private TarifaDto? BuscarTarifaPorTexto(IEnumerable<TarifaDto> tarifas, string texto)
        {
            if (string.IsNullOrEmpty(texto) || tarifas == null) return null;

            var textoNormalizado = texto.Trim().ToUpperInvariant();
            var exacta = tarifas.FirstOrDefault(t =>
                t.Nombre?.ToUpperInvariant() == textoNormalizado);
            if (exacta != null) return exacta;


            var porCodigo = tarifas.FirstOrDefault(t =>
                !string.IsNullOrEmpty(t.Codigo) &&
                t.Codigo.ToUpperInvariant() == textoNormalizado);
            if (porCodigo != null) return porCodigo;

            var parcial = tarifas.FirstOrDefault(t =>
                !string.IsNullOrEmpty(t.Nombre) &&
                (t.Nombre.ToUpperInvariant().Contains(textoNormalizado) ||
                 textoNormalizado.Contains(t.Nombre.ToUpperInvariant())));

            return parcial;
        }

        private async Task<string> ResolverCombustible(PolizaCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Combustible))
                {
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

        private CategoriaDto? BuscarCategoriaPorTexto(IEnumerable<CategoriaDto> categorias, string texto)
        {
            if (string.IsNullOrEmpty(texto) || categorias == null) return null;

            var textoNormalizado = texto.Trim().ToUpperInvariant();

            var exacta = categorias.FirstOrDefault(c =>
                c.Catdsc?.ToUpperInvariant() == textoNormalizado);
            if (exacta != null) return exacta;

            var parcial = categorias.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Catdsc) &&
                (c.Catdsc.ToUpperInvariant().Contains(textoNormalizado) ||
                 textoNormalizado.Contains(c.Catdsc.ToUpperInvariant())));

            return parcial;
        }

        private static DestinoDto? BuscarDestinoPorTexto(IEnumerable<DestinoDto> destinos, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            var exacta = destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            var exactaCodigo = destinos.FirstOrDefault(d => d.Descod?.ToUpperInvariant() == textoUpper);
            if (exactaCodigo != null) return exactaCodigo;

            return textoUpper switch
            {
                "PARTICULAR" or "PERSONAL" or "PRIVADO" =>
                    destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains("PARTICULAR") == true),

                "COMERCIAL" or "TRABAJO" or "LABORAL" or "BUSINESS" or "COMERCIO" =>
                    destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains("TRABAJO") == true),

                "PARTICULAR Y TRABAJO" or "MIXTO" or "AMBOS" =>
                    destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains("PARTICULAR Y TRABAJO") == true),

                "UBER" =>
                    destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains("UBER") == true),

                "TAXI" or "REMISE" =>
                    destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains("TAXI") == true || d.Desnom?.ToUpperInvariant().Contains("REMISE") == true),

                _ => destinos.FirstOrDefault(d => d.Desnom?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        private static CalidadDto? BuscarCalidadPorTexto(IEnumerable<CalidadDto> calidades, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            var exacta = calidades.FirstOrDefault(c => c.Caldsc?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            return textoUpper switch
            {
                "PROPIETARIO" or "DUEÑO" or "OWNER" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("PROPIETARIO") == true),
                "USUARIO" or "USER" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("USUARIO") == true),
                "ARRENDATARIO" or "INQUILINO" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("ARRENDATARIO") == true),
                "COMPRADOR" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("COMPRADOR") == true),
                _ => calidades.FirstOrDefault(c => c.Caldsc?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        private static CombustibleDto? BuscarCombustiblePorTexto(IEnumerable<CombustibleDto> combustibles, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            var exacta = combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            var exactaId = combustibles.FirstOrDefault(c => c.Id?.ToUpperInvariant() == textoUpper);
            if (exactaId != null) return exactaId;

            return textoUpper switch
            {
                "DIESEL" or "GASOIL" or "GAS-OIL" or "DISEL" or "DIS" =>
                    combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant().Contains("DISEL") == true || c.Id?.ToUpperInvariant() == "DIS"),

                "GASOLINA" or "NAFTA" or "SUPER" or "GAS" =>
                    combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant().Contains("GASOLINA") == true || c.Id?.ToUpperInvariant() == "GAS"),

                "ELECTRICO" or "ELECTRIC" or "ELE" =>
                    combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant().Contains("ELECTRICO") == true || c.Id?.ToUpperInvariant() == "ELE"),

                "HIBRIDO" or "HYBRID" or "HYB" =>
                    combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant().Contains("HYBRIDO") == true || c.Id?.ToUpperInvariant() == "HYB"),

                _ => combustibles.FirstOrDefault(c => c.Name?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

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
            return request.Contot ?? request.PremioTotal ?? request.Conpremio;
        }

        private int ResolverMonedaCobertura(PolizaCreateRequest request)
        {
            if (request.Moncod.HasValue && request.Moncod.Value > 0)
                return request.Moncod.Value;

            if (!string.IsNullOrEmpty(request.Moneda))
                return MapearTextoAMonedaId(request.Moneda);

            return 1;
        }

        private int ResolverMonedaCondicionesPago(PolizaCreateRequest request)
        {
            if (request.Conviamon.HasValue && request.Conviamon.Value > 0)
                return request.Conviamon.Value;

            return ResolverMonedaCobertura(request);
        }

        private int MapearTextoAMonedaId(string moneda)
        {
            if (string.IsNullOrEmpty(moneda)) return 1;

            return moneda.ToUpperInvariant().Trim() switch
            {
                "UYU" or "PESOS" or "PESO URUGUAYO" or "URUGUAYOS" or "$U" or "PES" => 1,
                "USD" or "DOLARES" or "DOLLAR" or "DOLAR" or "$" or "DOL" => 2,
                "UI" or "UNIDADES INDEXADAS" or "UNIDAD INDEXADA" => 3,
                "EUR" or "EUROS" or "EURO" or "€" or "EU" => 4,
                "BRL" or "REAL" or "REALES" or "R$" or "RS" => 5,
                "UF" or "UNIDAD DE FOMENTO" => 6,
                _ => 1 
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
            var observaciones = new StringBuilder();

            // Agregar observaciones originales si existen
            if (!string.IsNullOrEmpty(request.Observaciones))
            {
                observaciones.AppendLine(request.Observaciones);
            }

            // Agregar nota de procesamiento con IA si aplica
            if (request.ProcesadoConIA == true)
            {
                observaciones.AppendLine("Procesado con IA");
            }

            // NUEVO: Agregar detalle de cuotas si hay más de una cuota
            var cantidadCuotas = ResolverCuotas(request);
            if (cantidadCuotas > 1)
            {
                observaciones.AppendLine();
                observaciones.AppendLine("=== DETALLE DE CUOTAS ===");
                observaciones.AppendLine($"Plan de pago: {cantidadCuotas} cuotas");

                // Calcular valores
                decimal premioTotal = request.Contot ?? request.PremioTotal ?? request.Conpremio;
                decimal valorCuota = request.ValorCuota ?? (premioTotal / cantidadCuotas);

                // Obtener símbolo de moneda
                string simboloMoneda = ObtenerSimboloMoneda(ResolverMonedaCondicionesPago(request));

                observaciones.AppendLine($"Total: {simboloMoneda} {premioTotal:N2}");
                observaciones.AppendLine($"Valor por cuota: {simboloMoneda} {valorCuota:N2}");

                // Generar detalle de cada cuota
                DateTime fechaBase = DateTime.Now;
                if (DateTime.TryParse(request.Confchdes, out DateTime fechaDesde))
                {
                    fechaBase = fechaDesde;
                }

                observaciones.AppendLine();
                observaciones.AppendLine("Cronograma de pagos:");

                for (int i = 1; i <= cantidadCuotas; i++)
                {
                    DateTime fechaVencimiento = fechaBase.AddMonths(i - 1);
                    observaciones.AppendLine($"  Cuota {i}/{cantidadCuotas}: {simboloMoneda} {valorCuota:N2} - Vence: {fechaVencimiento:dd/MM/yyyy}");
                }
            }
            else if (cantidadCuotas == 1)
            {
                observaciones.AppendLine($"Pago: Contado - {ObtenerSimboloMoneda(ResolverMonedaCondicionesPago(request))} {request.Conpremio:N2}");
            }

            return observaciones.ToString().Trim();
        }

        // Método auxiliar para obtener símbolo de moneda
        private string ObtenerSimboloMoneda(int monedaId)
        {
            return monedaId switch
            {
                1 => "$U",     // Peso uruguayo
                2 => "USD",    // Dólar
                3 => "UI",     // Unidades indexadas
                4 => "€",      // Euro
                5 => "R$",     // Real brasileño
                6 => "UF",     // Unidad de fomento
                _ => "$"
            };
        }

        private static string? FormatearFecha(object? fecha)
        {
            if (fecha == null) return null;

            try
            {
                if (fecha is DateTime dt)
                    return dt.ToString("yyyy-MM-dd");

                if (fecha is string str && DateTime.TryParse(str, out var parsedDate))
                    return parsedDate.ToString("yyyy-MM-dd");

                return null;
            }
            catch
            {
                return null;
            }
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

        public async Task<PolicyMappingResultDto> ValidarMapeoCompletoAsync(AzureDatosPolizaVelneoDto azureData)
        {
            var result = new PolicyMappingResultDto
            {
                CamposMapeados = new Dictionary<string, MappedField>(),
                CamposQueFallaronMapeo = new List<string>(),
                PorcentajeExito = 0
            };

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Iniciando validación de mapeo completo para tenant: {TenantId}", tenantId);

                // 1. CAMPOS DINÁMICOS (que usan maestros del backend)
                await MapearCamposDinamicos(azureData, result);

                // 2. CAMPOS DE TEXTO PLANO (valores fijos según imágenes)
                MapearCamposTextoPlano(azureData, result);

                // 3. CALCULAR MÉTRICAS
                CalcularMetricasMapeo(result);

                _logger.LogInformation("✅ Mapeo completado: {Exito}% de éxito, {Total} campos procesados",
                    result.PorcentajeExito, result.CamposMapeados.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en validación de mapeo completo");
                throw;
            }
        }
        private async Task MapearCamposDinamicos(AzureDatosPolizaVelneoDto azureData, PolicyMappingResultDto result)
        {
            try
            {
                _logger.LogDebug("🔄 Iniciando mapeo de campos dinámicos...");

                if (!string.IsNullOrEmpty(azureData.DatosVehiculo?.Categoria))
                {
                    var categorias = await GetAllCategoriasAsync();
                    var categoria = BuscarCategoriaPorTexto(categorias, azureData.DatosVehiculo.Categoria);

                    result.CamposMapeados["categoria"] = new MappedField
                    {
                        ValorExtraido = azureData.DatosVehiculo.Categoria,
                        ValorMapeado = categoria,
                        Confianza = categoria != null ? CalcularConfianza(azureData.DatosVehiculo.Categoria, categoria.Catdsc) : 0,
                        OpcionesDisponibles = categorias.ToList()
                    };

                    if (categoria == null) result.CamposQueFallaronMapeo.Add("categoria");
                }

                if (!string.IsNullOrEmpty(azureData.DatosVehiculo?.Destino))
                {
                    var destinos = await GetAllDestinosAsync();
                    var destino = BuscarDestinoPorTexto(destinos, azureData.DatosVehiculo.Destino);

                    result.CamposMapeados["destino"] = new MappedField
                    {
                        ValorExtraido = azureData.DatosVehiculo.Destino,
                        ValorMapeado = destino,
                        Confianza = destino != null ? CalcularConfianza(azureData.DatosVehiculo.Destino, destino.Desnom) : 0,
                        OpcionesDisponibles = destinos.ToList()
                    };

                    if (destino == null) result.CamposQueFallaronMapeo.Add("destino");
                }

                if (!string.IsNullOrEmpty(azureData.DatosVehiculo?.Calidad))
                {
                    var calidades = await GetAllCalidadesAsync();
                    var calidad = BuscarCalidadPorTexto(calidades, azureData.DatosVehiculo.Calidad);

                    result.CamposMapeados["calidad"] = new MappedField
                    {
                        ValorExtraido = azureData.DatosVehiculo.Calidad,
                        ValorMapeado = calidad,
                        Confianza = calidad != null ? CalcularConfianza(azureData.DatosVehiculo.Calidad, calidad.Caldsc) : 0,
                        OpcionesDisponibles = calidades.ToList()
                    };

                    if (calidad == null) result.CamposQueFallaronMapeo.Add("calidad");
                }

                if (!string.IsNullOrEmpty(azureData.DatosVehiculo?.Combustible))
                {
                    var combustibles = await GetAllCombustiblesAsync();
                    var combustible = BuscarCombustiblePorTexto(combustibles, azureData.DatosVehiculo.Combustible);

                    result.CamposMapeados["combustible"] = new MappedField
                    {
                        ValorExtraido = azureData.DatosVehiculo.Combustible,
                        ValorMapeado = combustible,
                        Confianza = combustible != null ? CalcularConfianza(azureData.DatosVehiculo.Combustible, combustible.Name) : 0,
                        OpcionesDisponibles = combustibles.ToList()
                    };

                    if (combustible == null) result.CamposQueFallaronMapeo.Add("combustible");
                }

                if (!string.IsNullOrEmpty(azureData.DatosCobertura?.Moneda))
                {
                    var monedas = await GetAllMonedasAsync();
                    var moneda = BuscarMonedaPorTexto(monedas, azureData.DatosCobertura.Moneda);

                    result.CamposMapeados["moneda"] = new MappedField
                    {
                        ValorExtraido = azureData.DatosCobertura.Moneda,
                        ValorMapeado = moneda,
                        Confianza = moneda != null ? CalcularConfianza(azureData.DatosCobertura.Moneda, moneda.Nombre) : 0,
                        OpcionesDisponibles = monedas.ToList()
                    };

                    if (moneda == null) result.CamposQueFallaronMapeo.Add("moneda");
                }

                _logger.LogDebug("✅ Mapeo de campos dinámicos completado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en mapeo de campos dinámicos");
                throw;
            }
        }

        private void MapearCamposTextoPlano(AzureDatosPolizaVelneoDto azureData, PolicyMappingResultDto result)
        {
            try
            {
                _logger.LogDebug("📝 Iniciando mapeo de campos de texto plano...");

                var estadosPoliza = new[] { "VIG", "ANT", "VEN", "END", "ELIM", "FIN" };
                var estadoMapeado = MapearEstadoPolizaInteligente(azureData.DatosBasicos?.Estado);

                result.CamposMapeados["estadoPoliza"] = new MappedField
                {
                    ValorExtraido = azureData.DatosBasicos?.Estado ?? "",
                    ValorMapeado = new { Text = estadoMapeado, Code = estadoMapeado },
                    Confianza = !string.IsNullOrEmpty(estadoMapeado) ? 90 : 0,
                    OpcionesDisponibles = estadosPoliza.Select(e => new { Text = e, Code = e }).ToList()
                };

                var tiposTramite = new[] { "Nuevo", "Renovación", "Cambio", "Endoso", "No Renueva", "Cancelación" };
                var tramiteMapeado = MapearTipoTramiteInteligente(azureData.DatosPoliza?.TipoMovimiento);

                result.CamposMapeados["tipoTramite"] = new MappedField
                {
                    ValorExtraido = azureData.DatosPoliza?.TipoMovimiento ?? "",
                    ValorMapeado = new { Text = tramiteMapeado, Code = tramiteMapeado },
                    Confianza = !string.IsNullOrEmpty(tramiteMapeado) ? 90 : 0,
                    OpcionesDisponibles = tiposTramite.Select(t => new { Text = t, Code = t }).ToList()
                };

                var estadosBasicos = new[] { "Pendiente", "Pendiente c/plazo", "Terminado", "En proceso",
                                    "Modificaciones", "En emisión", "Enviado a cía", "Enviado a cía x mail",
                                    "Devuelto a ejecutivo", "Declinado" };
                var estadoBasicoMapeado = MapearEstadoBasicoInteligente(azureData.DatosBasicos?.Estado);

                result.CamposMapeados["estadoBasico"] = new MappedField
                {
                    ValorExtraido = azureData.DatosBasicos?.Estado ?? "",
                    ValorMapeado = new { Text = estadoBasicoMapeado, Code = estadoBasicoMapeado },
                    Confianza = !string.IsNullOrEmpty(estadoBasicoMapeado) ? 90 : 0,
                    OpcionesDisponibles = estadosBasicos.Select(e => new { Text = e, Code = e }).ToList()
                };

                var tiposLinea = new[] { "Líneas personales", "Líneas comerciales" };
                var tipoLineaMapeado = MapearTipoLineaInteligente(azureData.DatosVehiculo?.Uso);

                result.CamposMapeados["tipoLinea"] = new MappedField
                {
                    ValorExtraido = azureData.DatosVehiculo?.Uso ?? "",
                    ValorMapeado = new { Text = tipoLineaMapeado, Code = tipoLineaMapeado },
                    Confianza = !string.IsNullOrEmpty(tipoLineaMapeado) ? 85 : 0,
                    OpcionesDisponibles = tiposLinea.Select(t => new { Text = t, Code = t }).ToList()
                };

                var formasPago = new[] { "Contado", "Tarjeta de Crédito", "Débito Automático", "Cuotas", "Financiado" };
                var formaPagoMapeada = MapearFormaPagoInteligente(azureData.CondicionesPago?.FormaPago);

                result.CamposMapeados["formaPago"] = new MappedField
                {
                    ValorExtraido = azureData.CondicionesPago?.FormaPago ?? "",
                    ValorMapeado = new { Text = formaPagoMapeada, Code = formaPagoMapeada },
                    Confianza = !string.IsNullOrEmpty(formaPagoMapeada) ? 90 : 0,
                    OpcionesDisponibles = formasPago.Select(f => new { Text = f, Code = f }).ToList()
                };

                _logger.LogDebug("✅ Mapeo de campos de texto plano completado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en mapeo de campos de texto plano");
                throw;
            }
        }

        private string MapearEstadoPolizaInteligente(string? estado)
        {
            if (string.IsNullOrEmpty(estado)) return "VIG";

            return estado.ToUpperInvariant().Trim() switch
            {
                "VIGENTE" or "ACTIVO" or "ACTIVE" or "VIG" => "VIG",
                "ANTERIOR" or "ANT" or "PREVIO" => "ANT",
                "VENCIDO" or "EXPIRED" or "VEN" => "VEN",
                "ENDOSO" or "ENDORSEMENT" or "END" => "END",
                "ELIMINADO" or "DELETED" or "ELIM" => "ELIM",
                "FINALIZADO" or "FINISHED" or "FIN" => "FIN",
                _ => "VIG" 
            };
        }

        private string MapearTipoTramiteInteligente(string? tipoMovimiento)
        {
            if (string.IsNullOrEmpty(tipoMovimiento)) return "Nuevo";

            return tipoMovimiento.ToUpperInvariant().Trim() switch
            {
                "EMISIÓN" or "EMISION" or "NUEVA" or "NEW" or "NUEVO" => "Nuevo",
                "RENOVACIÓN" or "RENOVACION" or "RENEWAL" or "RENEW" => "Renovación",
                "CAMBIO" or "MODIFICATION" or "CHANGE" or "MODIFICACION" => "Cambio",
                "ENDOSO" or "ENDORSEMENT" => "Endoso",
                "NO RENUEVA" or "NOT_RENEW" or "DECLINE" => "No Renueva",
                "CANCELACIÓN" or "CANCELACION" or "CANCEL" => "Cancelación",
                _ => "Nuevo" 
            };
        }

        private string MapearEstadoBasicoInteligente(string? estado)
        {
            if (string.IsNullOrEmpty(estado)) return "Pendiente"; 

            return estado.ToUpperInvariant().Trim() switch
            {
                "PENDIENTE" or "PENDING" => "Pendiente",
                "PENDIENTE CON PLAZO" or "PENDING_WITH_DEADLINE" => "Pendiente c/plazo",
                "TERMINADO" or "COMPLETED" or "FINISHED" => "Terminado",
                "EN PROCESO" or "IN_PROCESS" or "PROCESSING" => "En proceso",
                "MODIFICACIONES" or "MODIFICATIONS" => "Modificaciones",
                "EN EMISIÓN" or "EN_EMISION" or "ISSUING" => "En emisión",
                "ENVIADO A CIA" or "SENT_TO_COMPANY" => "Enviado a cía",
                "ENVIADO A CIA X MAIL" or "SENT_BY_EMAIL" => "Enviado a cía x mail",
                "DEVUELTO A EJECUTIVO" or "RETURNED" => "Devuelto a ejecutivo",
                "DECLINADO" or "DECLINED" => "Declinado",
                _ => "Pendiente" 
            };
        }

        private string MapearTipoLineaInteligente(string? uso)
        {
            if (string.IsNullOrEmpty(uso)) return "Líneas personales";

            return uso.ToUpperInvariant().Trim() switch
            {
                "PARTICULAR" or "PERSONAL" or "PRIVADO" => "Líneas personales",
                "COMERCIAL" or "TRABAJO" or "BUSINESS" or "LABORAL" => "Líneas comerciales",
                "TAXI" or "REMISE" or "UBER" => "Líneas comerciales", 
                _ => "Líneas personales" 
            };
        }

        private string MapearFormaPagoInteligente(string? formaPago)
        {
            if (string.IsNullOrEmpty(formaPago)) return "Tarjeta de Crédito"; 

            return formaPago.ToUpperInvariant().Trim() switch
            {
                "CONTADO" or "CASH" or "EFECTIVO" => "Contado",
                "TARJETA DE CRÉDITO" or "TARJETA DE CREDITO" or "CREDIT_CARD" or "TARJETA" => "Tarjeta de Crédito",
                "DÉBITO AUTOMÁTICO" or "DEBITO AUTOMATICO" or "AUTO_DEBIT" => "Débito Automático",
                "CUOTAS" or "INSTALLMENTS" => "Cuotas",
                "FINANCIADO" or "FINANCED" => "Financiado",
                _ => "Tarjeta de Crédito" 
            };
        }

        private int CalcularConfianza(string textoExtraido, string? textoMapeado)
        {
            if (string.IsNullOrEmpty(textoExtraido) || string.IsNullOrEmpty(textoMapeado))
                return 0;

            var texto1 = textoExtraido.ToUpperInvariant().Trim();
            var texto2 = textoMapeado.ToUpperInvariant().Trim();

            if (texto1 == texto2) return 100;

            if (texto1.Contains(texto2) || texto2.Contains(texto1)) return 85;

            var similarity = CalculateSimilarity(texto1, texto2);
            return (int)(similarity * 100);
        }

        private double CalculateSimilarity(string str1, string str2)
        {
            var longer = str1.Length > str2.Length ? str1 : str2;
            var shorter = str1.Length > str2.Length ? str2 : str1;

            if (longer.Length == 0) return 1.0;

            var distance = LevenshteinDistance(longer, shorter);
            return (longer.Length - distance) / (double)longer.Length;
        }

        private int LevenshteinDistance(string str1, string str2)
        {
            var matrix = new int[str1.Length + 1, str2.Length + 1];

            for (int i = 0; i <= str1.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= str2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= str1.Length; i++)
            {
                for (int j = 1; j <= str2.Length; j++)
                {
                    var cost = str1[i - 1] == str2[j - 1] ? 0 : 1;
                    matrix[i, j] = Math.Min(Math.Min(
                        matrix[i - 1, j] + 1,      
                        matrix[i, j - 1] + 1),     
                        matrix[i - 1, j - 1] + cost); 
                }
            }

            return matrix[str1.Length, str2.Length];
        }

        private void CalcularMetricasMapeo(PolicyMappingResultDto result)
        {
            var totalCampos = result.CamposMapeados.Count;
            var camposExitosos = result.CamposMapeados.Values.Count(c => c.Confianza > 70);

            result.PorcentajeExito = totalCampos > 0 ? (camposExitosos * 100) / totalCampos : 0;
            result.CamposConAltaConfianza = result.CamposMapeados.Values.Count(c => c.Confianza >= 90);
            result.CamposConMediaConfianza = result.CamposMapeados.Values.Count(c => c.Confianza >= 70 && c.Confianza < 90);
            result.CamposConBajaConfianza = result.CamposMapeados.Values.Count(c => c.Confianza < 70);
        }

        private static MonedaDto? BuscarMonedaPorTexto(IEnumerable<MonedaDto> monedas, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            var exacta = monedas.FirstOrDefault(m => m.Nombre?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            var exactaCodigo = monedas.FirstOrDefault(m => m.Codigo?.ToUpperInvariant() == textoUpper);
            if (exactaCodigo != null) return exactaCodigo;

            var exactaSimbolo = monedas.FirstOrDefault(m => m.Simbolo?.ToUpperInvariant() == textoUpper);
            if (exactaSimbolo != null) return exactaSimbolo;

            return textoUpper switch
            {
                "UYU" or "PESOS" or "PESO URUGUAYO" or "URUGUAYOS" or "$U" or "PES" =>
                    monedas.FirstOrDefault(m => m.Codigo?.ToUpperInvariant() == "PES" || m.Nombre?.ToUpperInvariant().Contains("PESO") == true),

                "USD" or "DOLARES" or "DOLLAR" or "DOLAR" or "$" or "DOL" =>
                    monedas.FirstOrDefault(m => m.Codigo?.ToUpperInvariant() == "DOL" || m.Nombre?.ToUpperInvariant().Contains("DOLAR") == true),

                "EUR" or "EUROS" or "EURO" or "€" or "EU" =>
                    monedas.FirstOrDefault(m => m.Codigo?.ToUpperInvariant() == "EU" || m.Nombre?.ToUpperInvariant().Contains("EURO") == true),

                "BRL" or "REAL" or "REALES" or "R$" or "RS" =>
                    monedas.FirstOrDefault(m => m.Codigo?.ToUpperInvariant() == "RS" || m.Nombre?.ToUpperInvariant().Contains("REAL") == true),

                "UF" or "UNIDAD DE FOMENTO" =>
                    monedas.FirstOrDefault(m => m.Codigo?.ToUpperInvariant() == "UF"),

                _ => monedas.FirstOrDefault(m => m.Nombre?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }
        public async Task<int> ObtenerCategoriaIdPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    _logger.LogWarning("ObtenerCategoriaIdPorNombre llamado con nombre vacío");
                    return 20;
                }

                var categorias = await GetAllCategoriasAsync();
                var categoria = BuscarCategoriaPorTexto(categorias, nombre);

                if (categoria != null)
                {
                    _logger.LogInformation("✅ Categoría encontrada: '{Nombre}' -> ID {Id}", nombre, categoria.Id);
                    return categoria.Id;
                }

                _logger.LogWarning("⚠️ Categoría no encontrada: '{Nombre}'. Usando valor por defecto", nombre);
                return 20;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ID de categoría por nombre: {Nombre}", nombre);
                return 20;
            }
        }

        public async Task<int> ObtenerDestinoIdPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    _logger.LogWarning("ObtenerDestinoIdPorNombre llamado con nombre vacío");
                    return 1;
                }

                var destinos = await GetAllDestinosAsync();
                var destino = BuscarDestinoPorTexto(destinos, nombre);

                if (destino != null)
                {
                    _logger.LogInformation("✅ Destino encontrado: '{Nombre}' -> ID {Id}", nombre, destino.Id);
                    return destino.Id;
                }

                _logger.LogWarning("⚠️ Destino no encontrado: '{Nombre}'. Usando valor por defecto", nombre);
                return 1; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ID de destino por nombre: {Nombre}", nombre);
                return 1;
            }
        }

        public async Task<int> ObtenerCalidadIdPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    _logger.LogWarning("ObtenerCalidadIdPorNombre llamado con nombre vacío");
                    return 2;
                }

                var calidades = await GetAllCalidadesAsync();
                var calidad = BuscarCalidadPorTexto(calidades, nombre);

                if (calidad != null)
                {
                    _logger.LogInformation("✅ Calidad encontrada: '{Nombre}' -> ID {Id}", nombre, calidad.Id);
                    return calidad.Id;
                }

                _logger.LogWarning("⚠️ Calidad no encontrada: '{Nombre}'. Usando valor por defecto", nombre);
                return 2;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ID de calidad por nombre: {Nombre}", nombre);
                return 2; 
            }
        }
        public async Task<VelneoCorredor?> BuscarCorredorPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    _logger.LogWarning("BuscarCorredorPorNombre llamado con nombre vacío");
                    return null;
                }

                // TODO: Implementar búsqueda real de corredores
                // Por ahora retornamos un corredor mock
                _logger.LogWarning("⚠️ BuscarCorredorPorNombre no está completamente implementado. Nombre: {Nombre}", nombre);

                // Mapeo básico de corredores conocidos
                var corredorId = nombre.ToUpperInvariant() switch
                {
                    "ZABARI S A" or "ZABARI" => 14277,
                    "SURA" => 2,
                    "PORTO" => 3,
                    _ => 2 
                };

                return new VelneoCorredor
                {
                    Id = corredorId,
                    Corrnom = nombre
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando corredor por nombre: {Nombre}", nombre);
                return null;
            }
        }

        public async Task<VelneoCorredor?> ObtenerCorredorPorId(int id)
        {
            try
            {
                // TODO: Implementar búsqueda real de corredores por ID
                _logger.LogWarning("⚠️ ObtenerCorredorPorId no está completamente implementado. ID: {Id}", id);

                // Por ahora retornamos un corredor mock
                var nombreCorredor = id switch
                {
                    14277 => "ZABARI S A",
                    2 => "SURA",
                    3 => "PORTO",
                    _ => $"Corredor {id}"
                };

                return new VelneoCorredor
                {
                    Id = id,
                    Corrnom = nombreCorredor
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo corredor por ID: {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<VelneoCorredor>> GetAllCorredoresAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting ALL corredores from Velneo API for tenant {TenantId}", tenantId);

                var allCorredores = new List<VelneoCorredor>();
                var pageNumber = 1;
                var pageSize = 500;
                var hasMoreData = true;

                while (hasMoreData)
                {
                    _logger.LogDebug("Obteniendo página {Page} de corredores...", pageNumber);

                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                    var url = await _httpService.BuildVelneoUrlAsync($"v1/corredores?page={pageNumber}&limit={pageSize}");
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Error obteniendo página {Page} de corredores: {Status}", pageNumber, response.StatusCode);
                        break;
                    }

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var velneoResponse = System.Text.Json.JsonSerializer.Deserialize<VelneoCorredoresResponse>(jsonContent, GetJsonOptions());

                    if (velneoResponse?.Corredores != null && velneoResponse.Corredores.Any())
                    {
                        allCorredores.AddRange(velneoResponse.Corredores);

                        _logger.LogDebug("✅ Página {Page}: {Count} corredores obtenidos (Total acumulado: {Total})",
                            pageNumber, velneoResponse.Corredores.Count, allCorredores.Count);

                        hasMoreData = velneoResponse.HasMoreData == true || velneoResponse.Corredores.Count >= pageSize;
                        pageNumber++;
                    }
                    else
                    {
                        hasMoreData = false;
                    }
                }

                _logger.LogInformation("✅ Successfully retrieved {Count} corredores from Velneo API", allCorredores.Count);
                return allCorredores;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting corredores from Velneo API");
                throw;
            }
        }

        public async Task<VelneoCorredor?> GetCorredorByIdAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting corredor {CorredorId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/corredores/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Corredor {CorredorId} not found in Velneo API", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var velneoCorredor = await _httpService.DeserializeResponseAsync<VelneoCorredor>(response);
                if (velneoCorredor != null && velneoCorredor.Id > 0)
                {
                    _logger.LogInformation("✅ Successfully retrieved corredor {CorredorId}", id);
                    return velneoCorredor;
                }

                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoCorredorResponse>(response);
                if (velneoResponse?.Corredor != null)
                {
                    _logger.LogInformation("✅ Successfully retrieved corredor {CorredorId} (wrapper format)", id);
                    return velneoResponse.Corredor;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting corredor {CorredorId} from Velneo API", id);
                throw;
            }
        }

        public async Task<VelneoCorredor> CreateCorredorAsync(VelneoCorredor corredor)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🚀 Creating corredor '{Nombre}' in Velneo for tenant {TenantId}",
                    corredor.Corrnom, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();

                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(corredor, GetJsonOptions());
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                var url = await _httpService.BuildVelneoUrlAsync("v1/corredores");
                var response = await httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("❌ Error creating corredor: {Status} - {Error}",
                        response.StatusCode, errorContent);
                    throw new ApplicationException($"Error creating corredor: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var createdCorredor = System.Text.Json.JsonSerializer.Deserialize<VelneoCorredor>(responseContent, GetJsonOptions());
                if (createdCorredor != null)
                {
                    _logger.LogInformation("✅ Corredor created successfully with ID {Id}", createdCorredor.Id);
                    return createdCorredor;
                }

                throw new ApplicationException("Unable to deserialize created corredor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating corredor '{Nombre}'", corredor.Corrnom);
                throw;
            }
        }

        public async Task<IEnumerable<VelneoCorredor>> SearchCorredoresAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new List<VelneoCorredor>();

                _logger.LogInformation("🔍 Searching corredores with term '{SearchTerm}'", searchTerm);

                var allCorredores = await GetAllCorredoresAsync();
                var searchUpper = searchTerm.ToUpperInvariant();

                var filtered = allCorredores.Where(c =>
                    c.Corrnom?.ToUpperInvariant().Contains(searchUpper) == true ||
                    c.Corremail?.ToUpperInvariant().Contains(searchUpper) == true ||
                    c.Rut?.Contains(searchTerm) == true ||
                    c.Cod_corr?.Contains(searchTerm) == true
                ).ToList();

                _logger.LogInformation("Found {Count} corredores matching '{SearchTerm}'", filtered.Count, searchTerm);
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching corredores with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }
    }
}