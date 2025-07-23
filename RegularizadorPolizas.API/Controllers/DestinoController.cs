using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DestinoController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<DestinoController> _logger;

        public DestinoController(
            IVelneoApiService velneoApiService,
            ILogger<DestinoController> logger)
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DestinoDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DestinoDto>>> GetAllDestinos()
        {
            try
            {
                _logger.LogInformation("Getting destinos from Velneo API");

                var destinos = await _velneoApiService.GetAllDestinosAsync();

                _logger.LogInformation("Successfully retrieved {Count} destinos", destinos.Count());
                return Ok(destinos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting destinos from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}