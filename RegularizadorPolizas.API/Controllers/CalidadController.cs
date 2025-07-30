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
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<CalidadController> _logger;

        public CalidadController(
            IVelneoApiService velneoApiService,
            ILogger<CalidadController> logger)
        {
            _velneoApiService = velneoApiService;
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
                _logger.LogInformation("Getting calidades from Velneo API");

                var calidades = await _velneoApiService.GetAllCalidadesAsync();

                _logger.LogInformation("Successfully retrieved {Count} calidades", calidades.Count());
                return Ok(calidades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calidades from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}