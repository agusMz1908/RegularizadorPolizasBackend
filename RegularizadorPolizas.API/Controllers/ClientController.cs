using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            IVelneoApiService velneoApiService,
            ILogger<ClientsController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            try
            {
                _logger.LogInformation("Getting all clients via Velneo API");

                var clients = await _velneoApiService.GetClientesAsync();

                if (clients == null || !clients.Any())
                {
                    _logger.LogWarning("No clients found in Velneo API");
                    return Ok(new List<ClientDto>());
                }

                _logger.LogInformation("Successfully retrieved {Count} clients from Velneo API", clients.Count());
                return Ok(clients);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetClientes not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetClientes no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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

        [HttpPost]
        [ProducesResponseType(typeof(ClientDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                {
                    return BadRequest(new { message = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("Creating client via Velneo API");

                var createdClient = await _velneoApiService.CreateClienteAsync(clientDto);

                _logger.LogInformation("Successfully created client {ClientId} via Velneo API", createdClient.Id);
                return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("CreateCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "CreateCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client via Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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

                _logger.LogInformation("Updating client {ClientId} via Velneo API", id);

                await _velneoApiService.UpdateClienteAsync(clientDto);

                _logger.LogInformation("Successfully updated client {ClientId} via Velneo API", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found for update in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("UpdateCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "UpdateCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {ClientId} via Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteClient(int id)
        {
            try
            {
                _logger.LogInformation("Deleting client {ClientId} via Velneo API", id);

                await _velneoApiService.DeleteClienteAsync(id);

                _logger.LogInformation("Successfully deleted client {ClientId} via Velneo API", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found for deletion in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("DeleteCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "DeleteCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {ClientId} via Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("test-connection")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                _logger.LogInformation("Testing connection to Velneo API");

                var isConnected = await _velneoApiService.TestConnectionAsync();

                var result = new
                {
                    connected = isConnected,
                    message = isConnected ? "Conexión exitosa a Velneo API" : "No se pudo conectar a Velneo API",
                    timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Velneo API connection test result: {IsConnected}", isConnected);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection to Velneo API");
                return StatusCode(500, new { message = "Error probando conexión", error = ex.Message });
            }
        }
    }
}