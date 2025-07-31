using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<CategoriaController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
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
                _logger.LogInformation("🚗 Getting categorias from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var categorias = await _velneoMaestrosService.GetAllCategoriasAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} categorias", categorias.Count());

                return Ok(new
                {
                    success = true,
                    data = categorias,
                    total = categorias.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting categorias from VelneoMaestrosService");
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