// RegularizadorPolizas.API/Controllers/CombustibleController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CombustibleController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<CombustibleController> _logger;

        public CombustibleController(
            IVelneoApiService velneoApiService,
            ILogger<CombustibleController> logger)
        {
            _velneoApiService = velneoApiService;
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
                _logger.LogInformation("Getting combustibles from Velneo API");

                var combustibles = await _velneoApiService.GetAllCombustiblesAsync();

                _logger.LogInformation("Successfully retrieved {Count} combustibles", combustibles.Count());
                return Ok(combustibles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting combustibles from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}