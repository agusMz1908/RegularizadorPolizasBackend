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
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<TarifaController> _logger;

        public TarifaController(
            IVelneoApiService velneoApiService,
            ILogger<TarifaController> logger)
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
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
                _logger.LogInformation("🎯 TarifaController: Getting all tarifas from Velneo API...");

                var tarifas = await _velneoApiService.GetAllTarifasAsync();

                _logger.LogInformation("✅ TarifaController: Successfully retrieved {Count} tarifas", tarifas.Count());

                // Ordenar por compañía y luego por nombre para consistencia
                var tarifasOrdenadas = tarifas
                    .OrderBy(t => t.CompaniaId)
                    .ThenBy(t => t.Nombre)
                    .ToList();

                return Ok(tarifasOrdenadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifas from Velneo API");
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al obtener tarifas",
                    error = ex.Message
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

                var todasLasTarifas = await _velneoApiService.GetAllTarifasAsync();

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

                return Ok(lookup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifas lookup");
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al obtener tarifas lookup",
                    error = ex.Message
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

                var todasLasTarifas = await _velneoApiService.GetAllTarifasAsync();
                var tarifa = todasLasTarifas.FirstOrDefault(t => t.Id == id);

                if (tarifa == null)
                {
                    _logger.LogWarning("⚠️ TarifaController: Tarifa {TarifaId} not found", id);
                    return NotFound(new
                    {
                        message = $"Tarifa con ID {id} no encontrada"
                    });
                }

                _logger.LogInformation("✅ TarifaController: Found tarifa {TarifaId}: {Nombre}",
                    id, tarifa.Nombre);
                return Ok(tarifa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifa {TarifaId}", id);
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al obtener tarifa",
                    error = ex.Message
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

                var todasLasTarifas = await _velneoApiService.GetAllTarifasAsync();

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

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TarifaController: Error getting tarifas stats");
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al obtener estadísticas",
                    error = ex.Message
                });
            }
        }
    }
}