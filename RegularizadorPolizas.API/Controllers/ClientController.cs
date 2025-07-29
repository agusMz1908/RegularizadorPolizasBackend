using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External;
using RegularizadorPolizas.Application.Services;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<ClientsController> _logger;
        private readonly ApplicationDbContext _context;

        public ClientsController(
            IVelneoApiService velneoApiService,
            ILogger<ClientsController> logger,
            ApplicationDbContext context) 
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetClientes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 REAL PAGINATION: Getting clients - Page: {Page}, PageSize: {PageSize}, Search: {Search}",
                    page, pageSize, search);

                // ✅ USAR PAGINACIÓN REAL DE VELNEO
                var velneoResponse = await _velneoApiService.GetClientesPaginatedAsync(page, pageSize, search);

                var result = new
                {
                    items = velneoResponse.Items,
                    totalCount = velneoResponse.TotalCount,
                    currentPage = velneoResponse.PageNumber,
                    pageNumber = velneoResponse.PageNumber,
                    pageSize = velneoResponse.PageSize,
                    totalPages = velneoResponse.TotalPages,
                    hasNextPage = velneoResponse.HasNextPage,
                    hasPreviousPage = velneoResponse.HasPreviousPage,

                    // ✅ Metadatos útiles
                    startItem = velneoResponse.Items.Any() ? ((page - 1) * pageSize + 1) : 0,
                    endItem = Math.Min(page * pageSize, velneoResponse.TotalCount),

                    // ✅ Info de performance
                    requestDuration = velneoResponse.RequestDuration.TotalMilliseconds,
                    dataSource = "velneo_real_pagination",
                    velneoHasMoreData = velneoResponse.VelneoHasMoreData
                };

                _logger.LogInformation("✅ REAL PAGINATION SUCCESS: Page {Page}/{TotalPages} - {Count} clients in {Duration}ms",
                    page, velneoResponse.TotalPages, velneoResponse.Items.Count(), velneoResponse.RequestDuration.TotalMilliseconds);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error with REAL PAGINATION - Page: {Page}, PageSize: {PageSize}", page, pageSize);
                return StatusCode(500, new
                {
                    message = "Error obteniendo clientes con paginación real",
                    error = ex.Message,
                    pagination = "real"
                });
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetAllClientes([FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 ALL CLIENTS: Getting all clients (fallback to old method) - Search: {Search}", search);

                // ✅ Para "all", usamos el método anterior que ya funciona
                // Porque obtener TODO con paginación real sería muchas llamadas a Velneo
                var allClients = (await _velneoApiService.GetClientesAsync()).ToList();

                // Aplicar búsqueda si existe
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var originalCount = allClients.Count;
                    allClients = allClients.Where(c =>
                        (c.Clinom?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliemail?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliced?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliruc?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                    ).ToList();

                    _logger.LogInformation("Search filter applied: {FilteredCount} of {OriginalCount} clients",
                        allClients.Count, originalCount);
                }

                var result = new
                {
                    items = allClients,
                    totalCount = allClients.Count,
                    retrievedAt = DateTime.UtcNow,
                    dataSource = "velneo_full_load", 
                    filtered = !string.IsNullOrWhiteSpace(search),
                    note = "This endpoint loads all data from Velneo for backwards compatibility"
                };

                _logger.LogInformation("✅ ALL CLIENTS SUCCESS: {Count} total clients returned", allClients.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all clients");
                return StatusCode(500, new
                {
                    message = "Error obteniendo todos los clientes",
                    error = ex.Message,
                    dataSource = "velneo_full_load"
                });
            }
        }

        [HttpGet("direct")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClientesDirect([FromQuery] string filtro)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    return BadRequest(new { message = "Filtro de búsqueda requerido" });
                }

                _logger.LogInformation("🔍 BÚSQUEDA DIRECTA VELNEO: Searching clients with filter '{Filtro}'", filtro);

                // ✅ USAR EL MÉTODO QUE YA TIENES IMPLEMENTADO
                var clients = await _velneoApiService.SearchClientesDirectAsync(filtro);

                _logger.LogInformation("✅ BÚSQUEDA DIRECTA EXITOSA: Found {Count} clients matching '{Filtro}'", clients.Count(), filtro);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda directa de clientes con filtro '{Filtro}'", filtro);
                return StatusCode(500, new
                {
                    message = "Error en búsqueda directa de clientes",
                    error = ex.Message,
                    filtro = filtro
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            try
            {
                _logger.LogInformation("Getting client {ClientId} via Velneo API", id);

                var client = await _velneoApiService.GetClienteAsync(id);

                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found in Velneo API", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                _logger.LogInformation("Successfully retrieved client {ClientId} from Velneo API", id);
                return Ok(client);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId} from Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetClientsCount()
        {
            try
            {
                var clients = await _velneoApiService.GetClientesAsync();
                var count = clients.Count();

                return Ok(new { total = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting clients via Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClients([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Término de búsqueda requerido" });
                }

                _logger.LogInformation("Searching clients with term '{SearchTerm}' via Velneo API", searchTerm);

                var clients = await _velneoApiService.SearchClientesAsync(searchTerm);

                _logger.LogInformation("Found {Count} clients matching '{SearchTerm}' in Velneo API", clients.Count(), searchTerm);
                return Ok(clients);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("SearchClientes not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "SearchClientes no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients with term '{SearchTerm}' via Velneo API", searchTerm);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("tenant-config")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetTenantConfiguration()
        {
            try
            {
                _logger.LogInformation("🔑 Obteniendo configuración de tenant para búsqueda directa");

                const string TENANT_ID = "KEYDEMO"; 

                var apiKey = await _context.ApiKeys
                    .FirstOrDefaultAsync(a => a.TenantId == TENANT_ID &&
                                             a.Activo &&
                                             (a.FechaExpiracion == null || a.FechaExpiracion > DateTime.UtcNow));

                if (apiKey == null)
                {
                    _logger.LogError("❌ No se encontró configuración activa para tenant: {TenantId}", TENANT_ID);
                    return StatusCode(500, new { message = $"No hay configuración activa para tenant {TENANT_ID}" });
                }

                var clientConfig = new
                {
                    baseUrl = apiKey.BaseUrl,        
                    apiKey = apiKey.Key,                
                    tenantId = apiKey.TenantId,         
                    timeout = apiKey.TimeoutSeconds,    
                    isActive = apiKey.Activo,          
                    environment = apiKey.Environment    
                };

                _logger.LogInformation("✅ Configuración de tenant KEYDEMO obtenida exitosamente");
                _logger.LogInformation("📋 BaseUrl: {BaseUrl}", apiKey.BaseUrl);
                _logger.LogInformation("🔑 ApiKey: {ApiKeyMasked}", apiKey.Key?.Substring(0, 8) + "...");

                return Ok(clientConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo configuración de tenant");
                return StatusCode(500, new
                {
                    message = "Error obteniendo configuración",
                    error = ex.Message,
                    tenantId = "KEYDEMO"
                });
            }
        }
    }
}