using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonedaController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<MonedaController> _logger;

        public MonedaController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<MonedaController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MonedaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MonedaDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("💰 Getting all monedas from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var monedas = await _velneoMaestrosService.GetAllMonedasAsync();

                if (monedas == null || !monedas.Any())
                {
                    _logger.LogWarning("No monedas found in VelneoMaestrosService");
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontraron monedas en Velneo",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ Successfully retrieved {Count} monedas", monedas.Count());

                return Ok(new
                {
                    success = true,
                    data = monedas,
                    total = monedas.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting monedas from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<MonedaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MonedaDto>>> GetActive()
        {
            try
            {
                _logger.LogInformation("💰 Getting active monedas from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var monedas = await _velneoMaestrosService.GetAllMonedasAsync();

                // Filtrar monedas activas
                var monedasActivas = monedas?.Where(m => m.Activa).ToList() ?? new List<MonedaDto>();

                if (!monedasActivas.Any())
                {
                    _logger.LogWarning("No active monedas found in VelneoMaestrosService");
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontraron monedas activas en Velneo",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ Successfully retrieved {Count} active monedas", monedasActivas.Count);

                return Ok(new
                {
                    success = true,
                    data = monedasActivas,
                    total = monedasActivas.Count,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting active monedas from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}