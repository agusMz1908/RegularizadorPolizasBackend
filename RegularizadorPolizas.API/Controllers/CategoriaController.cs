using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(
            IVelneoApiService velneoApiService,
            ILogger<CategoriaController> logger)
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAllCategorias()
        {
            try
            {
                _logger.LogInformation("Getting categorias from Velneo API");

                var categorias = await _velneoApiService.GetAllCategoriasAsync();

                _logger.LogInformation("Successfully retrieved {Count} categorias", categorias.Count());
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categorias from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}