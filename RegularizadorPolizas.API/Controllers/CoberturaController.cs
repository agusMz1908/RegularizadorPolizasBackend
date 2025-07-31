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
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<CoberturaController> _logger;

        public CoberturaController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<CoberturaController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
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
                _logger.LogInformation("🛡️ Getting coberturas from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var coberturas = await _velneoMaestrosService.GetAllCoberturasAsync();

                _logger.LogInformation("✅ Coberturas obtenidas: {Count}", coberturas.Count());

                return Ok(new
                {
                    success = true,
                    data = coberturas,
                    total = coberturas.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo coberturas");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Error interno del servidor",
                    details = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<ActionResult> GetCoberturasLookup()
        {
            try
            {
                _logger.LogInformation("🛡️ Getting coberturas lookup from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var coberturas = await _velneoMaestrosService.GetAllCoberturasAsync();

                var lookup = coberturas
                    .Where(c => c.Activo)
                    .Select(c => new {
                        id = c.Id,
                        descripcion = c.Descripcion,
                        codigo = c.Codigo
                    })
                    .OrderBy(c => c.descripcion);

                _logger.LogInformation("✅ Coberturas lookup obtenidas: {Count}", lookup.Count());

                return Ok(new
                {
                    success = true,
                    data = lookup,
                    total = lookup.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo coberturas lookup");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Error interno del servidor",
                    details = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CoberturaDto>), 200)]
        public async Task<ActionResult<IEnumerable<CoberturaDto>>> SearchCoberturas([FromQuery] string filtro)
        {
            try
            {
                _logger.LogInformation("🔍 Searching coberturas with filter: {Filtro}", filtro);

                if (string.IsNullOrWhiteSpace(filtro))
                {
                    return Ok(new
                    {
                        success = true,
                        data = Enumerable.Empty<CoberturaDto>(),
                        total = 0,
                        message = "Filtro vacío - sin resultados",
                        timestamp = DateTime.UtcNow
                    });
                }

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var coberturas = await _velneoMaestrosService.GetAllCoberturasAsync();

                var filtradas = coberturas.Where(c =>
                    c.Descripcion.Contains(filtro, StringComparison.OrdinalIgnoreCase) &&
                    c.Activo);

                _logger.LogInformation("✅ Coberturas encontradas: {Count}", filtradas.Count());

                return Ok(new
                {
                    success = true,
                    data = filtradas,
                    total = filtradas.Count(),
                    filtro = filtro,
                    message = $"Se encontraron {filtradas.Count()} coberturas con el filtro '{filtro}'",
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando coberturas");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Error interno del servidor",
                    details = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}