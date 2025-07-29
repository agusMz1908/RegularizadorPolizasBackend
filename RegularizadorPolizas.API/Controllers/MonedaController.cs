using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonedaController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<MonedaController> _logger;

        public MonedaController(
            IVelneoApiService velneoApiService,
            ILogger<MonedaController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
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
                _logger.LogInformation("Getting all monedas from Velneo API");

                var monedas = await _velneoApiService.GetAllMonedasAsync();

                if (monedas == null || !monedas.Any())
                {
                    _logger.LogWarning("No monedas found in Velneo API");
                    return NotFound("No se encontraron monedas en Velneo");
                }

                _logger.LogInformation("Successfully retrieved {Count} monedas", monedas.Count());
                return Ok(monedas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monedas from Velneo API");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
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
                _logger.LogInformation("Getting active monedas from Velneo API");

                var monedas = await _velneoApiService.GetAllMonedasAsync();
                // Como Velneo solo devuelve monedas activas, devolvemos todas
                var monedasActivas = monedas?.Where(m => m.Activa).ToList() ?? monedas?.ToList();

                if (monedasActivas == null || !monedasActivas.Any())
                {
                    _logger.LogWarning("No active monedas found in Velneo API");
                    return NotFound("No se encontraron monedas activas en Velneo");
                }

                _logger.LogInformation("Successfully retrieved {Count} active monedas", monedasActivas.Count);
                return Ok(monedasActivas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active monedas from Velneo API");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}