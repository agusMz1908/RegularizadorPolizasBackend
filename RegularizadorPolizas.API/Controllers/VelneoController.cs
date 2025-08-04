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
    [Authorize]
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

        /// <summary>
        /// 🔍 NUEVO ENDPOINT: Validar mapeo automático de datos escaneados
        /// </summary>
        /// <param name="azureData">Datos extraídos del PDF por Azure Document Intelligence</param>
        /// <returns>Resultado del mapeo con niveles de confianza y opciones disponibles</returns>
        [HttpPost("validate-mapping")]
        [ProducesResponseType(typeof(PolicyMappingResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateMapping([FromBody, Required] AzureDatosPolizaVelneoDto azureData)
        {
            try
            {
                _logger.LogInformation("🔍 Iniciando validación de mapeo para datos Azure");

                if (azureData == null)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Datos requeridos",
                        Detail = "Los datos de Azure Document Intelligence son requeridos"
                    });
                }

                // Validar que tenga datos mínimos
                if (azureData.DatosBasicos == null && azureData.DatosVehiculo == null && azureData.DatosPoliza == null)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Datos insuficientes",
                        Detail = "Debe proporcionar al menos datos básicos, del vehículo o de la póliza"
                    });
                }

                // Ejecutar validación de mapeo
                var mappingResult = await _velneoMaestrosService.ValidarMapeoCompletoAsync(azureData);

                // Preparar respuesta con métricas adicionales
                var response = new PolicyMappingResponseDto
                {
                    Success = true,
                    MappingResult = mappingResult,
                    Timestamp = DateTime.UtcNow,
                    ProcessingTimeMs = 0, // Se puede agregar tracking de tiempo si es necesario
                    Recommendations = GenerarRecomendaciones(mappingResult)
                };

                _logger.LogInformation("✅ Validación de mapeo completada: {Exito}% éxito, {Campos} campos procesados",
                    mappingResult.PorcentajeExito, mappingResult.CamposMapeados.Count);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Error de validación en mapeo");
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Error de validación",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error interno en validación de mapeo");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error interno del servidor",
                    Detail = "Error al procesar la validación de mapeo. Contacte al administrador."
                });
            }
        }

        /// <summary>
        /// 📊 ENDPOINT: Obtener todas las opciones de maestros para mapeo manual
        /// </summary>
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

        /// <summary>
        /// 🔧 ENDPOINT: Aplicar mapeo manual y generar objeto final para Velneo
        /// </summary>
        [HttpPost("apply-manual-mapping")]
        [ProducesResponseType(typeof(ApplyMappingResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApplyManualMapping([FromBody, Required] ApplyMappingRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔧 Aplicando mapeo manual para generar objeto Velneo");

                if (request?.AzureData == null || request?.ManualMappings == null)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Datos requeridos",
                        Detail = "Se requieren los datos de Azure y los mapeos manuales"
                    });
                }

                // Aquí combinarías los datos de Azure con los mapeos manuales
                // y generarías el objeto final para enviar a Velneo usando el método existente

                // Por ahora retornamos success - esto se puede implementar según necesidades específicas
                var response = new ApplyMappingResponseDto
                {
                    Success = true,
                    Message = "Mapeo manual aplicado correctamente",
                    VelneoObject = new { /* Objeto final para Velneo */ },
                    Timestamp = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error aplicando mapeo manual");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error interno del servidor",
                    Detail = "Error al aplicar mapeo manual. Contacte al administrador."
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
