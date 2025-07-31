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
        // ✅ CRITICAL: Inyectar AMBOS servicios durante la transición
        private readonly IVelneoApiService _velneoApiService;      // Legacy - para métodos no refactorizados
        private readonly IVelneoMaestrosService _velneoMaestrosService; // Nuevo - para métodos refactorizados
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IVelneoApiService velneoApiService,           // Legacy
            IVelneoMaestrosService velneoMaestrosService, // ✅ NUEVO
            ILogger<ClientController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ===========================
        // ✅ MÉTODOS REFACTORIZADOS - USAR VelneoMaestrosService
        // ===========================

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

                // ✅ USAR SERVICIO REFACTORIZADO
                var client = await _velneoMaestrosService.GetClienteAsync(id);

                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found in Velneo API", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                _logger.LogInformation("Successfully retrieved client {ClientId} from VelneoMaestrosService", id);
                return Ok(client);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId} from VelneoMaestrosService", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
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
                // ✅ LOGGING INICIAL - SIMPLE Y SEGURO
                _logger.LogInformation("🔄 REAL PAGINATION: Getting clients - Page: {Page}, PageSize: {PageSize}", page, pageSize);
                if (!string.IsNullOrEmpty(search))
                {
                    _logger.LogInformation("🔍 Search filter applied: {SearchTerm}", search);
                }

                // ✅ USAR SERVICIO REFACTORIZADO
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
                    startItem = velneoResponse.Items.Any() ?
                        ((velneoResponse.PageNumber - 1) * velneoResponse.PageSize) + 1 : 0,
                    endItem = velneoResponse.Items.Any() ?
                        Math.Min(velneoResponse.PageNumber * velneoResponse.PageSize, velneoResponse.TotalCount) : 0,
                    timestamp = DateTime.UtcNow,
                    dataSource = "velneo_maestros_service"
                };

                // ✅ FIX CRÍTICO: Usar .Count() método de extensión, no .Count propiedad
                var currentPage = page;
                var totalPages = result.totalPages;
                var itemCount = velneoResponse.Items.Count(); // ✅ MÉTODO .Count() con paréntesis

                _logger.LogInformation("✅ PAGINACIÓN EXITOSA: Page {CurrentPage} of {TotalPages} - {ItemCount} clients retrieved",
                    currentPage, totalPages, itemCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // ✅ LOGGING DE ERROR - SIMPLE Y DIRECTO
                _logger.LogError(ex, "❌ Error getting paginated clients");
                return StatusCode(500, new
                {
                    message = "Error obteniendo clientes paginados",
                    error = ex.Message,
                    dataSource = "velneo_maestros_service"
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
                _logger.LogInformation("🔄 ALL CLIENTS: Getting all clients via VelneoMaestrosService - Search: {Search}", search);

                // ✅ USAR SERVICIO REFACTORIZADO
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
                    dataSource = "velneo_maestros_service", // ✅ NUEVO
                    filtered = !string.IsNullOrWhiteSpace(search),
                    note = "Using refactored VelneoMaestrosService"
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
                    dataSource = "velneo_maestros_service"
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

                _logger.LogInformation("🔍 BÚSQUEDA DIRECTA VELNEO: Searching clients with filter '{Filtro}' via VelneoMaestrosService", filtro);

                // ✅ CRITICAL FIX: Usar VelneoMaestrosService (refactorizado) en lugar del legacy
                var clients = await _velneoMaestrosService.SearchClientesDirectAsync(filtro);

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
                    filtro = filtro,
                    service = "velneo_maestros_service"
                });
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

                _logger.LogInformation("Searching clients with term '{SearchTerm}' via VelneoMaestrosService", searchTerm);

                // ✅ USAR SERVICIO REFACTORIZADO
                var clients = await _velneoMaestrosService.SearchClientesAsync(searchTerm);

                _logger.LogInformation("Found {Count} clients matching '{SearchTerm}' in VelneoMaestrosService", clients.Count(), searchTerm);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients with term '{SearchTerm}' via VelneoMaestrosService", searchTerm);
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
                // ✅ USAR SERVICIO REFACTORIZADO
                var clients = await _velneoMaestrosService.GetClientesAsync();
                var count = clients.Count();

                return Ok(new { total = count, service = "velneo_maestros_service" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting clients via VelneoMaestrosService");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // ===========================
        // ❌ MÉTODOS AÚN NO REFACTORIZADOS - USAR LEGACY TEMPORALMENTE
        // Estos se refactorizarán cuando implementen IVelneoClientService
        // ===========================

        //[HttpPost]
        //[ProducesResponseType(typeof(ClientDto), 201)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //[Authorize]
        //public async Task<ActionResult<ClientDto>> CreateClient([FromBody] ClientDto clientDto)
        //{
        //    try
        //    {
        //        if (clientDto == null)
        //        {
        //            return BadRequest(new { message = "Datos del cliente requeridos" });
        //        }

        //        _logger.LogInformation("Creating client via Legacy VelneoApiService (TODO: refactor)");

        //        // ❌ TEMPORAL: Usar legacy hasta que se refactorice CreateClienteAsync
        //        var createdClient = await _velneoApiService.CreateClienteAsync(clientDto);

        //        _logger.LogInformation("Successfully created client {ClientId} via Legacy API", createdClient.Id);
        //        return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
        //    }
        //    catch (NotImplementedException ex)
        //    {
        //        _logger.LogWarning("CreateCliente not implemented in Velneo API: {Message}", ex.Message);
        //        return StatusCode(501, new { message = "CreateCliente no está implementado en Velneo API aún", error = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating client via Legacy API");
        //        return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        //    }
        //}

        //[HttpPut("{id}")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(500)]
        //[Authorize]
        //public async Task<ActionResult> UpdateClient(int id, [FromBody] ClientDto clientDto)
        //{
        //    try
        //    {
        //        if (clientDto == null)
        //        {
        //            return BadRequest(new { message = "Datos del cliente requeridos" });
        //        }

        //        if (id != clientDto.Id)
        //        {
        //            return BadRequest(new { message = "ID en la URL no coincide con el ID del cliente" });
        //        }

        //        _logger.LogInformation("Updating client {ClientId} via Legacy VelneoApiService (TODO: refactor)", id);

        //        // ❌ TEMPORAL: Usar legacy hasta que se refactorice UpdateClienteAsync
        //        await _velneoApiService.UpdateClienteAsync(clientDto);

        //        _logger.LogInformation("Successfully updated client {ClientId} via Legacy API", id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        _logger.LogWarning("Client {ClientId} not found for update", id);
        //        return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        //    }
        //    catch (NotImplementedException ex)
        //    {
        //        _logger.LogWarning("UpdateCliente not implemented in Velneo API: {Message}", ex.Message);
        //        return StatusCode(501, new { message = "UpdateCliente no está implementado en Velneo API aún", error = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating client {ClientId} via Legacy API", id);
        //        return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        //    }
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(500)]
        //[Authorize]
        //public async Task<ActionResult> DeleteClient(int id)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Deleting client {ClientId} via Legacy VelneoApiService (TODO: refactor)", id);

        //        // ❌ TEMPORAL: Usar legacy hasta que se refactorice DeleteClienteAsync
        //        await _velneoApiService.DeleteClienteAsync(id);

        //        _logger.LogInformation("Successfully deleted client {ClientId} via Legacy API", id);
        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        _logger.LogWarning("Client {ClientId} not found for deletion", id);
        //        return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        //    }
        //    catch (NotImplementedException ex)
        //    {
        //        _logger.LogWarning("DeleteCliente not implemented in Velneo API: {Message}", ex.Message);
        //        return StatusCode(501, new { message = "DeleteCliente no está implementado en Velneo API aún", error = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting client {ClientId} via Legacy API", id);
        //        return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        //    }
        //}
    }
}