using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Services;
using System.ComponentModel.DataAnnotations;
using static ClienteMatchResult;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolizaProcessingController : ControllerBase
    {
        private readonly IDocumentExtractionService _documentService;
        private readonly IClienteMatchingService _clienteMatchingService;
        private readonly ILogger<PolizaProcessingController> _logger;

        public PolizaProcessingController(
            IDocumentExtractionService documentService,
            IClienteMatchingService clienteMatchingService,
            ILogger<PolizaProcessingController> logger)
        {
            _documentService = documentService;
            _clienteMatchingService = clienteMatchingService;
            _logger = logger;
        }

        /// <summary>
        /// PASO 1: Procesar documento y extraer datos
        /// </summary>
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
        /// PASO 2: Buscar clientes con los datos extraídos
        /// </summary>
        [HttpPost("search-clients")]
        [ProducesResponseType(typeof(ClienteMatchResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClienteMatchResult>> SearchClients([FromBody] DatosClienteExtraidos datosCliente)
        {
            try
            {
                if (datosCliente == null)
                {
                    return BadRequest(new { error = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("🔄 PASO 2: Buscando clientes para '{Nombre}' - '{Documento}'",
                    datosCliente.Nombre, datosCliente.Documento);

                var result = await _clienteMatchingService.BuscarClienteAsync(datosCliente);

                _logger.LogInformation("✅ PASO 2 COMPLETADO: {TipoResultado} - {Count} matches encontrados",
                    result.TipoResultado, result.Matches.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando clientes");
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

        /// <summary>
        /// FLUJO COMPLETO: Procesar documento completo (para casos automáticos)
        /// </summary>
        [HttpPost("process-complete")]
        [ProducesResponseType(typeof(ProcessCompleteResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProcessCompleteResponse>> ProcessComplete([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔄 FLUJO COMPLETO: Procesando {FileName}", file.FileName);

                var response = new ProcessCompleteResponse
                {
                    NombreArchivo = file.FileName,
                    FechaInicio = DateTime.Now
                };

                // PASO 1: Extraer datos
                _logger.LogInformation("📄 Extrayendo datos del documento...");
                var extractResult = await _documentService.ProcessDocumentAsync(file);
                response.ExtraccionCompletada = true;
                response.DatosExtraidos = extractResult;

                if (extractResult.EstadoProcesamiento.Contains("ERROR"))
                {
                    response.Success = false;
                    response.Message = "Error extrayendo datos del documento";
                    return Ok(response);
                }

                // PASO 2: Buscar cliente automáticamente
                _logger.LogInformation("🔍 Buscando cliente automáticamente...");
                var clienteMatch = await _clienteMatchingService.BuscarClienteAsync(extractResult.DatosClienteBusqueda);
                response.BusquedaCompletada = true;
                response.ResultadoBusqueda = clienteMatch;

                // PASO 3: Decidir siguiente acción basado en resultado
                switch (clienteMatch.TipoResultado)
                {
                    case TipoResultadoCliente.MatchExacto:
                        // Crear automáticamente si hay match exacto
                        _logger.LogInformation("✅ Match exacto encontrado, creando póliza automáticamente...");
                        var polizaRequest = new CrearPolizaConClienteRequest
                        {
                            ClienteId = clienteMatch.Matches[0].Cliente.Id,
                            DatosPoliza = extractResult.DatosPoliza,
                            ArchivoOriginal = file.FileName,
                            ConfirmadoPorUsuario = false,
                            ObservacionesUsuario = "Creación automática por match exacto"
                        };

                        var polizaResult = await _documentService.CrearPolizaConClienteAsync(polizaRequest);
                        response.PolizaCreada = polizaResult.Success;
                        response.ResultadoPoliza = polizaResult;

                        if (polizaResult.Success)
                        {
                            response.Success = true;
                            response.Message = $"Póliza creada automáticamente para {clienteMatch.Matches[0].Cliente.Clinom}";
                            response.RequiereIntervencion = false;
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = $"Error creando póliza: {polizaResult.Message}";
                            response.RequiereIntervencion = true;
                        }
                        break;

                    case TipoResultadoCliente.MatchMuyProbable:
                        response.Success = true;
                        response.Message = $"Cliente muy probable encontrado: {clienteMatch.Matches[0].Cliente.Clinom}. Requiere confirmación.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "confirmar_cliente";
                        break;

                    case TipoResultadoCliente.MultiplesMatches:
                        response.Success = true;
                        response.Message = $"Se encontraron {clienteMatch.Matches.Count} clientes similares. Seleccione el correcto.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "seleccionar_cliente";
                        break;

                    case TipoResultadoCliente.SinCoincidencias:
                        response.Success = true;
                        response.Message = "No se encontraron clientes coincidentes. Considere crear un cliente nuevo.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "crear_cliente";
                        break;

                    default:
                        response.Success = true;
                        response.Message = "Se encontraron coincidencias parciales. Revise cuidadosamente.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "revisar_coincidencias";
                        break;
                }

                response.FechaFin = DateTime.Now;
                response.TiempoTotal = (response.FechaFin - response.FechaInicio).TotalMilliseconds;

                _logger.LogInformation("✅ FLUJO COMPLETO FINALIZADO: {FileName} - {Message}",
                    file.FileName, response.Message);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en flujo completo");

                return Ok(new ProcessCompleteResponse
                {
                    NombreArchivo = file.FileName,
                    Success = false,
                    Message = $"Error procesando documento: {ex.Message}",
                    RequiereIntervencion = true,
                    FechaInicio = DateTime.Now,
                    FechaFin = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Obtener información de un cliente específico para confirmación
        /// </summary>
        [HttpGet("client/{clienteId}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClientDto>> GetClientInfo(int clienteId)
        {
            try
            {
                // Esto debería usar tu servicio de clientes existente
                // Para este ejemplo, asumo que tienes acceso al servicio de Velneo
                _logger.LogInformation("🔍 Obteniendo información del cliente {ClienteId}", clienteId);

                // Aquí necesitarías inyectar ITenantAwareVelneoApiService
                // var cliente = await _velneoService.GetClienteAsync(clienteId);

                // Por ahora retorno un placeholder
                return StatusCode(501, new { message = "Implementar obtención de cliente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo cliente {ClienteId}", clienteId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Buscar clientes manualmente (para casos donde la búsqueda automática falla)
        /// </summary>
        [HttpGet("search-clients-manual")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> SearchClientsManual([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { error = "Término de búsqueda requerido" });
                }

                _logger.LogInformation("🔍 Búsqueda manual de clientes: '{SearchTerm}'", searchTerm);

                // Esto debería usar tu controlador de clientes existente
                // Por ahora retorno un placeholder
                return StatusCode(501, new { message = "Implementar búsqueda manual" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda manual");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para debug - ver datos extraídos en detalle
        /// </summary>
        [HttpPost("debug-extraction")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DebugExtraction([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔧 DEBUG: Analizando extracción de {FileName}", file.FileName);

                var extractResult = await _documentService.ProcessDocumentAsync(file);

                return Ok(new
                {
                    archivo = file.FileName,
                    timestamp = DateTime.Now,
                    extraccion = extractResult,
                    resumen = new
                    {
                        numeroPoliza = extractResult.DatosPoliza.NumeroPoliza,
                        cliente = extractResult.DatosClienteBusqueda.Nombre,
                        documento = extractResult.DatosClienteBusqueda.Documento,
                        vehiculo = extractResult.DatosPoliza.DescripcionVehiculo,
                        primaComercial = extractResult.DatosPoliza.PrimaComercial,
                        premioTotal = extractResult.DatosPoliza.PremioTotal,
                        vigenciaDesde = extractResult.DatosPoliza.VigenciaDesde,
                        vigenciaHasta = extractResult.DatosPoliza.VigenciaHasta,
                        advertencias = extractResult.Advertencias.Count,
                        confianza = extractResult.ConfianzaExtraccion
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en debug de extracción");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    // ===== MODELOS DE RESPONSE =====

    public class ProcessCompleteResponse
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RequiereIntervencion { get; set; }
        public string SiguientePaso { get; set; } = string.Empty;

        // Pasos del proceso
        public bool ExtraccionCompletada { get; set; }
        public bool BusquedaCompletada { get; set; }
        public bool PolizaCreada { get; set; }

        // Resultados de cada paso
        public DocumentExtractResult? DatosExtraidos { get; set; }
        public ClienteMatchResult? ResultadoBusqueda { get; set; }
        public CrearPolizaResponse? ResultadoPoliza { get; set; }

        // Metadatos
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public double TiempoTotal { get; set; } // en millisegundos

        // Información para el usuario
        public List<string> PasosCompletados => new()
        {
            ExtraccionCompletada ? "✅ Extracción de datos" : "⏳ Extracción de datos",
            BusquedaCompletada ? "✅ Búsqueda de cliente" : "⏳ Búsqueda de cliente",
            PolizaCreada ? "✅ Creación de póliza" : "⏳ Creación de póliza"
        };

        public string EstadoGeneral => Success
            ? (RequiereIntervencion ? "🟡 Requiere intervención manual" : "🟢 Completado automáticamente")
            : "🔴 Error en procesamiento";
    }
}