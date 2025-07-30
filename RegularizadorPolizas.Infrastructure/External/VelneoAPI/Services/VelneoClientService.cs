using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Application.Models;
using RegularizadorPolizas.Application.Mappers;
using System.Text.Json;
using System.Net.Http;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services
{
    public class VelneoClientService : BaseVelneoService, IVelneoClientService
    {
        public VelneoClientService(
            IVelneoHttpService httpService,
            ITenantService tenantService,
            ILogger<VelneoClientService> logger)
            : base(httpService, tenantService, logger)
        {
        }

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

        public async Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(
            int page = 1,
            int pageSize = 50,
            string? search = null)
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            _logger.LogInformation("📄 Getting clientes page {Page} (size: {PageSize}) for tenant {TenantId}",
                page, pageSize, tenantId);

            try
            {
                var endpoint = $"v1/clientes?page={page}&limit={pageSize}";
                if (!string.IsNullOrWhiteSpace(search))
                {
                    endpoint += $"&search={Uri.EscapeDataString(search)}";
                }

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync(endpoint);
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoClientesResponse>(response);

                if (velneoResponse?.Clientes == null)
                {
                    return new PaginatedVelneoResponse<ClientDto>
                    {
                        Items = new List<ClientDto>(),
                        TotalCount = 0,
                        PageNumber = page,
                        PageSize = pageSize,
                        VelneoHasMoreData = false
                    };
                }

                var clientsPage = velneoResponse.Clientes.ToClienteDtos().ToList();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetClientesPaginatedAsync - Page: {Page}, PageSize: {PageSize}",
                    page, pageSize);
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

        private int EstimateTotalCount(int currentPageCount, int currentPage, int pageSize)
        {
            if (currentPageCount < pageSize)
            {
                return ((currentPage - 1) * pageSize) + currentPageCount;
            }

            return currentPage * pageSize + 1; 
        }

        private async Task<List<ClientDto>> ParseDirectSearchResponse(HttpResponseMessage response, string searchTerm)
        {
            var clients = new List<ClientDto>();

            try
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    _logger.LogWarning("Empty response from direct search");
                    return clients;
                }

                var jsonDocument = JsonDocument.Parse(jsonContent);
                var root = jsonDocument.RootElement;

                JsonElement clientsArray;
                if (root.TryGetProperty("clientes", out clientsArray) && clientsArray.ValueKind == JsonValueKind.Array)
                {
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    clientsArray = root;
                }
                else
                {
                    _logger.LogWarning("No se encontró array de clientes en respuesta directa");
                    return clients;
                }

                foreach (var clientElement in clientsArray.EnumerateArray())
                {
                    try
                    {
                        var clientDto = new ClientDto
                        {
                            Polizas = new List<PolizaResumidaDto>(),
                            Activo = true
                        };

                        if (clientElement.TryGetProperty("id", out var idElement))
                            clientDto.Id = idElement.GetInt32();

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

                        clientDto.Clinom ??= "";
                        clientDto.Cliced ??= "";
                        clientDto.Cliruc ??= "";
                        clientDto.Cliemail ??= "";
                        clientDto.Clidir ??= "";
                        clientDto.Telefono ??= "";

                        if (!string.IsNullOrEmpty(clientDto.Clinom) && clientDto.Id > 0)
                        {
                            clients.Add(clientDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Error parseando cliente individual en respuesta directa");
                        continue;
                    }
                }

                return clients;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "❌ Error parseando JSON de búsqueda directa: {SearchTerm}", searchTerm);
                return clients;
            }
        }
    }
}