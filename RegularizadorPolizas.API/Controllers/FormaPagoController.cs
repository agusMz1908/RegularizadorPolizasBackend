using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormaPagoController : ControllerBase
    {
        public class FormasPagoController : ControllerBase
        {
            private readonly IVelneoApiService _velneoApiService;
            private readonly ILogger<FormaPagoController> _logger;

            public FormasPagoController(
                IVelneoApiService velneoApiService,
                ILogger<FormaPagoController> logger)
            {
                _velneoApiService = velneoApiService;
                _logger = logger;
            }

            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<FormaPagoDto>), 200)]
            [ProducesResponseType(401)]
            [ProducesResponseType(500)]
            public async Task<ActionResult<IEnumerable<FormaPagoDto>>> GetFormasPago()
            {
                try
                {
                    _logger.LogInformation("💳 FormasPagoController: Obteniendo formas de pago...");

                    var formasPago = await _velneoApiService.GetAllFormasPagoAsync();

                    _logger.LogInformation("✅ Formas de pago obtenidas: {Count}", formasPago.Count());

                    return Ok(formasPago.OrderBy(f => f.Descripcion));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error obteniendo formas de pago");
                    return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
                }
            }

            /// <summary>
            /// Obtiene formas de pago para lookup/select
            /// </summary>
            /// <returns>Lista simplificada de formas de pago</returns>
            [HttpGet("lookup")]
            [ProducesResponseType(typeof(IEnumerable<object>), 200)]
            public async Task<ActionResult> GetFormasPagoLookup()
            {
                try
                {
                    _logger.LogInformation("💳 FormasPagoController: Obteniendo formas de pago lookup...");

                    var formasPago = await _velneoApiService.GetAllFormasPagoAsync();

                    var lookup = formasPago
                        .Where(f => f.Activo)
                        .Select(f => new
                        {
                            id = f.Id,
                            descripcion = f.Descripcion,
                            codigo = f.Codigo
                        })
                        .OrderBy(f => f.descripcion);

                    _logger.LogInformation("✅ Formas de pago lookup obtenidas: {Count}", lookup.Count());

                    return Ok(lookup);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error obteniendo formas de pago lookup");
                    return StatusCode(500, new { error = "Error interno del servidor" });
                }
            }

            /// <summary>
            /// Busca formas de pago por texto
            /// </summary>
            /// <param name="filtro">Texto a buscar</param>
            /// <returns>Formas de pago filtradas</returns>
            [HttpGet("search")]
            [ProducesResponseType(typeof(IEnumerable<FormaPagoDto>), 200)]
            public async Task<ActionResult<IEnumerable<FormaPagoDto>>> SearchFormasPago([FromQuery] string filtro)
            {
                try
                {
                    _logger.LogInformation("🔍 FormasPagoController: Buscando formas de pago con filtro: {Filtro}", filtro);

                    if (string.IsNullOrWhiteSpace(filtro))
                    {
                        return Ok(Enumerable.Empty<FormaPagoDto>());
                    }

                    var formasPago = await _velneoApiService.GetAllFormasPagoAsync();

                    var filtradas = formasPago.Where(f =>
                        f.Descripcion.Contains(filtro, StringComparison.OrdinalIgnoreCase) &&
                        f.Activo);

                    _logger.LogInformation("✅ Formas de pago encontradas: {Count}", filtradas.Count());

                    return Ok(filtradas);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error buscando formas de pago");
                    return StatusCode(500, new { error = "Error interno del servidor" });
                }
            }
        }
    }
}