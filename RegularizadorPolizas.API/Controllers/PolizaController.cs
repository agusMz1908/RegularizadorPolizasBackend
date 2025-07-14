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
                    total = count,  
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
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetPolizas(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 REAL PAGINATION PÓLIZAS: Getting polizas - Page: {Page}, PageSize: {PageSize}, Search: {Search}",
                    page, pageSize, search);

                var velneoResponse = await _velneoApiService.GetPolizasPaginatedAsync(page, pageSize, search);

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

                    startItem = velneoResponse.Items.Any() ? ((page - 1) * pageSize + 1) : 0,
                    endItem = Math.Min(page * pageSize, velneoResponse.TotalCount),

                    requestDuration = velneoResponse.RequestDuration.TotalMilliseconds,
                    dataSource = "velneo_polizas_pagination",
                    velneoHasMoreData = velneoResponse.VelneoHasMoreData
                };

                _logger.LogInformation("✅ REAL PAGINATION PÓLIZAS SUCCESS: Page {Page}/{TotalPages} - {Count} pólizas in {Duration}ms",
                    page, velneoResponse.TotalPages, velneoResponse.Items.Count(), velneoResponse.RequestDuration.TotalMilliseconds);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error with REAL PAGINATION PÓLIZAS - Page: {Page}, PageSize: {PageSize}", page, pageSize);
                return StatusCode(500, new
                {
                    message = "Error obteniendo pólizas con paginación real",
                    error = ex.Message,
                    pagination = "real_polizas"
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PolizaDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
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

        [HttpGet("all")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetAllPolizas([FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 ALL PÓLIZAS: Getting all polizas (fallback to old method) - Search: {Search}", search);

                var allPolizas = (await _velneoApiService.GetPolizasAsync()).ToList();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var originalCount = allPolizas.Count;
                    allPolizas = allPolizas.Where(p =>
                        (p.Conpol?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Ramo?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Com_alias?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Clinom?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) 
                    ).ToList();

                    _logger.LogInformation("Search filter applied to pólizas: {FilteredCount} of {OriginalCount} pólizas",
                        allPolizas.Count, originalCount);
                }

                var result = new
                {
                    items = allPolizas,
                    totalCount = allPolizas.Count,
                    retrievedAt = DateTime.UtcNow,
                    dataSource = "velneo_polizas_full_load", 
                    filtered = !string.IsNullOrWhiteSpace(search),
                    note = "This endpoint loads all polizas from Velneo for backwards compatibility"
                };

                _logger.LogInformation("✅ ALL PÓLIZAS SUCCESS: {Count} total pólizas returned", allPolizas.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all pólizas");
                return StatusCode(500, new
                {
                    message = "Error obteniendo todas las pólizas",
                    error = ex.Message,
                    dataSource = "velneo_polizas_full_load"
                });
            }
        }

        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetPolizasByCliente(
            int clienteId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 PÓLIZAS POR CLIENTE: Getting polizas for client {ClienteId} - Page: {Page}, PageSize: {PageSize}, Search: {Search}",
                    clienteId, page, pageSize, search);

                // ✅ USAR PAGINACIÓN REAL PARA PÓLIZAS POR CLIENTE
                var velneoResponse = await _velneoApiService.GetPolizasByClientPaginatedAsync(clienteId, page, pageSize, search);

                var result = new
                {
                    clienteId = clienteId,
                    items = velneoResponse.Items,
                    totalCount = velneoResponse.TotalCount,
                    currentPage = velneoResponse.PageNumber,
                    pageNumber = velneoResponse.PageNumber,
                    pageSize = velneoResponse.PageSize,
                    totalPages = velneoResponse.TotalPages,
                    hasNextPage = velneoResponse.HasNextPage,
                    hasPreviousPage = velneoResponse.HasPreviousPage,

                    // ✅ Metadatos útiles para el frontend
                    startItem = velneoResponse.Items.Any() ? ((page - 1) * pageSize + 1) : 0,
                    endItem = Math.Min(page * pageSize, velneoResponse.TotalCount),
                    isEmpty = !velneoResponse.Items.Any(),

                    // ✅ Info de performance
                    requestDuration = velneoResponse.RequestDuration.TotalMilliseconds,
                    dataSource = "velneo_client_polizas_pagination",
                    velneoHasMoreData = velneoResponse.VelneoHasMoreData,

                    // ✅ Info para UI
                    searchApplied = !string.IsNullOrWhiteSpace(search),
                    searchTerm = search
                };

                _logger.LogInformation("✅ PÓLIZAS POR CLIENTE SUCCESS: Client {ClienteId}, Page {Page}/{TotalPages} - {Count} pólizas in {Duration}ms",
                    clienteId, page, velneoResponse.TotalPages, velneoResponse.Items.Count(), velneoResponse.RequestDuration.TotalMilliseconds);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting pólizas for client {ClienteId} - Page: {Page}, PageSize: {PageSize}",
                    clienteId, page, pageSize);
                return StatusCode(500, new
                {
                    message = $"Error obteniendo pólizas para cliente {clienteId}",
                    error = ex.Message,
                    clienteId = clienteId,
                    pagination = "client_polizas"
                });
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
                    total = count, 
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