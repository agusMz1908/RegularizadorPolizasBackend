using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Extensions;
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
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationDtoAsync();

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
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationDtoAsync(); 
            var baseUrl = tenantConfig.BaseUrl.TrimEnd('/');
            var separator = endpoint.Contains('?') ? "&" : "?";
            var fullUrl = $"{baseUrl}/{endpoint}{separator}api_key={tenantConfig.ApiKey}";

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
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationDtoAsync(); 
                var maskedUrl = url.Replace(tenantConfig.ApiKey, "***");
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

                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationDtoAsync(); 
                if (tenantConfig == null)
                {
                    _logger.LogError("❌ No se pudo obtener configuración del tenant");
                    return new List<ClientDto>();
                }

                // ✅ CREAR HTTPCLIENT USANDO FACTORY
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                // ✅ CONSTRUIR URL CON FILTRO DIRECTO
                var filterUrl = $"{tenantConfig.BaseUrl}/v1/clientes?filter[nombre]={Uri.EscapeDataString(searchTerm)}&api_key={tenantConfig.ApiKey}";

                _logger.LogInformation("📤 URL filtrada: {FilterUrl}", filterUrl.Replace(tenantConfig.ApiKey, "***"));

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
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationDtoAsync();
                var maskedUrl = url.Replace(tenantConfig.ApiKey, "***");
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
                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationDtoAsync();
                var maskedUrl = url.Replace(tenantConfig.ApiKey, "***");
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
                _logger.LogInformation("🚀 CREANDO PÓLIZA EN VELNEO: Número={NumeroPoliza}, Cliente={ClienteId}, Tenant={TenantId}",
                    request.Conpol, request.Clinro, tenantId);

                // ✅ VALIDAR CAMPOS CRÍTICOS ANTES DE ENVIAR
                ValidarCamposRequeridos(request);

                using var httpClient = await GetConfiguredHttpClientAsync();

                // ✅ MAPEAR COMPLETAMENTE A ESTRUCTURA VELNEO (AHORA ASYNC)
                var velneoContrato = await MapearCreateRequestAVelneoCompleto(request);

                // Serializar el payload
                var jsonPayload = JsonSerializer.Serialize(velneoContrato, _jsonOptions);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("📤 PAYLOAD VELNEO: {JsonLength} caracteres para póliza {NumeroPoliza}",
                    jsonPayload.Length, request.Conpol);

                // ✅ ENVIAR A VELNEO
                var url = await BuildVelneoUrlAsync("v1/contratos");
                var response = await httpClient.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    stopwatch.Stop();
                    _logger.LogInformation("✅ PÓLIZA CREADA EXITOSAMENTE en Velneo: {NumeroPoliza} - Duration: {Duration}ms",
                        request.Conpol, stopwatch.ElapsedMilliseconds);

                    var result = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    return result ?? new { success = true, numeroPoliza = request.Conpol };
                }
                else
                {
                    var errorMessage = ExtractErrorMessageFromResponse(response.StatusCode, responseContent);
                    _logger.LogError("❌ ERROR al crear póliza en Velneo: {StatusCode} - {Error} - Póliza: {NumeroPoliza}",
                        response.StatusCode, errorMessage, request.Conpol);

                    throw new InvalidOperationException($"Error en Velneo: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ EXCEPCIÓN al crear póliza {NumeroPoliza} - Duration: {Duration}ms",
                    request.Conpol, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private string ExtractErrorMessageFromResponse(HttpStatusCode statusCode, string responseContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    return $"Error HTTP {(int)statusCode}: Sin contenido de respuesta";
                }

                // Intentar parsear como JSON para obtener mensaje específico
                var doc = JsonDocument.Parse(responseContent);

                // Buscar propiedades comunes de error
                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    return errorProp.GetString() ?? $"Error HTTP {(int)statusCode}";
                }

                if (doc.RootElement.TryGetProperty("message", out var messageProp))
                {
                    return messageProp.GetString() ?? $"Error HTTP {(int)statusCode}";
                }

                if (doc.RootElement.TryGetProperty("detail", out var detailProp))
                {
                    return detailProp.GetString() ?? $"Error HTTP {(int)statusCode}";
                }

                if (doc.RootElement.TryGetProperty("title", out var titleProp))
                {
                    return titleProp.GetString() ?? $"Error HTTP {(int)statusCode}";
                }

                // Si no hay propiedades conocidas, retornar parte del contenido
                return $"Error HTTP {(int)statusCode}: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}";
            }
            catch (JsonException)
            {
                // Si no es JSON válido, retornar el contenido truncado
                return $"Error HTTP {(int)statusCode}: {responseContent.Substring(0, Math.Min(100, responseContent.Length))}";
            }
            catch (Exception)
            {
                return $"Error HTTP {(int)statusCode}: Error al procesar respuesta";
            }
        }

        // ✅ MÉTODO COMPLETO CORREGIDO CON MAPEO DINÁMICO ASYNC

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

                // ✅ CAMPOS CON MAPEO DINÁMICO ASYNC
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
                convig = ResolverEstadoPoliza(request), // ✅ CORREGIDO PARA QUE USE VIG
                concan = 0,
                congrucon = "",
                contipoemp = "",
                conmatpar = "",
                conmatte = "",
                concapla = 0,
                conflota = 0,
                condednum = 0,
                consta = ResolverFormaPago(request),

                contra = ResolverTramite(request),
                conconf = "",
                conpadre = 0,

                confchcan = nowLocal.ToString("yyyy-MM-dd"),
                concaucan = "",
                conobjtot = 0,
                contpoact = "",
                conesp = "",
                convalacr = 0,
                convallet = 0,
                condecram = "",
                conmedtra = "",
                conviades = "",
                conviaa = "",
                conviaenb = "",
                conviakb = 0,
                conviakn = 0,
                conviatra = "",
                conviacos = 0,
                conviafle = 0,
                dptnom = 0,
                conedaret = 0,
                congar = 0,
                condecpri = 0,
                condecpro = 0,
                condecptj = 0,
                conubi = "",
                concaudsc = "",
                conincuno = "",
                conviagas = 0,
                conviarec = 0,
                conviapri = 0,
                linobs = 0,
                concomdes = nowLocal.ToString("yyyy-MM-dd"),
                concalcom = "1",
                tpoconcod = 0,
                tpovivcod = 0,
                tporiecod = 0,
                modcod = 0,
                concapase = 0,
                conpricap = 0,
                tposegdsc = ResolverCobertura(request),
                conriecod = 0,
                conriedsc = "",
                conrecfin = 0,
                conimprf = 0,
                conafesin = 0,
                conautcor = 0,
                conlinrie = 0,
                conconesp = 0,
                conlimnav = "",
                contpocob = "",
                connomemb = "",
                contpoemb = "",
                lincarta = 0,
                cancecod = 0,
                concomotr = 0,
                conautcome = "",
                conviafac = "",
                conviamon = ResolverMoneda(request),
                conviatpo = "1",
                connrorc = "",
                condedurc = "",
                lininclu = 0,
                linexclu = 0,
                concapret = 0,
                forpagvid = request.Forpagvid ?? "1",
                clinom = ResolverNombreCliente(request),
                tarcod = request.Tarcod ?? 0,
                corrnom = request.Corrnom ?? 0,
                connroint = 0,
                conautnd = "",
                conpadend = 0,
                contotpri = 0,
                padreaux = 0,
                conlinflot = 0,
                conflotimp = 0,
                conflottotal = 0,
                conflotsaldo = 0,
                conaccicer = "",
                concerfin = nowLocal.ToString("yyyy-MM-dd"),
                condetemb = "",
                conclaemb = "",
                confabemb = "",
                conbanemb = "",
                conmatemb = "",
                convelemb = "",
                conmatriemb = "",
                conptoemb = "",
                otrcorrcod = 0,
                condeta = "",
                observaciones = ResolverObservaciones(request),
                clipcupfia = 0,
                conclieda = "",
                condecrea = "",
                condecaju = "",
                conviatot = 0,
                contpoemp = "",
                congaran = "",
                congarantel = "",
                mot_no_ren = "",
                condetrc = "",
                conautcort = false,
                condetail = "",
                clinro1 = request.Clinro1 ?? 0,
                consumsal = 0,
                conespbon = "",
                leer = false,
                enviado = false,
                sob_recib = false,
                leer_obs = false,
                sublistas = "",
                com_sub_corr = 0,
                tipos_de_alarma = 0,
                tiene_alarma = false,
                coberturas_bicicleta = 0,
                com_bro = 0,
                com_bo = 0,
                contotant = 0,
                cotizacion = "",
                motivos_no_renovacion = 0,
                com_alias = request.Com_alias ?? "",
                ramo = request.Ramo ?? "AUTOMOVILES",
                clausula = "1",
                aereo = false,
                maritimo = false,
                terrestre = true,
                max_aereo = 0,
                max_mar = 0,
                max_terrestre = 0,
                tasa = 0,
                facturacion = "1",
                importacion = false,
                exportacion = false,
                offshore = false,
                transito_interno = false,
                coning = GetUsuarioIngreso(),
                cat_cli = 0,
                llamar = false,
                granizo = false,
                idorden = "",
                var_ubi = false,
                mis_rie = false,
                ingresado = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                last_update = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                comcod1 = 0,
                comcod2 = 0,
                pagos_efectivo = 0,
                productos_de_vida = 0,
                app_id = 1,
                update_date = nowLocal.ToString("yyyy-MM-dd"),
                gestion = "",
                asignado = 0,
                conidpad = 0
            };

            return velneoContrato;
        }

        #region MÉTODOS DE RESOLUCIÓN DE CAMPOS

        private int ResolverSeccion(PolizaCreateRequest request)
    {
        return request.Seccod > 0 ? request.Seccod : request.SeccionId ?? 0;
    }

    private string ResolverDireccion(PolizaCreateRequest request)
    {
        return request.Condom ?? request.Direccion ?? "";
    }

    private string ResolverMarca(PolizaCreateRequest request)
    {
        if (!string.IsNullOrEmpty(request.Conmaraut))
            return request.Conmaraut;

        if (!string.IsNullOrEmpty(request.Vehiculo))
            return request.Vehiculo;

        if (!string.IsNullOrEmpty(request.Marca) && !string.IsNullOrEmpty(request.Modelo))
            return $"{request.Marca} {request.Modelo}".Trim();

        return request.Marca ?? "";
    }

    private int ResolverAnio(PolizaCreateRequest request)
    {
        if (request.Conanioaut.HasValue && request.Conanioaut > 0)
            return request.Conanioaut.Value;

        if (request.Anio.HasValue && request.Anio > 0)
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
        if (request.Contot.HasValue && request.Contot > 0)
            return request.Contot.Value;

        if (request.PremioTotal.HasValue && request.PremioTotal > 0)
            return request.PremioTotal.Value;

        return request.Conpremio;
    }

    private int ResolverMoneda(PolizaCreateRequest request)
    {
        if (request.Moncod.HasValue && request.Moncod > 0)
            return request.Moncod.Value;

        return request.Moneda?.ToUpperInvariant() switch
        {
            "UYU" => 1,
            "USD" => 2,
            "UI" => 3,
            _ => 1
        };
    }
    private async Task<int> ResolverCategoria(PolizaCreateRequest request)
    {
        try
        {
            // ✅ PRIORIDAD 1: Campo directo catdsc (código numérico)
            if (request.Catdsc.HasValue && request.Catdsc.Value > 0)
            {
                _logger.LogInformation("📋 Usando categoría directa: código {Codigo}", request.Catdsc.Value);
                return request.Catdsc.Value;
            }
    
            // ✅ PRIORIDAD 2: Campo CategoriaId
            if (request.CategoriaId.HasValue && request.CategoriaId.Value > 0)
            {
                _logger.LogInformation("📋 Usando categoría desde CategoriaId: código {Codigo}", request.CategoriaId.Value);
                return request.CategoriaId.Value;
            }
    
            // ✅ PRIORIDAD 3: Mapear desde texto usando API de Velneo
            if (!string.IsNullOrEmpty(request.Categoria))
            {
                var categoriaCode = await MapearCategoriaConVelneo(request.Categoria);
                if (categoriaCode > 0)
                {
                    _logger.LogInformation("📋 Mapeando categoría '{Categoria}' a código {Codigo} desde Velneo", request.Categoria, categoriaCode);
                    return categoriaCode;
                }
            }
    
            // ✅ PRIORIDAD 4: Inferir desde vehículo y mapear con Velneo
            if (!string.IsNullOrEmpty(request.Conmaraut) || !string.IsNullOrEmpty(request.Marca))
            {
                var categoriaInferida = InferirCategoriaDesdeVehiculo(request.Conmaraut ?? request.Marca ?? "", request.Vehiculo);
                var categoriaCode = await MapearCategoriaConVelneo(categoriaInferida);
                if (categoriaCode > 0)
                {
                    _logger.LogInformation("📋 Infiriendo categoría '{Categoria}' desde vehículo y mapeando a código {Codigo}", categoriaInferida, categoriaCode);
                    return categoriaCode;
                }
            }
    
            // ✅ DEFAULT: Buscar "Automóvil" en Velneo
            var defaultCode = await MapearCategoriaConVelneo("AUTOMOVIL");
            _logger.LogWarning("⚠️ Usando categoría default 'AUTOMOVIL': código {Codigo}", defaultCode);
            return defaultCode > 0 ? defaultCode : 20; // Fallback al código que vimos en tu lista
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error resolviendo categoría, usando fallback");
            return 20; // Código de "Automóvil" según tu lista
        }
    }

        private async Task<int> ResolverDestino(PolizaCreateRequest request)
        {
            try
            {
                // ✅ PRIORIDAD 1: Campo directo desdsc (código numérico)
                if (request.Desdsc.HasValue && request.Desdsc.Value > 0)
                {
                    _logger.LogInformation("📋 Usando destino directo: código {Codigo}", request.Desdsc.Value);
                    return request.Desdsc.Value;
                }

                // ✅ PRIORIDAD 2: Campo DestinoId
                if (request.DestinoId.HasValue && request.DestinoId.Value > 0)
                {
                    _logger.LogInformation("📋 Usando destino desde DestinoId: código {Codigo}", request.DestinoId.Value);
                    return request.DestinoId.Value;
                }

                // ✅ PRIORIDAD 3: Mapear desde texto usando API de Velneo
                if (!string.IsNullOrEmpty(request.Destino))
                {
                    var destinoCode = await MapearDestinoConVelneo(request.Destino);
                    if (destinoCode > 0)
                    {
                        _logger.LogInformation("📋 Mapeando destino '{Destino}' a código {Codigo} desde Velneo", request.Destino, destinoCode);
                        return destinoCode;
                    }
                }

                // ✅ PRIORIDAD 4: Mapear desde uso
                if (!string.IsNullOrEmpty(request.Uso))
                {
                    var destinoCode = await MapearDestinoConVelneo(request.Uso);
                    if (destinoCode > 0)
                    {
                        _logger.LogInformation("📋 Mapeando uso '{Uso}' a destino código {Codigo}", request.Uso, destinoCode);
                        return destinoCode;
                    }
                }

                // ✅ DEFAULT: Buscar "PARTICULAR" en Velneo
                var defaultCode = await MapearDestinoConVelneo("PARTICULAR");
                _logger.LogWarning("⚠️ Usando destino default 'PARTICULAR': código {Codigo}", defaultCode);
                return defaultCode > 0 ? defaultCode : 1; // Fallback al código que vimos en tu lista
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo destino, usando fallback");
                return 1; // Código de "PARTICULAR" según tu lista
            }
        }

        private async Task<string> ResolverCombustible(PolizaCreateRequest request)
        {
            try
            {
                // ✅ PRIORIDAD 1: Campo directo Combustible
                if (!string.IsNullOrEmpty(request.Combustible))
                {
                    var combustibleCode = await MapearCombustibleConVelneo(request.Combustible);
                    if (!string.IsNullOrEmpty(combustibleCode))
                    {
                        _logger.LogInformation("📋 Mapeando combustible '{Combustible}' a código '{Codigo}' desde Velneo", request.Combustible, combustibleCode);
                        return combustibleCode;
                    }
                }

                // ✅ PRIORIDAD 2: Inferir desde marca/modelo y mapear con Velneo
                var marca = request.Conmaraut ?? request.Marca ?? "";
                var modelo = request.Modelo ?? "";

                if (!string.IsNullOrEmpty(marca))
                {
                    var combustibleInferido = InferirCombustibleDesdeVehiculo(marca, modelo);
                    var combustibleCode = await MapearCombustibleConVelneo(combustibleInferido);
                    if (!string.IsNullOrEmpty(combustibleCode))
                    {
                        _logger.LogInformation("📋 Infiriendo combustible '{Combustible}' desde vehículo y mapeando a código '{Codigo}'", combustibleInferido, combustibleCode);
                        return combustibleCode;
                    }
                }

                // ✅ DEFAULT: Buscar "GASOLINA" en Velneo
                var defaultCode = await MapearCombustibleConVelneo("GASOLINA");
                _logger.LogWarning("⚠️ Usando combustible default 'GASOLINA': código '{Codigo}'", defaultCode);
                return !string.IsNullOrEmpty(defaultCode) ? defaultCode : "GAS"; // Fallback al código que vimos en tu lista
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo combustible, usando fallback");
                return "GAS"; // Código de "GASOLINA" según tu lista
            }
        }

        private int ResolverCuotas(PolizaCreateRequest request)
        {
            // ✅ PRIORIDAD 1: Campo directo concuo
            if (request.Concuo.HasValue && request.Concuo.Value > 0)
            {
                _logger.LogInformation("📋 Usando cuotas directo: {Cuotas}", request.Concuo.Value);
                return request.Concuo.Value;
            }

            // ✅ PRIORIDAD 2: Campo CantidadCuotas
            if (request.CantidadCuotas.HasValue && request.CantidadCuotas.Value > 0)
            {
                _logger.LogInformation("📋 Usando cuotas desde CantidadCuotas: {Cuotas}", request.CantidadCuotas.Value);
                return request.CantidadCuotas.Value;
            }

            // ✅ PRIORIDAD 3: Inferir desde forma de pago
            var formaPago = request.Consta ?? request.FormaPago ?? "";
            var cuotasInferidas = InferirCuotasDesdeFormaPago(formaPago);

            if (cuotasInferidas > 0)
            {
                _logger.LogInformation("📋 Infiriendo cuotas desde forma de pago '{FormaPago}': {Cuotas}", formaPago, cuotasInferidas);
                return cuotasInferidas;
            }

            // ✅ PRIORIDAD 4: Calcular desde montos si hay valor de cuota
            if (request.ValorCuota.HasValue && request.ValorCuota.Value > 0 && request.Contot.HasValue && request.Contot.Value > 0)
            {
                var cuotasCalculadas = (int)Math.Ceiling(request.Contot.Value / request.ValorCuota.Value);
                cuotasCalculadas = Math.Min(cuotasCalculadas, 12); // Máximo 12 cuotas

                _logger.LogInformation("📋 Calculando cuotas desde total/valor cuota: {Total}/{ValorCuota} = {Cuotas}",
                    request.Contot.Value, request.ValorCuota.Value, cuotasCalculadas);
                return cuotasCalculadas;
            }

            // ✅ DEFAULT: 1 cuota (contado)
            _logger.LogInformation("📋 Usando cuotas default: 1 (contado)");
            return 1;
        }

        private async Task<int> ResolverCalidad(PolizaCreateRequest request)
        {
            try
            {
                // ✅ PRIORIDAD 1: Campo directo caldsc (código numérico)
                if (request.Caldsc.HasValue && request.Caldsc.Value > 0)
                {
                    _logger.LogInformation("📋 Usando calidad directa: código {Codigo}", request.Caldsc.Value);
                    return request.Caldsc.Value;
                }

                // ✅ PRIORIDAD 2: Campo CalidadId
                if (request.CalidadId.HasValue && request.CalidadId.Value > 0)
                {
                    _logger.LogInformation("📋 Usando calidad desde CalidadId: código {Codigo}", request.CalidadId.Value);
                    return request.CalidadId.Value;
                }

                // ✅ PRIORIDAD 3: Mapear desde texto usando API de Velneo
                if (!string.IsNullOrEmpty(request.Calidad))
                {
                    var calidadCode = await MapearCalidadConVelneo(request.Calidad);
                    if (calidadCode > 0)
                    {
                        _logger.LogInformation("📋 Mapeando calidad '{Calidad}' a código {Codigo} desde Velneo", request.Calidad, calidadCode);
                        return calidadCode;
                    }
                }

                // ✅ DEFAULT: Buscar "PROPIETARIO" en Velneo (parece ser el más común según tu lista)
                var defaultCode = await MapearCalidadConVelneo("PROPIETARIO");
                _logger.LogWarning("⚠️ Usando calidad default 'PROPIETARIO': código {Codigo}", defaultCode);
                return defaultCode > 0 ? defaultCode : 2; // Fallback al código que vimos en tu lista
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resolviendo calidad, usando fallback");
                return 2; // Código de "PROPIETARIO" según tu lista
            }
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
            // ✅ PRIORIDAD 1: Campo directo
            if (!string.IsNullOrEmpty(request.Convig))
            {
                _logger.LogInformation("📋 Usando estado póliza directo: '{Estado}'", request.Convig);
                return request.Convig;
            }

            // ✅ PRIORIDAD 2: Campo EstadoPoliza
            if (!string.IsNullOrEmpty(request.EstadoPoliza))
            {
                var estadoMapeado = MapearEstadoPolizaTextoACodigo(request.EstadoPoliza);
                _logger.LogInformation("📋 Mapeando estado póliza '{Estado}' a código '{Codigo}'", request.EstadoPoliza, estadoMapeado);
                return estadoMapeado;
            }

            // ✅ DEFAULT: Vigente
            _logger.LogInformation("📋 Usando estado póliza default: VIG (Vigente)");
            return "VIG";
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

        private string GetUsuarioIngreso()
        {
        return "Sistema Automático";
        }

        // ============================================================================
        // 🗺️ MÉTODOS DE MAPEO DE TEXTO A CÓDIGO
        // ============================================================================

        private async Task<int> MapearCategoriaConVelneo(string categoriaTexto)
        {
            try
            {
                var categorias = await GetAllCategoriasAsync();
                var categoria = BuscarCategoriaPorTexto(categorias, categoriaTexto);
                return categoria?.Id ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categorías de Velneo");
                return 0;
            }
        }

        private async Task<int> MapearDestinoConVelneo(string destinoTexto)
        {
            try
            {
                var destinos = await GetAllDestinosAsync();
                var destino = BuscarDestinoPorTexto(destinos, destinoTexto);
                return destino?.Id ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo destinos de Velneo");
                return 0;
            }
        }

        private async Task<string> MapearCombustibleConVelneo(string combustibleTexto)
        {
            try
            {
                var combustibles = await GetAllCombustiblesAsync();
                var combustible = BuscarCombustiblePorTexto(combustibles, combustibleTexto);
                return combustible?.Id ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo combustibles de Velneo");
                return "";
            }
        }

        private async Task<int> MapearCalidadConVelneo(string calidadTexto)
        {
            try
            {
                var calidades = await GetAllCalidadesAsync();
                var calidad = BuscarCalidadPorTexto(calidades, calidadTexto);
                return calidad?.Id ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo calidades de Velneo");
                return 0;
            }
        }

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

        private static CategoriaDto? BuscarCategoriaPorTexto(IEnumerable<CategoriaDto> categorias, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            // Búsqueda exacta
            var exacta = categorias.FirstOrDefault(c => c.Catdsc?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            // Búsqueda por palabras clave
            var porPalabra = categorias.FirstOrDefault(c =>
                c.Catdsc?.ToUpperInvariant().Contains(textoUpper) == true);
            if (porPalabra != null) return porPalabra;

            // Mapeo específico basado en tu lista
            return textoUpper switch
            {
                "AUTOMOVIL" or "AUTO" or "COCHE" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("Automóvil") == true),
                "CAMIONETA" or "PICKUP" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("Camioneta") == true || c.Catdsc?.Contains("Pick-Up") == true),
                "MOTO" or "MOTOCICLETA" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("MOTOS") == true),
                "JEEP" or "SUV" => categorias.FirstOrDefault(c => c.Catdsc?.Contains("Jeeps") == true),
                _ => null
            };
        }

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

        private static CalidadDto? BuscarCalidadPorTexto(IEnumerable<CalidadDto> calidades, string texto)
        {
            var textoUpper = texto.ToUpperInvariant().Trim();

            // Búsqueda exacta
            var exacta = calidades.FirstOrDefault(c => c.Caldsc?.ToUpperInvariant() == textoUpper);
            if (exacta != null) return exacta;

            // Mapeo específico basado en tu lista
            return textoUpper switch
            {
                "PROPIETARIO" or "DUEÑO" or "OWNER" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("PROPIETARIO") == true),
                "USUARIO" or "USER" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("USUARIO") == true),
                "ARRENDATARIO" or "INQUILINO" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("ARRENDATARIO") == true),
                "COMPRADOR" => calidades.FirstOrDefault(c => c.Caldsc?.Contains("COMPRADOR") == true),
                _ => calidades.FirstOrDefault(c => c.Caldsc?.ToUpperInvariant().Contains(textoUpper) == true)
            };
        }

        // ============================================================================
        // 🧠 MÉTODOS DE INFERENCIA INTELIGENTE
        // ============================================================================

        private static string InferirCategoriaDesdeVehiculo(string marca, string? vehiculoCompleto)
        {
            var marcaUpper = marca.ToUpperInvariant();
            var vehiculoUpper = (vehiculoCompleto ?? "").ToUpperInvariant();

            if (vehiculoUpper.Contains("PICKUP") || vehiculoUpper.Contains("CAMIONETA") ||
                vehiculoUpper.Contains("SUV") || vehiculoUpper.Contains("4X4") ||
                vehiculoUpper.Contains("MASTER") || vehiculoUpper.Contains("TRANSIT"))
                return "CAMIONETA";

            if (vehiculoUpper.Contains("MOTO") || vehiculoUpper.Contains("SCOOTER"))
                return "MOTO";

            if (vehiculoUpper.Contains("CAMION") || vehiculoUpper.Contains("TRUCK"))
                return "CAMION";

            if (vehiculoUpper.Contains("JEEP"))
                return "JEEP";

            return "AUTOMOVIL"; // Default
        }

        private static string InferirCombustibleDesdeVehiculo(string marca, string modelo)
        {
            var marcaUpper = marca.ToUpperInvariant();
            var modeloUpper = modelo.ToUpperInvariant();

            if (modeloUpper.Contains("DIESEL") || modeloUpper.Contains("TDI") ||
                modeloUpper.Contains("HDI") || modeloUpper.Contains("CDI") ||
                modeloUpper.Contains("MASTER") || modeloUpper.Contains("TRANSIT"))
                return "DIESEL";

            if (modeloUpper.Contains("ELECTRIC") || modeloUpper.Contains("EV") ||
                marcaUpper == "TESLA")
                return "ELECTRICO";

            if (modeloUpper.Contains("HYBRID") || modeloUpper.Contains("HIBRIDO"))
                return "HIBRIDO";

            return "GASOLINA"; // Default
        }

        private static string MapearEstadoPolizaTextoACodigo(string estadoTexto)
        {
            var texto = estadoTexto.ToUpperInvariant().Trim();

            return texto switch
            {
                "VIGENTE" or "VIG" or "ACTIVA" or "ACTIVO" => "VIG",
                "VENCIDA" or "VEN" or "VENCIDO" => "VEN",
                "CANCELADA" or "CAN" or "CANCELADO" => "CAN",
                "ENDOSADA" or "END" or "ENDOSADO" => "END",
                "ANTECEDENTE" or "ANT" => "ANT",
                _ => "VIG" // DEFAULT: Vigente
            };
        }

        private static int InferirCuotasDesdeFormaPago(string formaPago)
        {
            if (string.IsNullOrEmpty(formaPago))
                return 0;

            var formaPagoUpper = formaPago.ToUpperInvariant().Trim();

            return formaPagoUpper switch
            {
                // Contado
                "CONTADO" or "1" or "EFECTIVO" or "CASH" => 1,

                // Tarjeta (típicamente se financian)
                "TARJETA" or "TARJETA DE CREDITO" or "TARJETA DE CRÉDITO" or "T" => 6,
                "CREDIT CARD" => 6,

                // Débito automático (típicamente financiado)
                "DEBITO" or "DEBITO AUTOMATICO" or "DÉBITO AUTOMÁTICO" => 10,
                "DEBIT" => 10,

                // Financiado
                "FINANCIADO" or "2" or "CUOTAS" => 12,
                "FINANCING" => 12,

                // Cheque (típicamente contado o pocas cuotas)
                "CHEQUE" => 3,

                // Transferencia (típicamente contado)
                "TRANSFERENCIA" or "WIRE" => 1,

                _ => 0 // No se puede inferir
            };
        }

        #endregion

        #region MÉTODOS DE MAPEO ESPECÍFICO

        private static string MapearTramite(string? tramite)
    {
        return tramite?.ToUpperInvariant() switch
        {
            "NUEVO" => "Nuevo",
            "RENOVACION" => "Renovacion",
            "RENOVACIÓN" => "Renovacion",
            "ENDOSO" => "Endoso",
            "ANULACION" => "Cancelacion",
            "ANULACIÓN" => "Cancelacion",
            "CAMBIO" => "Cambio",
            _ => "Nuevo"
        };
    }

    private static string MapearEstadoPoliza(string? estado)
    {
        return estado?.ToUpperInvariant() switch
        {
            "VIGENTE" => "VIG",
            "VENCIDA" => "VEN",
            "VENCIDO" => "VEN",
            "ENDOSADA" => "END",
            "ENDOSADO" => "END",
            "CANCELADA" => "CAN",
            "CANCELADO" => "CAN",
            "ANULADA" => "CAN",
            "ANULADO" => "CAN",
            "ANTECEDENTE" => "ANT",
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

    #endregion

    #region MÉTODOS AUXILIARES

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

    private static string? FormatearFecha(object? fecha)
    {
        if (fecha == null) return null;

        if (fecha is DateTime dt)
            return dt.ToString("yyyy-MM-dd");

        if (fecha is string str && DateTime.TryParse(str, out var parsedDate))
            return parsedDate.ToString("yyyy-MM-dd");

        return null;
    }

    private static int? TryParseInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : null;
    }

    #endregion
        
    #endregion

    #region Métodos de Secciones
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

    public async Task<SeccionDto> GetSeccionAsync(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo sección: {SeccionId}", id);

            // Opción 1: Usar el método existente que ya funciona
            var secciones = await GetActiveSeccionesAsync();
            var seccion = secciones.FirstOrDefault(s => s.Id == id);

            if (seccion == null)
            {
                _logger.LogWarning("⚠️ Sección no encontrada: {SeccionId}", id);
                throw new KeyNotFoundException($"Sección con ID {id} no encontrada en Velneo API");
            }

            _logger.LogInformation("✅ Sección obtenida: {SeccionId} - {Nombre}", id, seccion.Seccion);
            return seccion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error obteniendo sección: {SeccionId}", id);
            throw;
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

    #region Métodos de Monedas
        public async Task<IEnumerable<MonedaDto>> GetAllMonedasAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all monedas from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await GetConfiguredHttpClientAsync();
                var url = await BuildVelneoUrlAsync("v1/monedas");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (formato esperado de Velneo)
                var velneoResponse = await DeserializeResponseAsync<VelneeMonedasResponse>(response);
                if (velneoResponse?.Monedas != null && velneoResponse.Monedas.Any())
                {
                    var monedas = velneoResponse.Monedas.ToMonedaDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} monedas from Velneo API (wrapper format). Total: {Total}",
                        monedas.Count, velneoResponse.TotalCount);
                    return monedas;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoMonedas = await DeserializeResponseAsync<List<VelneoMoneda>>(response);
                if (velneoMonedas != null && velneoMonedas.Any())
                {
                    var monedas = velneoMonedas.ToMonedaDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} monedas from Velneo API (array format)",
                        monedas.Count);
                    return monedas;
                }

                _logger.LogWarning("No monedas found in Velneo API response");
                return new List<MonedaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monedas from Velneo API");
                throw new ApplicationException($"Error retrieving monedas from Velneo API: {ex.Message}", ex);
            }
        }

        #endregion

    }
}
