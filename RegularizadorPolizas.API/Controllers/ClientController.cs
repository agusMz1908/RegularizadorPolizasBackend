using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        // ✅ MIGRADO: Solo usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ SOLO ESTE
            ILogger<ClientController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

                // ✅ MIGRADO: Usar VelneoMaestrosService con paginación real
                var velneoResponse = await _velneoMaestrosService.GetClientesPaginatedAsync(page, pageSize, search);

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
                    startItem = velneoResponse.Items.Any() ?
                        ((page - 1) * pageSize + 1) : 0,
                    endItem = Math.Min(page * pageSize, velneoResponse.TotalCount),

                    // ✅ Info de performance
                    requestDuration = velneoResponse.RequestDuration.TotalMilliseconds,
                    dataSource = "velneo_maestros_service_pagination",
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
                _logger.LogInformation("🔄 ALL CLIENTS: Getting all clients - Search: {Search}", search);

                // ✅ MIGRADO: Usar VelneoMaestrosService
                var allClients = (await _velneoMaestrosService.GetClientesAsync()).ToList();

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
                    dataSource = "velneo_maestros_service_full_load",
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
                    dataSource = "velneo_maestros_service_full_load"
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
                _logger.LogInformation("Getting client {ClientId} via VelneoMaestrosService", id);

                // ✅ MIGRADO: Usar VelneoMaestrosService
                var client = await _velneoMaestrosService.GetClienteAsync(id);

                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found in VelneoMaestrosService", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                _logger.LogInformation("Successfully retrieved client {ClientId} from VelneoMaestrosService", id);
                return Ok(client);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found in VelneoMaestrosService", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCliente not implemented in VelneoMaestrosService: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetCliente no está implementado en VelneoMaestrosService aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId} from VelneoMaestrosService", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClientes([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Término de búsqueda requerido" });
                }

                _logger.LogInformation("🔍 Searching clients with term '{SearchTerm}' via VelneoMaestrosService", searchTerm);

                // ✅ YA MIGRADO: Este método ya usa VelneoMaestrosService
                var clients = await _velneoMaestrosService.SearchClientesAsync(searchTerm);

                _logger.LogInformation("✅ Found {Count} clients matching '{SearchTerm}' in VelneoMaestrosService",
                    clients.Count(), searchTerm);

                return Ok(new
                {
                    items = clients,
                    count = clients.Count(),
                    searchTerm = searchTerm,
                    timestamp = DateTime.UtcNow,
                    service = "velneo_maestros_service",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error searching clients with term '{SearchTerm}' via VelneoMaestrosService", searchTerm);
                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message,
                    searchTerm = searchTerm,
                    timestamp = DateTime.UtcNow,
                    success = false
                });
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
                _logger.LogInformation("🔢 Getting clients count via VelneoMaestrosService");

                // ✅ MIGRADO: Usar VelneoMaestrosService
                var clients = await _velneoMaestrosService.GetClientesAsync();
                var count = clients.Count();

                _logger.LogInformation("✅ Total clients: {Count}", count);

                return Ok(new
                {
                    total = count,
                    service = "velneo_maestros_service",
                    timestamp = DateTime.UtcNow,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error counting clients via VelneoMaestrosService");
                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow,
                    success = false
                });
            }
        }

        [HttpGet("direct")]
        [ProducesResponseType(typeof(object), 200)] 
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> SearchClientesDirect([FromQuery] string filtro)  
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    return BadRequest(new
                    {
                        message = "Filtro de búsqueda requerido",
                        timestamp = DateTime.UtcNow,
                        success = false
                    });
                }

                _logger.LogInformation("🔍 BÚSQUEDA DIRECTA VELNEO: Searching clients with filter '{Filtro}' via VelneoMaestrosService", filtro);

                // ✅ PERFECTO: Ya usa VelneoMaestrosService
                var clients = await _velneoMaestrosService.SearchClientesDirectAsync(filtro);

                _logger.LogInformation("✅ BÚSQUEDA DIRECTA EXITOSA: Found {Count} clients matching '{Filtro}'", clients.Count(), filtro);

                // ✅ MEJORA: Respuesta consistente con otros endpoints
                return Ok(new
                {
                    items = clients,
                    count = clients.Count(),
                    filtro = filtro,
                    searchType = "direct_velneo_search",
                    timestamp = DateTime.UtcNow,
                    service = "velneo_maestros_service",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda directa de clientes con filtro '{Filtro}'", filtro);
                return StatusCode(500, new
                {
                    message = "Error en búsqueda directa de clientes",
                    error = ex.Message,
                    filtro = filtro,
                    service = "velneo_maestros_service",
                    timestamp = DateTime.UtcNow,
                    success = false
                });
            }
        }

        // ===========================
        // 🚧 MÉTODOS CREATE/UPDATE - No implementados en VelneoMaestrosService aún
        // ===========================

        [HttpPost]
        [ProducesResponseType(typeof(ClientDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                {
                    return BadRequest(new { message = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("Creating client via VelneoMaestrosService - NOT IMPLEMENTED YET");

                // ✅ TODO: Implementar CreateClienteAsync en VelneoMaestrosService
                return StatusCode(501, new
                {
                    message = "Creación de clientes no implementada en VelneoMaestrosService aún",
                    feature = "CreateClient",
                    recommendation = "Implementar CreateClienteAsync en VelneoMaestrosService",
                    timestamp = DateTime.UtcNow,
                    success = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client via VelneoMaestrosService");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> UpdateClient(int id, [FromBody] ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                {
                    return BadRequest(new { message = "Datos del cliente requeridos" });
                }

                if (id != clientDto.Id)
                {
                    return BadRequest(new { message = "ID en la URL no coincide con el ID del cliente" });
                }

                _logger.LogInformation("Updating client {ClientId} via VelneoMaestrosService - NOT IMPLEMENTED YET", id);

                // ✅ TODO: Implementar UpdateClienteAsync en VelneoMaestrosService
                return StatusCode(501, new
                {
                    message = "Actualización de clientes no implementada en VelneoMaestrosService aún",
                    feature = "UpdateClient",
                    clientId = id,
                    recommendation = "Implementar UpdateClienteAsync en VelneoMaestrosService",
                    timestamp = DateTime.UtcNow,
                    success = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {ClientId} via VelneoMaestrosService", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}