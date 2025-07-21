using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using static RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence.SmartDocumentParser;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolizaProcessingController : ControllerBase
    {
        private readonly IDocumentExtractionService _documentService;
        private readonly ILogger<PolizaProcessingController> _logger;

        public PolizaProcessingController(
            IDocumentExtractionService documentService,
            ILogger<PolizaProcessingController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [HttpPost("extract-document")]
        [ProducesResponseType(typeof(DocumentExtractResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentExtractResult>> ExtractDocument([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔄 PASO 1: Extrayendo datos de documento {FileName}", file.FileName);

                var result = await _documentService.ProcessDocumentAsync(file);

                _logger.LogInformation("✅ PASO 1 COMPLETADO: Datos extraídos de {FileName}", file.FileName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error extrayendo datos del documento");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// PASO 3: Crear póliza con cliente seleccionado
        /// </summary>
        [HttpPost("create-poliza")]
        [ProducesResponseType(typeof(CrearPolizaResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CrearPolizaResponse>> CreatePoliza([FromBody] CrearPolizaConClienteRequest request)
        {
            try
            {
                if (request == null || request.ClienteId <= 0 || request.DatosPoliza == null)
                {
                    return BadRequest(new { error = "Datos de solicitud inválidos" });
                }

                _logger.LogInformation("🔄 PASO 3: Creando póliza {NumeroPoliza} para cliente {ClienteId}",
                    request.DatosPoliza.NumeroPoliza, request.ClienteId);

                var result = await _documentService.CrearPolizaConClienteAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("✅ PASO 3 COMPLETADO: Póliza {NumeroPoliza} creada exitosamente",
                        request.DatosPoliza.NumeroPoliza);
                }
                else
                {
                    _logger.LogWarning("⚠️ PASO 3 FALLIDO: {Message}", result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creando póliza");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }


    public class ConfirmClientRequest
    {
        public int ClienteId { get; set; }
        public SmartExtractedData DatosPoliza { get; set; } = new();
        public bool SeleccionManual { get; set; } = true;
        public string? Observaciones { get; set; }
    }

    public class ProcessBasicResponse
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RequiereIntervencion { get; set; }
        public string SiguientePaso { get; set; } = string.Empty;

        public bool ExtraccionCompletada { get; set; }
        public bool BusquedaCompletada { get; set; }

        public DocumentExtractResult? DatosExtraidos { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public double TiempoTotal { get; set; }

        public string EstadoGeneral => Success
            ? (RequiereIntervencion ? "Requiere intervención manual" : "Listo para automatizar")
            : "Error en procesamiento";
    }
}
