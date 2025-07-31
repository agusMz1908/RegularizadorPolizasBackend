using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CombustibleController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<CombustibleController> _logger;

        public CombustibleController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<CombustibleController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CombustibleDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CombustibleDto>>> GetAllCombustibles()
        {
            try
            {
                _logger.LogInformation("⛽ Getting combustibles from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var combustibles = await _velneoMaestrosService.GetAllCombustiblesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} combustibles", combustibles.Count());

                return Ok(new
                {
                    success = true,
                    data = combustibles,
                    total = combustibles.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting combustibles from VelneoMaestrosService");
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