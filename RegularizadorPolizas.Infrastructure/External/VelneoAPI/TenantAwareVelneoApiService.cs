using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;
using System.Diagnostics;
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

        #region Métodos NO IMPLEMENTADOS

        public async Task<ClientDto> CreateClienteAsync(ClientDto clienteDto)
        {
            throw new NotImplementedException("CreateCliente no está implementado en Velneo API aún");
        }

        public async Task UpdateClienteAsync(ClientDto clienteDto)
        {
            throw new NotImplementedException("UpdateCliente no está implementado en Velneo API aún");
        }

        public async Task DeleteClienteAsync(int id)
        {
            throw new NotImplementedException("DeleteCliente no está implementado en Velneo API aún");
        }

        public async Task<PolizaDto> GetPolizaByNumberAsync(string numeroPoliza)
        {
            throw new NotImplementedException("GetPolizaByNumber no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchPolizas no está implementado en Velneo API aún");
        }

        public async Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto)
        {
            throw new NotImplementedException("CreatePoliza no está implementado en Velneo API aún");
        }

        public async Task UpdatePolizaAsync(PolizaDto polizaDto)
        {
            throw new NotImplementedException("UpdatePoliza no está implementado en Velneo API aún");
        }

        public async Task DeletePolizaAsync(int id)
        {
            throw new NotImplementedException("DeletePoliza no está implementado en Velneo API aún");
        }
        public async Task<SeccionDto> CreateSeccionAsync(SeccionDto seccionDto)
        {
            throw new NotImplementedException("CreateSeccion no está implementado en Velneo API aún");
        }

        public async Task UpdateSeccionAsync(SeccionDto seccionDto)
        {
            throw new NotImplementedException("UpdateSeccion no está implementado en Velneo API aún");
        }

        public async Task DeleteSeccionAsync(int id)
        {
            throw new NotImplementedException("DeleteSeccion no está implementado en Velneo API aún");
        }

        public async Task<BrokerDto> GetBrokerAsync(int id)
        {
            throw new NotImplementedException("GetBroker no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<BrokerDto>> GetBrokersAsync()
        {
            throw new NotImplementedException("GetBrokers no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchBrokers no está implementado en Velneo API aún");
        }

        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            throw new NotImplementedException("CreateBroker no está implementado en Velneo API aún");
        }

        public async Task UpdateBrokerAsync(BrokerDto brokerDto)
        {
            throw new NotImplementedException("UpdateBroker no está implementado en Velneo API aún");
        }

        public async Task DeleteBrokerAsync(int id)
        {
            throw new NotImplementedException("DeleteBroker no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm)
        {
            throw new NotImplementedException("SearchCurrencies no está implementado en Velneo API aún");
        }

        public async Task<CurrencyDto?> GetDefaultCurrencyAsync()
        {
            throw new NotImplementedException("GetDefaultCurrency no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync()
        {
            throw new NotImplementedException("GetCurrenciesForLookup no está implementado en Velneo API aún");
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
        {
            throw new NotImplementedException("GetAllCurrencies no está implementado en Velneo API aún");
        }

        #endregion

    }
}