using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI
{
    public class TenantAwareVelneoApiService : IVelneoApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantAwareVelneoApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public TenantAwareVelneoApiService(
            IHttpClientFactory httpClientFactory,
            ITenantService tenantService,
            ILogger<TenantAwareVelneoApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tenantService = tenantService;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
        }

        private async Task<HttpClient> GetConfiguredHttpClientAsync()
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            _logger.LogDebug("Creating HttpClient for tenant {TenantId} with BaseUrl: {BaseUrl}",
                tenantConfig.TenantId, tenantConfig.BaseUrl);

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(tenantConfig.TimeoutSeconds);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RegularizadorPolizas-API/1.0");

            _logger.LogInformation("HttpClient configured for tenant {TenantId}: {BaseUrl} (Timeout: {Timeout}s)",
                tenantConfig.TenantId, tenantConfig.BaseUrl, tenantConfig.TimeoutSeconds);

            return httpClient;
        }

        private async Task<string> BuildVelneoUrlAsync(string endpoint)
        {
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
            var baseUrl = tenantConfig.BaseUrl.TrimEnd('/');
            var separator = endpoint.Contains('?') ? "&" : "?";
            var fullUrl = $"{baseUrl}/{endpoint}{separator}api_key={tenantConfig.Key}";

            _logger.LogDebug("Built Velneo URL: {Url}", fullUrl);
            return fullUrl;
        }

        // ✅ Método helper para deserialización robusta - SIN ReadFromJsonAsync
        private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            var jsonContent = string.Empty;
            try
            {
                jsonContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("Empty JSON response from Velneo API");
                    return null;
                }

                _logger.LogDebug("JSON Response length: {Length} chars", jsonContent.Length);

                // ✅ Solo System.Text.Json - sin ambigüedad
                return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error in Velneo API response. Content preview: {Content}",
                    jsonContent?.Substring(0, Math.Min(500, jsonContent.Length)));
                throw new ApplicationException($"Error parsing Velneo API response: {ex.Message}", ex);
            }
        }

        #region Métodos de Clientes
        public async Task<ClientDto> GetClienteAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting cliente {ClienteId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync($"v1/clientes/{id}");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ Intentar primero como objeto directo
                var velneoCliente = await DeserializeResponseAsync<VelneoCliente>(response);
                if (velneoCliente != null)
                {
                    var result = velneoCliente.ToClienteDto();
                    _logger.LogInformation("Successfully retrieved cliente {ClienteId} from Velneo API", id);
                    return result;
                }

                // ✅ Si falla, intentar como wrapper - hacer nueva llamada
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoResponse = await DeserializeResponseAsync<VelneoClientResponse>(response);
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

        public async Task<IEnumerable<ClientDto>> GetClientesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 INICIO: Getting ALL clientes from Velneo API for tenant {TenantId}", tenantId);

                var allClientes = new List<ClientDto>();
                var pageNumber = 1;
                var pageSize = 1000;
                var hasMoreData = true;

                while (hasMoreData)
                {
                    _logger.LogInformation("Obteniendo página {Page}...", pageNumber);

                    using var httpClient = await GetConfiguredHttpClientAsync();
                    var url = await BuildVelneoUrlAsync($"v1/clientes?page={pageNumber}&limit={pageSize}");
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var velneoResponse = await DeserializeResponseAsync<VelneoClientesResponse>(response);

                    if (velneoResponse?.Clientes != null && velneoResponse.Clientes.Any())
                    {
                        var clientesPage = velneoResponse.Clientes.ToClienteDtos().ToList();
                        allClientes.AddRange(clientesPage);

                        _logger.LogInformation("✅ Página {Page}: {Count} clientes obtenidos (Total acumulado: {Total})",
                            pageNumber, clientesPage.Count, allClientes.Count);

                        hasMoreData = velneoResponse.HasMoreData == true;
                        pageNumber++;
                    }
                    else
                    {
                        hasMoreData = false;
                        _logger.LogInformation("No hay más datos en página {Page}", pageNumber);
                    }
                }

                _logger.LogInformation("🎯 FINAL: Successfully retrieved {Count} clientes total from Velneo API", allClientes.Count);
                return allClientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clientes from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm)
        {
            try
            {
                var allClientes = await GetClientesAsync();
                var filtered = allClientes.Where(c =>
                    c.Clinom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Cliced?.Contains(searchTerm) == true ||
                    c.Cliruc?.Contains(searchTerm) == true
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

        // ✅ Métodos auxiliares
        private int EstimateTotalCount(int currentPageCount, int currentPage, int pageSize)
        {
            // Si la página actual tiene menos elementos que el pageSize, probablemente es la última
            if (currentPageCount < pageSize)
            {
                return ((currentPage - 1) * pageSize) + currentPageCount;
            }

            // Si tiene el máximo, estimamos que hay al menos una página más
            return currentPage * pageSize + 1; // Estimación conservadora
        }

        public async Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(int page = 1, int pageSize = 50, string? search = null)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 PAGINACIÓN REAL: Getting clients page {Page} (size: {PageSize}) from Velneo API for tenant {TenantId}",
                    page, pageSize, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // ✅ Construir URL con paginación real de Velneo
                var endpoint = $"v1/clientes?page[number]={page}&page[size]={pageSize}";

                // ✅ Agregar búsqueda si existe (por ahora comentado hasta saber el formato exacto)
                if (!string.IsNullOrWhiteSpace(search))
                {
                    // TODO: Investigar cómo Velneo maneja búsquedas en el endpoint
                    // endpoint += $"&search={Uri.EscapeDataString(search)}";
                    _logger.LogInformation("🔍 Search requested but not yet implemented in Velneo endpoint: {Search}", search);
                }

                var url = await BuildVelneoUrlAsync(endpoint);
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                var maskedUrl = url.Replace(tenantConfig.Key, "***");
                _logger.LogInformation("🌐 Velneo URL: {Url}", maskedUrl);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("📡 Velneo response - Page {Page}: Status {Status}, JSON length: {Length} chars",
                    page, response.StatusCode, jsonContent.Length);

                // ✅ Deserializar respuesta de Velneo
                List<ClientDto> clientsPage = new List<ClientDto>();
                int totalCount = 0;
                bool hasMoreData = false;

                try
                {
                    // Intentar deserializar como array directo (formato actual de Velneo)
                    var velneoClientes = JsonSerializer.Deserialize<List<VelneoCliente>>(jsonContent, _jsonOptions);
                    if (velneoClientes != null && velneoClientes.Any())
                    {
                        clientsPage = velneoClientes.Select(vc => vc.ToClienteDto()).ToList();

                        // ✅ Verificar si hay headers con información total
                        if (response.Headers.Contains("X-Total-Count"))
                        {
                            var totalHeader = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
                            if (int.TryParse(totalHeader, out int headerTotal))
                            {
                                totalCount = headerTotal;
                                _logger.LogInformation("📊 Total count from header: {Total}", totalCount);
                            }
                        }

                        // Si no hay header, estimamos basado en la respuesta
                        if (totalCount == 0)
                        {
                            totalCount = EstimateTotalCount(clientsPage.Count, page, pageSize);
                            _logger.LogInformation("📊 Estimated total count: {Total}", totalCount);
                        }

                        hasMoreData = clientsPage.Count == pageSize; // Si devolvió el máximo, probablemente hay más

                        _logger.LogInformation("✅ Deserialized {Count} clients from Velneo page {Page}", clientsPage.Count, page);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Empty or null response from Velneo for page {Page}", page);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("⚠️ Error deserializing direct array, trying wrapped response: {Error}", ex.Message);

                    // Intentar con wrapper si el formato es diferente
                    try
                    {
                        var velneoResponse = JsonSerializer.Deserialize<VelneoClientesResponse>(jsonContent, _jsonOptions);
                        if (velneoResponse?.Clientes != null)
                        {
                            clientsPage = velneoResponse.Clientes.Select(vc => vc.ToClienteDto()).ToList();
                            totalCount = velneoResponse.TotalCount ?? EstimateTotalCount(clientsPage.Count, page, pageSize);
                            hasMoreData = velneoResponse.HasMoreData ?? (clientsPage.Count == pageSize);
                            _logger.LogInformation("✅ Used wrapped response format");
                        }
                    }
                    catch (JsonException ex2)
                    {
                        _logger.LogError(ex2, "❌ Failed to deserialize Velneo response in any known format");
                        throw;
                    }
                }

                stopwatch.Stop();

                // ✅ Aplicar filtro de búsqueda local si Velneo no lo soporta nativamente
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var originalCount = clientsPage.Count;
                    clientsPage = clientsPage.Where(c =>
                        (c.Clinom?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliemail?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliced?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliruc?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                    ).ToList();

                    _logger.LogInformation("🔍 Client-side search filter applied: {FilteredCount} of {OriginalCount}",
                        clientsPage.Count, originalCount);
                }

                var result = new PaginatedVelneoResponse<ClientDto>
                {
                    Items = clientsPage,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    VelneoHasMoreData = hasMoreData,
                    RequestDuration = stopwatch.Elapsed
                };

                _logger.LogInformation("✅ PAGINACIÓN REAL COMPLETADA: Page {Page}/{EstimatedTotal} - {Count} clients retrieved in {Duration}ms",
                    page, result.TotalPages, clientsPage.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ ERROR en GetClientesPaginatedAsync - Page: {Page}, PageSize: {PageSize}, Duration: {Duration}ms",
                    page, pageSize, stopwatch.ElapsedMilliseconds);
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

                // ✅ OBTENER CONFIGURACIÓN DEL TENANT DINÁMICAMENTE
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                if (tenantConfig == null)
                {
                    _logger.LogError("❌ No se pudo obtener configuración del tenant");
                    return new List<ClientDto>();
                }

                // ✅ CREAR HTTPCLIENT USANDO FACTORY
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                // ✅ CONSTRUIR URL CON FILTRO DIRECTO
                var filterUrl = $"{tenantConfig.BaseUrl}/v1/clientes?filter[nombre]={Uri.EscapeDataString(searchTerm)}&api_key={tenantConfig.Key}";

                _logger.LogInformation("📤 URL filtrada: {FilterUrl}", filterUrl.Replace(tenantConfig.Key, "***"));

                var response = await httpClient.GetAsync(filterUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Error en búsqueda directa: {StatusCode}", response.StatusCode);
                    return new List<ClientDto>();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("📥 JSON Response length: {Length}", jsonResponse.Length);

                // ✅ PARSEAR JSON CON LA ESTRUCTURA REAL DE VELNEO
                using var jsonDocument = JsonDocument.Parse(jsonResponse);
                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty("clientes", out var clientesElement))
                {
                    _logger.LogWarning("⚠️ No se encontró 'clientes' en respuesta de Velneo para: {SearchTerm}", searchTerm);
                    return new List<ClientDto>();
                }

                var clients = new List<ClientDto>();

                foreach (var clientElement in clientesElement.EnumerateArray())
                {
                    try
                    {
                        var clientDto = new ClientDto();

                        // ✅ EXTRAER ID 
                        if (clientElement.TryGetProperty("id", out var idElement))
                        {
                            clientDto.Id = idElement.GetInt32();
                        }

                        // ✅ CAMPOS DIRECTOS EN EL OBJETO (NO EN ATTRIBUTES)
                        if (clientElement.TryGetProperty("clinom", out var clinomElement))
                            clientDto.Clinom = clinomElement.GetString() ?? "";

                        if (clientElement.TryGetProperty("cliced", out var clicedElement))
                            clientDto.Cliced = clicedElement.GetString() ?? "";

                        if (clientElement.TryGetProperty("cliruc", out var clirucElement))
                            clientDto.Cliruc = clirucElement.GetString() ?? "";

                        if (clientElement.TryGetProperty("cliemail", out var cliemailElement))
                            clientDto.Cliemail = cliemailElement.GetString() ?? "";

                        if (clientElement.TryGetProperty("clidir", out var clidirElement))
                            clientDto.Clidir = clidirElement.GetString() ?? "";

                        if (clientElement.TryGetProperty("telefono", out var telefonoElement))
                            clientDto.Telefono = telefonoElement.GetString() ?? "";

                        // Activo por defecto ya que están en la respuesta
                        clientDto.Activo = true;

                        // ✅ SOLO AGREGAR SI TIENE DATOS MÍNIMOS
                        if (!string.IsNullOrEmpty(clientDto.Clinom) && clientDto.Id > 0)
                        {
                            clients.Add(clientDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Error parseando cliente individual en respuesta de Velneo");
                        continue;
                    }
                }

                // ✅ EXTRAER TOTAL DEL COUNT
                var totalCount = 0;
                if (root.TryGetProperty("total_count", out var totalCountElement))
                {
                    totalCount = totalCountElement.GetInt32();
                }
                else if (root.TryGetProperty("count", out var countElement))
                {
                    totalCount = countElement.GetInt32();
                }

                _logger.LogInformation("✅ BÚSQUEDA DIRECTA EXITOSA: {Count} clientes encontrados (total: {Total})",
                    clients.Count, totalCount);

                return clients;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "❌ Error parseando JSON de Velneo: {SearchTerm}", searchTerm);
                return new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda directa de clientes: {SearchTerm}", searchTerm);
                return new List<ClientDto>();
            }
        }

        public class VelneoClientesResponse
        {
            public List<VelneoCliente>? Clientes { get; set; }
            public int? TotalCount { get; set; }
            public bool? HasMoreData { get; set; }
            public int? CurrentPage { get; set; }
        }
        #endregion

        #region Métodos de Compañías

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all companies from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/companias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (que es lo que Velneo está devolviendo)
                var velneoResponse = await DeserializeResponseAsync<VelneoCompaniesResponse>(response);
                if (velneoResponse?.Companias != null && velneoResponse.Companias.Any())
                {
                    var companies = velneoResponse.Companias.ToCompanyDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (wrapper format)", companies.Count);
                    return companies;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoCompanies = await DeserializeResponseAsync<List<VelneoCompany>>(response);
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

        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                var companies = await GetAllCompaniesAsync();
                var lookupDtos = companies.Select(c => new CompanyLookupDto
                {
                    Id = c.Id,
                    Comnom = c.Comnom,
                    Comalias = c.Comalias,
                    Cod_srvcompanias = c.Cod_srvcompanias
                });

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
                var allCompanies = await GetAllCompaniesAsync();
                var filtered = allCompanies.Where(c =>
                    c.Comnom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Comalias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching companies");
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            try
            {
                var companies = await GetAllCompaniesAsync();
                return companies.FirstOrDefault(c =>
                    string.Equals(c.Comalias, alias, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by alias from Velneo API");
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting company {CompanyId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync($"v1/companias/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Company {CompanyId} not found in Velneo API", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                // ✅ INTENTAR PRIMERO COMO OBJETO DIRECTO
                var velneoCompany = await DeserializeResponseAsync<VelneoCompany>(response);
                if (velneoCompany != null)
                {
                    var result = velneoCompany.ToCompanyDto();
                    _logger.LogInformation("Successfully retrieved company {CompanyId} from Velneo API", id);
                    return result;
                }

                // ✅ SI FALLA, INTENTAR COMO WRAPPER - hacer nueva llamada
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoResponse = await DeserializeResponseAsync<VelneoCompanyResponse>(response);
                if (velneoResponse?.Compania != null)
                {
                    var result = velneoResponse.Compania.ToCompanyDto();
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
                _logger.LogDebug("Getting company by codigo {Codigo} from Velneo API for tenant {TenantId}", codigo, tenantId);

                // ✅ OPCIÓN 1: Si Velneo tiene endpoint específico para búsqueda por código
                try
                {
                    using var httpClient = await GetConfiguredHttpClientAsync();
                    var url = await BuildVelneoUrlAsync($"v1/companias/codigo/{Uri.EscapeDataString(codigo)}");
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var velneoCompany = await DeserializeResponseAsync<VelneoCompany>(response);
                        if (velneoCompany != null)
                        {
                            var result = velneoCompany.ToCompanyDto();
                            _logger.LogInformation("Successfully retrieved company by codigo {Codigo} from Velneo API", codigo);
                            return result;
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    // Continuar con búsqueda en la lista completa
                    _logger.LogDebug("Direct endpoint not found, searching in full list for codigo {Codigo}", codigo);
                }

                // ✅ OPCIÓN 2: FALLBACK - Buscar en la lista completa de compañías
                var companies = await GetAllCompaniesAsync();
                var company = companies.FirstOrDefault(c =>
                    string.Equals(c.Cod_srvcompanias, codigo, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.Codigo, codigo, StringComparison.OrdinalIgnoreCase));

                if (company != null)
                {
                    _logger.LogInformation("Successfully found company by codigo {Codigo} in full list", codigo);
                    return company;
                }

                _logger.LogWarning("Company with codigo {Codigo} not found in Velneo API", codigo);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by codigo {Codigo} from Velneo API", codigo);
                throw;
            }
        }

        #endregion

        #region Métodos de Contratos/Pólizas

        public async Task<PolizaDto> GetPolizaAsync(int id)
        {
            try
            {
                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync($"v1/contratos/{id}");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Contrato response: {Response}", jsonContent.Substring(0, Math.Min(200, jsonContent.Length)));

                throw new NotImplementedException("Mapeo de contratos a pólizas pendiente de implementación");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza {PolizaId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 INICIO: Getting ALL polizas/contratos from Velneo API for tenant {TenantId}", tenantId);
                var allPolizas = new List<PolizaDto>();
                var pageNumber = 1;
                var pageSize = 1000;
                var hasMoreData = true;

                while (hasMoreData)
                {
                    _logger.LogInformation("📄 Obteniendo página {Page} de contratos...", pageNumber);

                    using var httpClient = await GetConfiguredHttpClientAsync();
                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    var url = await BuildVelneoUrlAsync($"v1/contratos?page[number]={pageNumber}&page[size]={pageSize}");
                    _logger.LogInformation("🌐 URL página {Page}: {Url}", pageNumber, url);

                    var response = await httpClient.GetAsync(url);
                    _logger.LogInformation("📡 Respuesta página {Page}: Status {StatusCode}", pageNumber, response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("📄 JSON página {Page} - Length: {Length} caracteres", pageNumber, jsonContent.Length);
                    _logger.LogInformation("🔄 Deserializando página {Page}...", pageNumber);

                    // ✅ CAMBIO: Usar JsonSerializer.Deserialize en lugar de ReadFromJsonAsync
                    var velneoResponse = JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, _jsonOptions);

                    if (velneoResponse?.Polizas != null && velneoResponse.Polizas.Any())
                    {
                        _logger.LogInformation("✅ Página {Page} - Count: {Count}, Total en DB: {Total}",
                            pageNumber, velneoResponse.Polizas.Count, velneoResponse.TotalCount);

                        var polizasPage = velneoResponse.Polizas.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        _logger.LogInformation("🗺️ Mapeados {Count} contratos de página {Page}. Total acumulado: {TotalAccumulated}",
                            polizasPage.Count, pageNumber, allPolizas.Count);

                        hasMoreData = velneoResponse.Polizas.Count == pageSize && allPolizas.Count < velneoResponse.TotalCount;

                        if (hasMoreData)
                        {
                            pageNumber++;
                            _logger.LogInformation("➡️ Hay más datos. Continuando con página {NextPage}", pageNumber);
                        }
                        else
                        {
                            _logger.LogInformation("🏁 No hay más páginas. Proceso completado.");
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
                _logger.LogError(ex, "❌ ERROR en GetPolizasAsync con paginación: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("INICIO: Getting polizas for client {ClienteId} from Velneo API for tenant {TenantId}", clienteId, tenantId);
                var allPolizas = new List<PolizaDto>();
                var pageNumber = 1;
                var pageSize = 1000;
                var hasMoreData = true;

                while (hasMoreData)
                {
                    _logger.LogInformation("Obteniendo página {Page} de contratos para cliente {ClienteId}...", pageNumber, clienteId);

                    using var httpClient = await GetConfiguredHttpClientAsync();
                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    var url = await BuildVelneoUrlAsync($"v1/contratos?filter[clientes]={clienteId}&page[number]={pageNumber}&page[size]={pageSize}");
                    _logger.LogInformation("URL página {Page}: {Url}", pageNumber, url);

                    var response = await httpClient.GetAsync(url);
                    _logger.LogInformation("Respuesta página {Page}: Status {StatusCode}", pageNumber, response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("JSON página {Page} - Length: {Length} caracteres", pageNumber, jsonContent.Length);
                    _logger.LogInformation("Deserializando página {Page}...", pageNumber);

                    // ✅ CAMBIO: Usar JsonSerializer.Deserialize en lugar de ReadFromJsonAsync
                    var velneoResponse = JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, _jsonOptions);

                    if (velneoResponse?.Polizas != null && velneoResponse.Polizas.Any())
                    {
                        _logger.LogInformation("Página {Page} - Count: {Count}, Total en DB: {Total}",
                            pageNumber, velneoResponse.Polizas.Count, velneoResponse.TotalCount);

                        var polizasPage = velneoResponse.Polizas.ToPolizaDtos().ToList();
                        allPolizas.AddRange(polizasPage);

                        _logger.LogInformation("Mapeados {Count} contratos de página {Page}. Total acumulado: {TotalAccumulated}",
                            polizasPage.Count, pageNumber, allPolizas.Count);

                        hasMoreData = velneoResponse.Polizas.Count == pageSize && allPolizas.Count < velneoResponse.TotalCount;

                        if (hasMoreData)
                        {
                            pageNumber++;
                            _logger.LogInformation("Hay más datos. Continuando con página {NextPage}", pageNumber);
                        }
                        else
                        {
                            _logger.LogInformation("No hay más páginas. Proceso completado.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("⚠️Página {Page} vacía. Finalizando paginación.", pageNumber);
                        hasMoreData = false;
                    }
                }

                _logger.LogInformation("COMPLETADO: {TotalRetrieved} contratos obtenidos para cliente {ClienteId}", allPolizas.Count, clienteId);
                return allPolizas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR en GetPolizasByClientAsync para cliente {ClienteId}: {Message}", clienteId, ex.Message);
                throw;
            }
        }

        public async Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasByClientPaginatedAsync(
            int clienteId,
            int page = 1,
            int pageSize = 25,
            string? search = null)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 PAGINACIÓN PÓLIZAS POR CLIENTE: Getting polizas for client {ClienteId}, page {Page} (size: {PageSize}) for tenant {TenantId}",
                    clienteId, page, pageSize, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // ✅ URL con filtro por cliente específico y paginación
                var endpoint = $"v1/contratos?filter[clientes]={clienteId}&page[number]={page}&page[size]={pageSize}";

                // ✅ Agregar búsqueda si existe
                if (!string.IsNullOrWhiteSpace(search))
                {
                    // TODO: Investigar si Velneo soporta search + filter combinados
                    _logger.LogInformation("🔍 Search requested for client polizas: {Search}", search);
                }

                var url = await BuildVelneoUrlAsync(endpoint);
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                var maskedUrl = url.Replace(tenantConfig.Key, "***");
                _logger.LogInformation("🌐 Velneo Client Pólizas URL: {Url}", maskedUrl);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("📡 Velneo client pólizas response - Client: {ClienteId}, Page {Page}: Status {Status}, JSON length: {Length} chars",
                    clienteId, page, response.StatusCode, jsonContent.Length);

                // ✅ Deserializar respuesta
                List<PolizaDto> polizasPage = new List<PolizaDto>();
                int totalCount = 0;
                bool hasMoreData = false;

                try
                {
                    // Usar el formato existente que ya funciona
                    var velneoResponse = JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, _jsonOptions);

                    if (velneoResponse?.Polizas != null && velneoResponse.Polizas.Any())
                    {
                        polizasPage = velneoResponse.Polizas.ToPolizaDtos().ToList();

                        // ✅ Usar el total count del response
                        totalCount = velneoResponse.TotalCount.GetValueOrDefault(0);

                        // Si no hay total en el response, estimamos
                        if (totalCount == 0)
                        {
                            totalCount = EstimateTotalCount(polizasPage.Count, page, pageSize);
                            _logger.LogInformation("📊 Estimated total pólizas for client {ClienteId}: {Total}", clienteId, totalCount);
                        }
                        else
                        {
                            _logger.LogInformation("📊 Total pólizas for client {ClienteId} from Velneo: {Total}", clienteId, totalCount);
                        }

                        hasMoreData = polizasPage.Count == pageSize && page * pageSize < totalCount;

                        _logger.LogInformation("✅ Retrieved {Count} pólizas for client {ClienteId} from page {Page}",
                            polizasPage.Count, clienteId, page);
                    }
                    else
                    {
                        _logger.LogInformation("ℹ️ Client {ClienteId} has no pólizas on page {Page}", clienteId, page);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "❌ Failed to deserialize Velneo client pólizas response for client {ClienteId}", clienteId);
                    throw;
                }

                stopwatch.Stop();

                // ✅ Aplicar filtro de búsqueda local si es necesario
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var originalCount = polizasPage.Count;
                    polizasPage = polizasPage.Where(p =>
                        (p.Conpol?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Ramo?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Com_alias?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Contpocob?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) // Tipo de cobertura
                    ).ToList();

                    _logger.LogInformation("🔍 Search filter applied to client {ClienteId} pólizas: {FilteredCount} of {OriginalCount}",
                        clienteId, polizasPage.Count, originalCount);
                }

                var result = new PaginatedVelneoResponse<PolizaDto>
                {
                    Items = polizasPage,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    VelneoHasMoreData = hasMoreData,
                    RequestDuration = stopwatch.Elapsed
                };

                _logger.LogInformation("✅ PAGINACIÓN PÓLIZAS POR CLIENTE COMPLETADA: Client {ClienteId}, Page {Page}/{TotalPages} - {Count} pólizas in {Duration}ms",
                    clienteId, page, result.TotalPages, polizasPage.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ ERROR en GetPolizasByClientPaginatedAsync - Client: {ClienteId}, Page: {Page}, PageSize: {PageSize}, Duration: {Duration}ms",
                    clienteId, page, pageSize, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasPaginatedAsync(int page = 1, int pageSize = 50, string? search = null)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 PAGINACIÓN REAL PÓLIZAS: Getting polizas page {Page} (size: {PageSize}) from Velneo API for tenant {TenantId}",
                    page, pageSize, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // ✅ Construir URL con paginación real de Velneo para pólizas
                // Ajustar el endpoint según cómo se llamen las pólizas en Velneo API
                var endpoint = $"v1/contratos?page[number]={page}&page[size]={pageSize}";
                // O podría ser: $"v1/polizas?page[number]={page}&page[size]={pageSize}";

                // ✅ Agregar búsqueda si existe
                if (!string.IsNullOrWhiteSpace(search))
                {
                    // TODO: Investigar cómo Velneo maneja búsquedas de pólizas
                    _logger.LogInformation("🔍 Search requested for polizas but not yet implemented in Velneo endpoint: {Search}", search);
                }

                var url = await BuildVelneoUrlAsync(endpoint);
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                var maskedUrl = url.Replace(tenantConfig.Key, "***");
                _logger.LogInformation("🌐 Velneo Pólizas URL: {Url}", maskedUrl);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("📡 Velneo pólizas response - Page {Page}: Status {Status}, JSON length: {Length} chars",
                    page, response.StatusCode, jsonContent.Length);

                // ✅ Deserializar respuesta de Velneo
                List<PolizaDto> polizasPage = new List<PolizaDto>();
                int totalCount = 0;
                bool hasMoreData = false;

                try
                {
                    // Intentar deserializar como array directo
                    // NOTA: Ajustar según el modelo real de Velneo para pólizas
                    var velneoPolizas = JsonSerializer.Deserialize<List<VelneoPoliza>>(jsonContent, _jsonOptions);
                    // O podría ser: JsonSerializer.Deserialize<List<VelneoPoliza>>(jsonContent, _jsonOptions);

                    if (velneoPolizas != null && velneoPolizas.Any())
                    {
                        // ✅ Mapear usando el mapper existente (ajustar según tu implementación)
                        polizasPage = velneoPolizas.Select(vp => vp.ToPolizaDto()).ToList();

                        // ✅ Verificar headers para total count
                        if (response.Headers.Contains("X-Total-Count"))
                        {
                            var totalHeader = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
                            if (int.TryParse(totalHeader, out int headerTotal))
                            {
                                totalCount = headerTotal;
                                _logger.LogInformation("📊 Pólizas total count from header: {Total}", totalCount);
                            }
                        }

                        // Si no hay header, estimamos
                        if (totalCount == 0)
                        {
                            totalCount = EstimateTotalCount(polizasPage.Count, page, pageSize);
                            _logger.LogInformation("📊 Estimated pólizas total count: {Total}", totalCount);
                        }

                        hasMoreData = polizasPage.Count == pageSize;

                        _logger.LogInformation("✅ Deserialized {Count} pólizas from Velneo page {Page}", polizasPage.Count, page);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Empty or null pólizas response from Velneo for page {Page}", page);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("⚠️ Error deserializing pólizas direct array, trying wrapped response: {Error}", ex.Message);

                    try
                    {
                        var velneoResponse = JsonSerializer.Deserialize<VelneoPolizasResponse>(jsonContent, _jsonOptions);
                        if (velneoResponse?.Polizas != null)
                        {
                            polizasPage = velneoResponse.Polizas.Select(vp => vp.ToPolizaDto()).ToList();
                            if (velneoResponse.TotalCount.HasValue)
                                totalCount = velneoResponse.TotalCount.Value;
                            else
                                totalCount = EstimateTotalCount(polizasPage.Count, page, pageSize);
  
                            if (velneoResponse.HasMoreData.HasValue)
                                hasMoreData = velneoResponse.HasMoreData.Value;
                            else
                                hasMoreData = polizasPage.Count == pageSize;
                            _logger.LogInformation("✅ Used wrapped pólizas response format");
                        }
                    }
                    catch (JsonException ex2)
                    {
                        _logger.LogError(ex2, "❌ Failed to deserialize Velneo pólizas response in any known format");
                        throw;
                    }
                }

                stopwatch.Stop();

                // ✅ Aplicar filtro de búsqueda local si Velneo no lo soporta nativamente
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var originalCount = polizasPage.Count;
                    polizasPage = polizasPage.Where(p =>
                        // Ajustar campos según el PolizaDto real
                        (p.Conpol?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Ramo?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Com_alias?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                    // Agregar más campos de búsqueda según necesites
                    ).ToList();

                    _logger.LogInformation("🔍 Client-side pólizas search filter applied: {FilteredCount} of {OriginalCount}",
                        polizasPage.Count, originalCount);
                }

                var result = new PaginatedVelneoResponse<PolizaDto>
                {
                    Items = polizasPage,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    VelneoHasMoreData = hasMoreData,
                    RequestDuration = stopwatch.Elapsed
                };

                _logger.LogInformation("✅ PAGINACIÓN REAL PÓLIZAS COMPLETADA: Page {Page}/{EstimatedTotal} - {Count} pólizas retrieved in {Duration}ms",
                    page, result.TotalPages, polizasPage.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ ERROR en GetPolizasPaginatedAsync - Page: {Page}, PageSize: {PageSize}, Duration: {Duration}ms",
                    page, pageSize, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🚀 CREANDO PÓLIZA ENRIQUECIDA EN VELNEO: Número={NumeroPoliza}, Cliente={ClienteId}, Tenant={TenantId}",
                    request.Conpol, request.Clinro, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // ✅ VALIDAR CAMPOS REQUERIDOS ANTES DE ENVIAR
                var validationErrors = new List<string>();

                if (request.Comcod <= 0) validationErrors.Add("comcod debe ser mayor a 0");
                if (request.Clinro <= 0) validationErrors.Add("clinro debe ser mayor a 0");
                if (string.IsNullOrEmpty(request.Conpol)) validationErrors.Add("conpol no puede estar vacío");
                if (string.IsNullOrEmpty(request.Confchdes)) validationErrors.Add("confchdes no puede estar vacío");
                if (string.IsNullOrEmpty(request.Confchhas)) validationErrors.Add("confchhas no puede estar vacío");

                if (validationErrors.Any())
                {
                    var errors = string.Join(", ", validationErrors);
                    _logger.LogError("❌ VALIDACIÓN FALLIDA: {Errors}", errors);
                    throw new ArgumentException($"Datos requeridos faltantes: {errors}");
                }

                _logger.LogInformation("✅ Validación de campos críticos exitosa");

                // ✅ USAR TU MÉTODO EXISTENTE PERO CON DATOS ENRIQUECIDOS
                var velneoContrato = MapearCreateRequestAVelneo(request);

                // Serializar el payload
                var jsonPayload = JsonSerializer.Serialize(velneoContrato, _jsonOptions);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("📤 PAYLOAD VELNEO ENRIQUECIDO: {JsonLength} caracteres para póliza {NumeroPoliza}",
                    jsonPayload.Length, request.Conpol);

                // ⚠️ HABILITAR SOLO PARA DEBUG - PAYLOAD COMPLETO
                _logger.LogWarning("🔍 DEBUG - PAYLOAD COMPLETO ENVIADO A VELNEO:");
                _logger.LogWarning("📄 {JsonPayload}", jsonPayload);

                // ✅ LOGGING DE CAMPOS CRÍTICOS
                _logger.LogInformation("🎯 CAMPOS CRÍTICOS: comcod={Comcod}, clinro={Clinro}, conpol={Conpol}, confchdes={FechaDesde}, confchhas={FechaHasta}",
                    request.Comcod, request.Clinro, request.Conpol, request.Confchdes, request.Confchhas);

                // ✅ REUTILIZAR TU LÓGICA EXISTENTE
                var endpoint = "v1/contratos";
                var url = await BuildVelneoUrlAsync(endpoint);

                // ✅ LOGGING DE HEADERS DE LA REQUEST
                _logger.LogInformation("📨 REQUEST HEADERS:");
                foreach (var header in httpClient.DefaultRequestHeaders)
                {
                    _logger.LogInformation("   {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }
                _logger.LogInformation("   Content-Type: {ContentType}", content.Headers.ContentType);

                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                var maskedUrl = url.Replace(tenantConfig.Key, "***");
                _logger.LogInformation("🌐 POST Velneo URL: {Url}", maskedUrl);

                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                var response = await httpClient.PostAsync(url, content, cts.Token);

                var responseContent = await response.Content.ReadAsStringAsync();
                stopwatch.Stop();

                _logger.LogInformation("📡 Velneo CREATE response: Status {Status}, Duration: {Duration}ms, Content length: {Length}",
                    response.StatusCode, stopwatch.ElapsedMilliseconds, responseContent.Length);

                // ✅ LOGGING DE HEADERS DE LA RESPUESTA
                _logger.LogInformation("📨 RESPONSE HEADERS:");
                foreach (var header in response.Headers)
                {
                    _logger.LogInformation("   {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }

                // ✅ LOGGING DEL CONTENIDO DE LA RESPUESTA (SI NO ESTÁ VACÍO)
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    _logger.LogWarning("📄 RESPONSE CONTENT: {ResponseContent}", responseContent);
                }
                else
                {
                    _logger.LogWarning("📄 RESPONSE CONTENT: <VACÍO>");
                }

                // ✅ LOGGING DE STATUS CODE ESPECÍFICO
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✅ HTTP SUCCESS: {StatusCode} {ReasonPhrase}",
                        (int)response.StatusCode, response.ReasonPhrase);
                }
                else
                {
                    _logger.LogError("❌ HTTP ERROR: {StatusCode} {ReasonPhrase}",
                        (int)response.StatusCode, response.ReasonPhrase);
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Error en Velneo API: Status={Status}, Response={Response}",
                        response.StatusCode, responseContent);

                    var errorMessage = ExtractErrorMessage(responseContent, response.StatusCode);
                    throw new HttpRequestException($"Error creando póliza enriquecida en Velneo: {errorMessage}");
                }

                // ✅ NUEVA VERIFICACIÓN: Confirmar que se guardó realmente
                _logger.LogInformation("🔍 Verificando que la póliza se guardó correctamente en Velneo...");

                // Esperar un poco para que Velneo procese
                await Task.Delay(3000);

                var exists = await VerifyPolizaExistsAsync(request.Conpol);

                if (!exists)
                {
                    _logger.LogError("❌ VERIFICACIÓN FALLIDA: La póliza {NumeroPoliza} NO se encontró en Velneo después de la creación",
                        request.Conpol);
                    throw new InvalidOperationException($"La póliza {request.Conpol} no se guardó correctamente en Velneo");
                }

                _logger.LogInformation("✅ VERIFICACIÓN EXITOSA: Póliza {NumeroPoliza} confirmada en Velneo en {Duration}ms",
                    request.Conpol, stopwatch.ElapsedMilliseconds);

                // ✅ REUTILIZAR TU LÓGICA DE MANEJO DE RESPUESTA VACÍA
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    _logger.LogWarning("⚠️ Velneo retornó respuesta vacía, pero con status 200.");
                    _logger.LogInformation("✅ PÓLIZA ENRIQUECIDA CREADA EN VELNEO: Número={NumeroPoliza} en {Duration}ms (respuesta vacía pero exitosa)",
                        request.Conpol, stopwatch.ElapsedMilliseconds);

                    return new
                    {
                        success = true,
                        message = "Póliza enriquecida creada y verificada exitosamente en Velneo",
                        numeroPoliza = request.Conpol,
                        datosEnviados = GetDatosEnviadosSummary(request),
                        verificada = true,
                        duracionMs = stopwatch.ElapsedMilliseconds
                    };
                }

                // ✅ REUTILIZAR TU LÓGICA DE DESERIALIZACIÓN
                try
                {
                    var velneoResponse = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);

                    _logger.LogInformation("✅ PÓLIZA ENRIQUECIDA CREADA EN VELNEO: Número={NumeroPoliza} en {Duration}ms con respuesta JSON",
                        request.Conpol, stopwatch.ElapsedMilliseconds);

                    return new
                    {
                        success = true,
                        message = "Póliza enriquecida creada y verificada exitosamente en Velneo",
                        numeroPoliza = request.Conpol,
                        velneoResponse = velneoResponse,
                        datosEnviados = GetDatosEnviadosSummary(request),
                        verificada = true,
                        duracionMs = stopwatch.ElapsedMilliseconds
                    };
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogWarning(jsonEx, "⚠️ Error deserializando respuesta JSON, pero status fue 200.");

                    return new
                    {
                        success = true,
                        message = "Póliza enriquecida creada y verificada exitosamente (respuesta no estándar)",
                        numeroPoliza = request.Conpol,
                        rawResponse = responseContent.Substring(0, Math.Min(200, responseContent.Length)),
                        datosEnviados = GetDatosEnviadosSummary(request),
                        verificada = true,
                        duracionMs = stopwatch.ElapsedMilliseconds
                    };
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("⏰ TIMEOUT creando póliza enriquecida {NumeroPoliza} en Velneo después de {Duration}ms",
                    request.Conpol, stopwatch.ElapsedMilliseconds);
                throw new TimeoutException($"Timeout creando póliza enriquecida {request.Conpol} en Velneo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR creando póliza enriquecida {NumeroPoliza} en Velneo",
                    request.Conpol);
                throw;
            }
        }

        // ====================================================
        // TAMBIÉN AGREGAR EL MÉTODO DE VERIFICACIÓN
        // ====================================================

        public async Task<bool> VerifyPolizaExistsAsync(string numeroPoliza)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("🔍 Verificando que existe póliza {NumeroPoliza} en Velneo para tenant {TenantId}",
                    numeroPoliza, tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // ✅ BUSCAR POR NÚMERO DE PÓLIZA
                var searchUrl = await BuildVelneoUrlAsync($"v1/contratos?filter[conpol]={Uri.EscapeDataString(numeroPoliza)}");

                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();
                var maskedUrl = searchUrl.Replace(tenantConfig.Key, "***");
                _logger.LogInformation("🔍 Verificando URL: {Url}", maskedUrl);

                var response = await httpClient.GetAsync(searchUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️ Error verificando póliza {NumeroPoliza}: Status {Status}",
                        numeroPoliza, response.StatusCode);
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("📥 Respuesta verificación: {Length} caracteres", responseContent.Length);

                // ✅ LOG DE LA RESPUESTA COMPLETA PARA DEBUG
                _logger.LogWarning("🔍 RESPUESTA VERIFICACIÓN COMPLETA: {ResponseContent}", responseContent);

                // ✅ PARSEAR RESPUESTA PARA VER SI EXISTE
                using var jsonDocument = JsonDocument.Parse(responseContent);
                var root = jsonDocument.RootElement;

                // Buscar en la estructura que use Velneo
                if (root.TryGetProperty("contratos", out var contratosElement))
                {
                    var contratos = contratosElement.EnumerateArray().ToList();
                    var exists = contratos.Any();

                    _logger.LogInformation(exists ?
                        "✅ Póliza {NumeroPoliza} CONFIRMADA en Velneo - {Count} registro(s) encontrado(s)" :
                        "❌ Póliza {NumeroPoliza} NO ENCONTRADA en Velneo",
                        numeroPoliza, contratos.Count);

                    return exists;
                }

                // ✅ INTENTAR OTRAS ESTRUCTURAS POSIBLES
                if (root.TryGetProperty("data", out var dataElement))
                {
                    var hasData = dataElement.EnumerateArray().Any();
                    _logger.LogInformation(hasData ?
                        "✅ Póliza {NumeroPoliza} CONFIRMADA en Velneo (estructura 'data')" :
                        "❌ Póliza {NumeroPoliza} NO ENCONTRADA en Velneo (estructura 'data')",
                        numeroPoliza);
                    return hasData;
                }

                _logger.LogWarning("⚠️ Estructura inesperada en respuesta de verificación para póliza {NumeroPoliza}", numeroPoliza);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error verificando existencia de póliza {NumeroPoliza} en Velneo", numeroPoliza);
                return false;
            }
        }

        private object GetDatosEnviadosSummary(PolizaCreateRequest request)
        {
            return new
            {
                poliza = new
                {
                    numero = request.Conpol,
                    prima = request.Conpremio,
                    desde = request.Confchdes,
                    hasta = request.Confchhas
                },
                vehiculo = new
                {
                    descripcion = request.Vehiculo,
                    marca = request.Marca,
                    modelo = request.Modelo,
                    anio = request.Anio,
                    motor = request.Motor,
                    chasis = request.Chasis,
                    matricula = request.Matricula,
                    combustible = request.Combustible
                },
                cliente = new
                {
                    nombre = request.Asegurado,
                    documento = request.Documento,
                    email = request.Email,
                    telefono = request.Telefono,
                    direccion = request.Direccion,
                    localidad = request.Localidad,
                    departamento = request.Departamento
                },
                financiero = new
                {
                    prima = request.Conpremio,
                    primaComercial = request.PrimaComercial,
                    premioTotal = request.PremioTotal,
                    moneda = request.Moneda
                },
                otros = new
                {
                    corredor = request.Corredor,
                    plan = request.Plan,
                    ramo = request.Ramo,
                    procesadoConIA = request.ProcesadoConIA
                }
            };
        }

        private string ExtractErrorMessage(string responseContent, HttpStatusCode statusCode)
        {
            try
            {
                // Intentar extraer mensaje de error del response JSON
                using var doc = JsonDocument.Parse(responseContent);

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
            catch
            {
                return $"Error HTTP {(int)statusCode}: {responseContent.Substring(0, Math.Min(100, responseContent.Length))}";
            }
        }

        private object MapearCreateRequestAVelneo(PolizaCreateRequest request)
        {
            var velneoContrato = new
            {
                // ✅ CAMPOS BÁSICOS REQUERIDOS
                id = 0,
                comcod = request.Comcod,
                seccod = request.SeccionId ?? 0, // ✅ YA FUNCIONA
                clinro = request.Clinro,
                conpol = request.Conpol ?? "",

                // ✅ FECHAS
                confchdes = !string.IsNullOrEmpty(request.Confchdes) ?
                    DateTime.Parse(request.Confchdes).ToString("yyyy-MM-dd") :
                    DateTime.Now.ToString("yyyy-MM-dd"),
                confchhas = !string.IsNullOrEmpty(request.Confchhas) ?
                    DateTime.Parse(request.Confchhas).ToString("yyyy-MM-dd") :
                    DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"),

                // ✅ MONTOS
                conpremio = request.Conpremio,
                contot = request.PremioTotal ?? request.Conpremio,

                // ✅ DATOS DEL VEHÍCULO
                conmaraut = request.Marca ?? "",
                conanioaut = TryParseInt(request.Anio?.ToString()) ?? 0,
                conmataut = request.Matricula ?? "",
                conmotor = request.Motor ?? "",
                conchasis = request.Chasis ?? "",

                // ✅ CAMPOS FALTANTES MAPEADOS CORRECTAMENTE
                conges = MapearTramite(request.Tramite),           // ← TRÁMITE
                convig = MapearEstadoPoliza(request.EstadoPoliza), // ← ESTADO PÓLIZA
                concar = request.Certificado ?? "",               // ← CERTIFICADO
                tposegdsc = request.Cobertura ?? "",              // ← COBERTURA
                forpagvid = MapearFormaPago(request.FormaPago),   // ← FORMA DE PAGO
                concuo = request.CantidadCuotas ?? 0,             // ← CUOTAS

                // ✅ CAMPOS DE CÓDIGOS/IDs
                catdsc = request.CategoriaId ?? 0,        // ← Categoría
                desdsc = request.DestinoId ?? 0,          // ← Destino  
                caldsc = request.CalidadId ?? 0,          // ← Calidad

                // ✅ DATOS DEL CLIENTE
                clinom = request.Asegurado ?? "",
                condom = request.Direccion ?? "",

                // ✅ OBSERVACIONES ENRIQUECIDAS
                observaciones = GenerarObservacionesCompletas(request),

                // ✅ MONEDA CORREGIDA
                moncod = MapearMonedaCodigo(request.Moneda),

                // ✅ COMBUSTIBLE
                combustibles = request.Combustible ?? "",

                // ✅ RAMO
                ramo = request.Ramo ?? "AUTOMOVILES",

                // ✅ CAMPOS OBLIGATORIOS CON VALORES POR DEFECTO
                concodrev = 0,
                conficto = 0,
                conclaaut = 0,
                condedaut = 0,
                conresciv = 0,
                conbonnsin = 0,
                conbonant = 0,
                concaraut = 0,
                concapaut = 0,
                concomcorr = 0,
                flocod = 0,
                conimp = 0,
                connroser = 0,
                concan = 0,
                concapla = 0,
                conflota = 0,
                condednum = 0,
                conpadre = 0,
                conobjtot = 0,
                convalacr = 0,
                convallet = 0,
                conviakb = 0,
                conviakn = 0,
                conviacos = 0,
                conviafle = 0,
                dptnom = 0,
                conedaret = 0,
                congar = 0,
                condecpri = 0,
                condecpro = 0,
                condecptj = 0,
                conviagas = 0,
                conviarec = 0,
                conviapri = 0,
                linobs = 0,
                tpoconcod = 0,
                tpovivcod = 0,
                tporiecod = 0,
                modcod = 0,
                concapase = 0,
                conpricap = 0,
                conriecod = 0,
                conrecfin = 0,
                conimprf = 0,
                conafesin = 0,
                conautcor = 0,
                conlinrie = 0,
                conconesp = 0,
                lincarta = 0,
                cancecod = 0,
                concomotr = 0,
                conviamon = 0,
                connroint = 0,
                conpadend = 0,
                contotpri = 0,
                padreaux = 0,
                conlinflot = 0,
                conflotimp = 0,
                conflottotal = 0,
                conflotsaldo = 0,
                otrcorrcod = 0,
                clipcupfia = 0,
                clinro1 = 0,
                consumsal = 0,
                com_sub_corr = 0,
                tipos_de_alarma = 0,
                coberturas_bicicleta = 0,
                com_bro = 0,
                com_bo = 0,
                contotant = 0,
                motivos_no_renovacion = 0,
                max_aereo = 0,
                max_mar = 0,
                max_terrestre = 0,
                tasa = 0,
                cat_cli = 0,
                comcod1 = 0,
                comcod2 = 0,
                pagos_efectivo = 0,
                productos_de_vida = 0,
                app_id = 0,
                asignado = 0,
                conidpad = 0,
                tarcod = 0,
                corrnom = 0,

                // ✅ CAMPOS BOOLEAN REQUERIDOS
                conautcort = true,
                leer = true,
                enviado = true,
                sob_recib = true,
                leer_obs = true,
                tiene_alarma = false,
                aereo = false,
                maritimo = false,
                terrestre = true,
                importacion = false,
                exportacion = false,
                offshore = false,
                transito_interno = false,
                llamar = false,
                granizo = false,
                var_ubi = false,
                mis_rie = false,

                // ✅ CAMPOS STRING VACÍOS REQUERIDOS
                conend = "",
                rieres = "",
                congesti = "",
                congeses = "",
                congrucon = "",
                contipoemp = "",
                conmatpar = "",
                conmatte = "",
                consta = "",
                contra = "",
                conconf = "",
                concaucan = "",
                contpoact = "",
                conesp = "",
                condecram = "",
                conmedtra = "",
                conviades = "",
                conviaa = "",
                conviaenb = "",
                conviatra = "",
                conubi = "",
                concaudsc = "",
                conincuno = "",
                concalcom = "",
                conriedsc = "",
                conlimnav = "",
                contpocob = "",
                connomemb = "",
                contpoemb = "",
                conautcome = "",
                conviafac = "",
                conviatpo = "",
                connrorc = "",
                condedurc = "",
                conautnd = "",
                conaccicer = "",
                condetemb = "",
                conclaemb = "",
                confabemb = "",
                conbanemb = "",
                conmatemb = "",
                convelemb = "",
                conmatriemb = "",
                conptoemb = "",
                condeta = "",
                conclieda = "",
                condecrea = "",
                condecaju = "",
                contpoemp = "",
                congaran = "",
                congarantel = "",
                mot_no_ren = "",
                condetrc = "",
                condetail = "",
                conespbon = "",
                sublistas = "",
                com_alias = "",
                clausula = "",
                facturacion = "",
                coning = "",
                idorden = "",
                gestion = "",

                // ✅ CAMPOS DE FECHA CON VALORES POR DEFECTO
                congesfi = DateTime.Now.ToString("yyyy-MM-dd"),
                confchcan = DateTime.Now.ToString("yyyy-MM-dd"),
                concomdes = DateTime.Now.ToString("yyyy-MM-dd"),
                concerfin = DateTime.Now.ToString("yyyy-MM-dd"),

                // ✅ TIMESTAMPS
                ingresado = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                last_update = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                update_date = DateTime.Now.ToString("yyyy-MM-dd")
            };

            return velneoContrato;
        }

        // ====================================================
        // MÉTODOS AUXILIARES PARA MAPEO ESPECÍFICO
        // ====================================================

        private static string MapearTramite(string? tramite)
        {
            return tramite?.ToUpperInvariant() switch
            {
                "NUEVO" => "N",
                "RENOVACION" => "R",
                "RENOVACIÓN" => "R",
                "ENDOSO" => "E",
                "ANULACION" => "A",
                "ANULACIÓN" => "A",
                _ => "N" // Default: Nuevo
            };
        }

        private static string MapearEstadoPoliza(string? estado)
        {
            return estado?.ToUpperInvariant() switch
            {
                "VIGENTE" => "1",
                "VENCIDA" => "2",
                "ANULADA" => "3",
                "CANCELADA" => "4",
                _ => "1" // Default: Vigente
            };
        }

        private static string MapearFormaPago(string? formaPago)
        {
            return formaPago?.ToUpperInvariant() switch
            {
                "CONTADO" => "CON",
                "TARJETA DE CREDITO" => "TAR",
                "TARJETA DE CRÉDITO" => "TAR",
                "DEBITO AUTOMATICO" => "DEB",
                "DÉBITO AUTOMÁTICO" => "DEB",
                "CUOTAS" => "CUO",
                _ => "CON" // Default: Contado
            };
        }

        private static int MapearMonedaCodigo(string? moneda)
        {
            return moneda?.ToUpperInvariant() switch
            {
                "USD" => 1,
                "DOLARES" => 1,
                "DÓLARES" => 1,
                "EUR" => 2,
                "EUROS" => 2,
                "UYU" => 0,
                "PESOS" => 0,
                _ => 0 // Default: UYU
            };
        }

        private static string GenerarObservacionesCompletas(PolizaCreateRequest request)
        {
            var observaciones = new List<string>();

            // ✅ OBSERVACIÓN PRINCIPAL
            if (!string.IsNullOrEmpty(request.Observaciones))
            {
                observaciones.Add(request.Observaciones);
            }

            // ✅ MARCA COMO PROCESADO CON IA
            observaciones.Add("Procesado automáticamente con Azure Document Intelligence");

            // ✅ DATOS ADICIONALES SI ESTÁN DISPONIBLES
            if (!string.IsNullOrEmpty(request.Email))
            {
                observaciones.Add($"Email: {request.Email}");
            }

            if (!string.IsNullOrEmpty(request.Telefono))
            {
                observaciones.Add($"Teléfono: {request.Telefono}");
            }

            if (!string.IsNullOrEmpty(request.Plan))
            {
                observaciones.Add($"Plan: {request.Plan}");
            }

            return string.Join(" | ", observaciones);
        }

        private static int? TryParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return int.TryParse(value, out int result) ? result : null;
        }

        #endregion

        #region Métodos de Secciones

        public async Task<SeccionDto> GetSeccionAsync(int id)
        {
            try
            {
                var secciones = await GetActiveSeccionesAsync();
                var seccion = secciones.FirstOrDefault(s => s.Id == id);

                if (seccion == null)
                {
                    throw new KeyNotFoundException($"Seccion with ID {id} not found in Velneo API");
                }

                return seccion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seccion {SeccionId} from Velneo API", id);
                throw;
            }
        }

        public async Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting active secciones from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/secciones");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ Usar el método helper unificado
                var velneoResponse = await DeserializeResponseAsync<VelneoSeccionesResponse>(response);

                if (velneoResponse?.Secciones != null)
                {
                    var secciones = velneoResponse.Secciones.ToSeccionDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} secciones from Velneo API", secciones.Count);
                    return secciones;
                }

                _logger.LogWarning("No secciones found in Velneo API response");
                return new List<SeccionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones from Velneo API");
                throw new ApplicationException($"Error getting secciones from Velneo API: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogInformation("🔍 Getting secciones for lookup from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/secciones");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ Usar el método helper unificado
                var velneoResponse = await DeserializeResponseAsync<VelneoSeccionesResponse>(response);

                if (velneoResponse?.Secciones != null)
                {
                    var seccionesLookup = velneoResponse.Secciones
                        .Select(s => new SeccionLookupDto
                        {
                            Id = s.Id,
                            Name = s.Seccion,
                            Icono = SeccionMappers.GetIconoForSeccion(s.Seccion),
                            Activo = true
                        })
                        .OrderBy(s => s.Name)
                        .ToList();

                    _logger.LogInformation("✅ Successfully retrieved {Count} secciones for lookup from Velneo API", seccionesLookup.Count);

                    foreach (var seccion in seccionesLookup.Take(5))
                    {
                        _logger.LogDebug("📋 Sección: {Name} (ID: {Id}) {Icono}", seccion.Name, seccion.Id, seccion.Icono);
                    }

                    return seccionesLookup;
                }

                return new List<SeccionLookupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for lookup from Velneo API");
                throw;
            }
        }

        public async Task<IEnumerable<SeccionDto>> GetSeccionesByCompanyAsync(int companyId)
        {
            _logger.LogInformation("Getting secciones for company {CompanyId} (returning all for now)", companyId);
            return await GetActiveSeccionesAsync();
        }

        public async Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm)
        {
            try
            {
                var allSecciones = await GetActiveSeccionesAsync();
                var filtered = allSecciones.Where(s =>
                    s.Seccion?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                _logger.LogInformation("Found {Count} secciones matching search term '{SearchTerm}'", filtered.Count, searchTerm);
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching secciones with term '{SearchTerm}' from Velneo API", searchTerm);
                throw;
            }
        }

        #endregion

        #region Métodos de Combustibles

        /// <summary>
        /// Obtiene todos los combustibles desde Velneo API
        /// </summary>
        public async Task<IEnumerable<CombustibleDto>> GetAllCombustiblesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all combustibles from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/combustibles");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (formato esperado de Velneo)
                var velneoResponse = await DeserializeResponseAsync<VelneoCombustiblesResponse>(response);
                if (velneoResponse?.Combustibles != null && velneoResponse.Combustibles.Any())
                {
                    var combustibles = velneoResponse.Combustibles.ToCombustibleDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} combustibles from Velneo API (wrapper format)",
                        combustibles.Count);
                    return combustibles;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoCombustibles = await DeserializeResponseAsync<List<VelneoCombustible>>(response);
                if (velneoCombustibles != null && velneoCombustibles.Any())
                {
                    var combustibles = velneoCombustibles.ToCombustibleDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} combustibles from Velneo API (array format)",
                        combustibles.Count);
                    return combustibles;
                }

                _logger.LogWarning("No combustibles found in Velneo API response");
                return new List<CombustibleDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting combustibles from Velneo API");
                throw new ApplicationException($"Error retrieving combustibles from Velneo API: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos de Destinos

        /// <summary>
        /// Obtiene todos los destinos desde Velneo API
        /// </summary>
        public async Task<IEnumerable<DestinoDto>> GetAllDestinosAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all destinos from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/destinos");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (formato esperado de Velneo)
                var velneoResponse = await DeserializeResponseAsync<VelneoDestinosResponse>(response);
                if (velneoResponse?.Destinos != null && velneoResponse.Destinos.Any())
                {
                    var destinos = velneoResponse.Destinos.ToDestinoDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} destinos from Velneo API (wrapper format)",
                        destinos.Count);
                    return destinos;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoDestinos = await DeserializeResponseAsync<List<VelneoDestino>>(response);
                if (velneoDestinos != null && velneoDestinos.Any())
                {
                    var destinos = velneoDestinos.ToDestinoDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} destinos from Velneo API (array format)",
                        destinos.Count);
                    return destinos;
                }

                _logger.LogWarning("No destinos found in Velneo API response");
                return new List<DestinoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting destinos from Velneo API");
                throw new ApplicationException($"Error retrieving destinos from Velneo API: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos de Categorías

        /// <summary>
        /// Obtiene todas las categorías desde Velneo API
        /// </summary>
        public async Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all categorias from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/categorias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (formato esperado de Velneo)
                var velneoResponse = await DeserializeResponseAsync<VelneoCategoriasResponse>(response);
                if (velneoResponse?.Categorias != null && velneoResponse.Categorias.Any())
                {
                    var categorias = velneoResponse.Categorias.ToCategoriaDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} categorias from Velneo API (wrapper format)",
                        categorias.Count);
                    return categorias;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoCategorias = await DeserializeResponseAsync<List<VelneoCategoria>>(response);
                if (velneoCategorias != null && velneoCategorias.Any())
                {
                    var categorias = velneoCategorias.ToCategoriaDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} categorias from Velneo API (array format)",
                        categorias.Count);
                    return categorias;
                }

                _logger.LogWarning("No categorias found in Velneo API response");
                return new List<CategoriaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categorias from Velneo API");
                throw new ApplicationException($"Error retrieving categorias from Velneo API: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos de Calidades

        /// <summary>
        /// Obtiene todas las calidades desde Velneo API
        /// </summary>
        public async Task<IEnumerable<CalidadDto>> GetAllCalidadesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all calidades from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/calidades");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (formato esperado de Velneo)
                var velneoResponse = await DeserializeResponseAsync<VelneoCalidadesResponse>(response);
                if (velneoResponse?.Calidades != null && velneoResponse.Calidades.Any())
                {
                    var calidades = velneoResponse.Calidades.ToCalidadDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} calidades from Velneo API (wrapper format)",
                        calidades.Count);
                    return calidades;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoCalidades = await DeserializeResponseAsync<List<VelneoCalidad>>(response);
                if (velneoCalidades != null && velneoCalidades.Any())
                {
                    var calidades = velneoCalidades.ToCalidadDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} calidades from Velneo API (array format)",
                        calidades.Count);
                    return calidades;
                }

                _logger.LogWarning("No calidades found in Velneo API response");
                return new List<CalidadDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calidades from Velneo API");
                throw new ApplicationException($"Error retrieving calidades from Velneo API: {ex.Message}", ex);
            }
        }

        #endregion

    }
}