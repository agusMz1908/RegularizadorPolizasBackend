using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TarifaController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<TarifaController> _logger;

        public TarifaController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<TarifaController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TarifaDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TarifaDto>>> GetAllTarifas()
        {
            try
            {
                _logger.LogInformation("🎯 TarifaController: Getting all tarifas from VelneoMaestrosService...");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var tarifas = await _velneoMaestrosService.GetAllTarifasAsync();

                _logger.LogInformation("✅ TarifaController: Successfully retrieved {Count} tarifas", tarifas.Count());

                // Ordenar por compañía y luego por nombre para consistencia
                var tarifasOrdenadas = tarifas
                    .OrderBy(t => t.CompaniaId)
                    .ThenBy(t => t.Nombre)
                    .ToList();

                return Ok(new
                {
                    success = true,
                    data = tarifasOrdenadas,
                    total = tarifasOrdenadas.Count,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifas from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al obtener tarifas",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<TarifaLookupDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TarifaLookupDto>>> GetTarifasLookup([FromQuery] int? companiaId = null)
        {
            try
            {
                _logger.LogInformation("🎯 TarifaController: Getting tarifas lookup for company {CompaniaId}...",
                    companiaId?.ToString() ?? "ALL");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var todasLasTarifas = await _velneoMaestrosService.GetAllTarifasAsync();

                // Filtrar por compañía si se especifica
                var tarifasFiltradas = companiaId.HasValue
                    ? todasLasTarifas.Where(t => t.CompaniaId == companiaId.Value)
                    : todasLasTarifas;

                var lookup = tarifasFiltradas
                    .Where(t => t.Activa)
                    .Select(t => new TarifaLookupDto
                    {
                        Id = t.Id,
                        CompaniaId = t.CompaniaId,
                        Nombre = t.Nombre,
                        Codigo = t.Codigo
                    })
                    .OrderBy(t => t.Nombre)
                    .ToList();

                _logger.LogInformation("✅ TarifaController: Retrieved {Count} tarifas lookup for company {CompaniaId}",
                    lookup.Count, companiaId?.ToString() ?? "ALL");

                return Ok(new
                {
                    success = true,
                    data = lookup,
                    total = lookup.Count,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifas lookup");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al obtener tarifas lookup",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarifaDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TarifaDto>> GetTarifa(int id)
        {
            try
            {
                _logger.LogInformation("🎯 TarifaController: Getting tarifa {TarifaId}...", id);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var todasLasTarifas = await _velneoMaestrosService.GetAllTarifasAsync();
                var tarifa = todasLasTarifas.FirstOrDefault(t => t.Id == id);

                if (tarifa == null)
                {
                    _logger.LogWarning("⚠️ TarifaController: Tarifa {TarifaId} not found", id);
                    return NotFound(new
                    {
                        success = false,
                        message = $"Tarifa con ID {id} no encontrada",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ TarifaController: Found tarifa {TarifaId}: {Nombre}",
                    id, tarifa.Nombre);

                return Ok(new
                {
                    success = true,
                    data = tarifa,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifa {TarifaId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al obtener tarifa",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(IEnumerable<TarifaStatsDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TarifaStatsDto>>> GetTarifasStats()
        {
            try
            {
                _logger.LogInformation("📊 TarifaController: Getting tarifas statistics...");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var todasLasTarifas = await _velneoMaestrosService.GetAllTarifasAsync();

                var stats = todasLasTarifas
                    .GroupBy(t => t.CompaniaId)
                    .Select(group => new TarifaStatsDto
                    {
                        CompaniaId = group.Key,
                        TotalTarifas = group.Count(),
                        TarifasPopulares = group
                            .Take(5) // Top 5 tarifas por compañía
                            .Select(t => new TarifaLookupDto
                            {
                                Id = t.Id,
                                CompaniaId = t.CompaniaId,
                                Nombre = t.Nombre,
                                Codigo = t.Codigo
                            })
                            .ToList()
                    })
                    .OrderBy(stats => stats.CompaniaId)
                    .ToList();

                _logger.LogInformation("✅ TarifaController: Generated stats for {Count} companies",
                    stats.Count);

                return Ok(new
                {
                    success = true,
                    data = stats,
                    total = stats.Count,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifas stats");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al obtener estadísticas",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}