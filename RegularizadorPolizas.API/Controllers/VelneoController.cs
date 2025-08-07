using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VelneoController : ControllerBase
    {
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<VelneoController> _logger;

        public VelneoController(
            IVelneoMaestrosService velneoMaestrosService,
            ILogger<VelneoController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("mapping-options")]
        [ProducesResponseType(typeof(MasterDataOptionsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMappingOptions()
        {
            try
            {
                _logger.LogInformation("📊 Obteniendo opciones de maestros para mapeo");

                var options = new MasterDataOptionsDto
                {
                    Categorias = (await _velneoMaestrosService.GetAllCategoriasAsync()).ToList(),
                    Destinos = (await _velneoMaestrosService.GetAllDestinosAsync()).ToList(),
                    Calidades = (await _velneoMaestrosService.GetAllCalidadesAsync()).ToList(),
                    Combustibles = (await _velneoMaestrosService.GetAllCombustiblesAsync()).ToList(),
                    Monedas = (await _velneoMaestrosService.GetAllMonedasAsync()).ToList(),

                    // Opciones de texto plano
                    EstadosPoliza = new[] { "VIG", "ANT", "VEN", "END", "ELIM", "FIN" },
                    TiposTramite = new[] { "Nuevo", "Renovación", "Cambio", "Endoso", "No Renueva", "Cancelación" },
                    EstadosBasicos = new[] { "Pendiente", "Pendiente c/plazo", "Terminado", "En proceso",
                                            "Modificaciones", "En emisión", "Enviado a cía", "Enviado a cía x mail",
                                            "Devuelto a ejecutivo", "Declinado" },
                    TiposLinea = new[] { "Líneas personales", "Líneas comerciales" },
                    FormasPago = new[] { "Contado", "Tarjeta de Crédito", "Débito Automático", "Cuotas", "Financiado" }
                };

                _logger.LogInformation("✅ Opciones de maestros obtenidas: {Categorias} categorías, {Destinos} destinos, etc.",
                    options.Categorias.Count, options.Destinos.Count);

                return Ok(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo opciones de maestros");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error interno del servidor",
                    Detail = "Error al obtener opciones de maestros. Contacte al administrador."
                });
            }
        }

        private List<string> GenerarRecomendaciones(PolicyMappingResultDto mappingResult)
        {
            var recomendaciones = new List<string>();

            if (mappingResult.PorcentajeExito >= 90)
            {
                recomendaciones.Add("✅ Excelente mapeo automático. Los datos están listos para envío a Velneo.");
            }
            else if (mappingResult.PorcentajeExito >= 70)
            {
                recomendaciones.Add("⚠️ Buen mapeo automático. Revise los campos con baja confianza antes del envío.");
            }
            else
            {
                recomendaciones.Add("🔍 Mapeo automático limitado. Se requiere validación manual de varios campos.");
            }

            if (mappingResult.CamposQueFallaronMapeo.Any())
            {
                recomendaciones.Add($"📝 {mappingResult.CamposQueFallaronMapeo.Count} campos requieren selección manual: {string.Join(", ", mappingResult.CamposQueFallaronMapeo)}");
            }

            if (mappingResult.CamposConBajaConfianza > 0)
            {
                recomendaciones.Add($"⚡ {mappingResult.CamposConBajaConfianza} campos con baja confianza necesitan revisión.");
            }

            return recomendaciones;
        }
    }
}
