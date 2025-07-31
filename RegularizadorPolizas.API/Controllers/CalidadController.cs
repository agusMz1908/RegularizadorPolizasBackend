using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CalidadController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<CalidadController> _logger;

        public CalidadController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<CalidadController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalidadDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CalidadDto>>> GetAllCalidades()
        {
            try
            {
                _logger.LogInformation("🔧 Getting calidades from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var calidades = await _velneoMaestrosService.GetAllCalidadesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} calidades", calidades.Count());

                return Ok(new
                {
                    success = true,
                    data = calidades,
                    total = calidades.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting calidades from VelneoMaestrosService");
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