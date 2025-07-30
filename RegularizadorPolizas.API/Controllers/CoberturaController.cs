using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoberturaController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<CoberturaController> _logger;

        public CoberturaController(
            IVelneoApiService velneoApiService,
            ILogger<CoberturaController> logger)
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CoberturaDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CoberturaDto>>> GetCoberturas()
        {
            try
            {
                _logger.LogInformation("🛡️ CoberturasController: Obteniendo coberturas...");

                var coberturas = await _velneoApiService.GetAllCoberturasAsync();

                _logger.LogInformation("✅ Coberturas obtenidas: {Count}", coberturas.Count());

                return Ok(coberturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo coberturas");
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<ActionResult> GetCoberturasLookup()
        {
            try
            {
                _logger.LogInformation("🛡️ CoberturasController: Obteniendo coberturas lookup...");

                var coberturas = await _velneoApiService.GetAllCoberturasAsync();

                var lookup = coberturas
                    .Where(c => c.Activo)
                    .Select(c => new {
                        id = c.Id,
                        descripcion = c.Descripcion,
                        codigo = c.Codigo
                    })
                    .OrderBy(c => c.descripcion);

                _logger.LogInformation("✅ Coberturas lookup obtenidas: {Count}", lookup.Count());

                return Ok(lookup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo coberturas lookup");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CoberturaDto>), 200)]
        public async Task<ActionResult<IEnumerable<CoberturaDto>>> SearchCoberturas([FromQuery] string filtro)
        {
            try
            {
                _logger.LogInformation("🔍 CoberturasController: Buscando coberturas con filtro: {Filtro}", filtro);

                if (string.IsNullOrWhiteSpace(filtro))
                {
                    return Ok(Enumerable.Empty<CoberturaDto>());
                }

                var coberturas = await _velneoApiService.GetAllCoberturasAsync();

                var filtradas = coberturas.Where(c =>
                    c.Descripcion.Contains(filtro, StringComparison.OrdinalIgnoreCase) &&
                    c.Activo);

                _logger.LogInformation("✅ Coberturas encontradas: {Count}", filtradas.Count());

                return Ok(filtradas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando coberturas");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}