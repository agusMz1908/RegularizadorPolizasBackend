using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces; 

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolizasController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly IPolizaService _polizaService; 
        private readonly ILogger<PolizasController> _logger;

        public PolizasController(
            IVelneoApiService velneoApiService,
            IPolizaService polizaService,
            ILogger<PolizasController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetPolizasCount()
        {
            try
            {
                _logger.LogInformation("Getting polizas count from Velneo API");

                var polizas = await _velneoApiService.GetPolizasAsync();
                var count = polizas.Count();

                var result = new
                {
                    total_polizas = count,
                    timestamp = DateTime.UtcNow,
                    source = "Velneo API - Contratos",
                    message = $"Total de {count} contratos/pólizas obtenidos exitosamente"
                };

                _logger.LogInformation("Successfully counted {Count} polizas from Velneo API", count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting polizas via Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PolizaDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResult<PolizaDto>>> GetPolizas(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Getting polizas with pagination - Page: {Page}, PageSize: {PageSize}", page, pageSize);

                var allPolizas = (await _velneoApiService.GetPolizasAsync()).ToList();

                var totalCount = allPolizas.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var polizasPage = allPolizas
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new PagedResult<PolizaDto>
                {
                    Items = polizasPage,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                _logger.LogInformation("Successfully retrieved page {Page} of {TotalPages} - {Count} polizas of {Total} total",
                    page, totalPages, polizasPage.Count, totalCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas with pagination");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PolizaDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> GetPolizaById(int id)
        {
            try
            {
                _logger.LogInformation("Getting poliza {PolizaId} via Velneo API", id);

                var poliza = await _velneoApiService.GetPolizaAsync(id);

                _logger.LogInformation("Successfully retrieved poliza {PolizaId} via Velneo API", id);
                return Ok(poliza);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Poliza {PolizaId} not found in Velneo API", id);
                return NotFound(new { message = $"Póliza con ID {id} no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting poliza {PolizaId} via Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(PagedResult<PolizaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResult<PolizaDto>>> GetPolizasByCliente(
            int clienteId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Getting polizas for client {ClienteId} with pagination - Page: {Page}, PageSize: {PageSize}",
                    clienteId, page, pageSize);

                var allPolizas = (await _velneoApiService.GetPolizasByClientAsync(clienteId)).ToList();

                if (!allPolizas.Any())
                {
                    _logger.LogWarning("No policies found for client {ClienteId}", clienteId);
                    return NotFound(new { message = $"No se encontraron pólizas para el cliente con ID {clienteId}" });
                }

                var totalCount = allPolizas.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var polizasPage = allPolizas
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new PagedResult<PolizaDto>
                {
                    Items = polizasPage,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                _logger.LogInformation("Successfully retrieved page {Page} of {TotalPages} - {Count} polizas of {Total} total for client {ClienteId}",
                    page, totalPages, polizasPage.Count, totalCount, clienteId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting polizas for client {ClienteId} with pagination", clienteId);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("cliente/{clienteId}/count")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetPolizasCountByCliente(int clienteId)
        {
            try
            {
                _logger.LogInformation("Getting polizas count for client {ClienteId} from Velneo API", clienteId);

                var polizas = await _velneoApiService.GetPolizasByClientAsync(clienteId);
                var count = polizas.Count();

                if (count == 0)
                {
                    return NotFound(new { message = $"No se encontraron pólizas para el cliente con ID {clienteId}" });
                }

                var result = new
                {
                    client_id = clienteId,
                    total_polizas = count,
                    timestamp = DateTime.UtcNow,
                    source = "Velneo API - Contratos filtrados por cliente"
                };

                _logger.LogInformation("Successfully counted {Count} polizas for client {ClienteId}", count, clienteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting polizas for client {ClienteId} via Velneo API", clienteId);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}