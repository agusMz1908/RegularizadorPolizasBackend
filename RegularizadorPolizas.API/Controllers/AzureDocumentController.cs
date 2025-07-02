using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AzureDocumentController : ControllerBase
    {
        private readonly IAzureDocumentIntelligenceService _azureDocumentService;
        private readonly IProcessDocumentService _processDocumentService;
        private readonly IPolizaService _polizaService;
        private readonly ILogger<AzureDocumentController> _logger;

        public AzureDocumentController(
            IAzureDocumentIntelligenceService azureDocumentService,
            IProcessDocumentService processDocumentService,
            IPolizaService polizaService,
            ILogger<AzureDocumentController> logger)
        {
            _azureDocumentService = azureDocumentService ?? throw new ArgumentNullException(nameof(azureDocumentService));
            _processDocumentService = processDocumentService ?? throw new ArgumentNullException(nameof(processDocumentService));
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("process")]
        [ProducesResponseType(typeof(DocumentResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentResultDto>> ProcessDocument([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new
                    {
                        error = "Tipo de archivo no soportado",
                        allowed = allowedExtensions,
                        received = fileExtension
                    });
                }

                if (file.Length > 20 * 1024 * 1024)
                {
                    return BadRequest(new { error = "El archivo es demasiado grande (máximo 20MB)" });
                }

                _logger.LogInformation("Processing document: {FileName}, Size: {FileSize} bytes",
                    file.FileName, file.Length);

                var resultado = await _azureDocumentService.ProcessDocumentAsync(file);

                _logger.LogInformation("Document processed successfully: {FileName}, Status: {Status}",
                    file.FileName, resultado.EstadoProcesamiento);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document: {FileName}", file?.FileName ?? "unknown");

                return StatusCode(500, new
                {
                    error = "Error interno del servidor al procesar el documento",
                    details = ex.Message
                });
            }
        }

        [HttpPost("process-and-create-policy")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ProcessDocumentAndCreatePolicy([Required] IFormFile file, [FromForm] bool saveToVelneo = true)
        {
            try
            {
                var documentResult = await _azureDocumentService.ProcessDocumentAsync(file);

                if (documentResult.EstadoProcesamiento == "ERROR")
                {
                    return BadRequest(new
                    {
                        error = "Error al procesar el documento",
                        details = documentResult.MensajeError
                    });
                }

                var polizaDto = _azureDocumentService.MapDocumentToPoliza(documentResult);

                PolizaDto polizaCreada;
                if (saveToVelneo)
                {
                    polizaCreada = await _polizaService.CreatePolizaAsync(polizaDto);
                }
                else
                {
                    polizaCreada = await _polizaService.CreatePolizaLocalAsync(polizaDto);
                }

                var resultado = new
                {
                    success = true,
                    message = "Documento procesado y póliza creada exitosamente",
                    document = new
                    {
                        nombre = documentResult.NombreArchivo,
                        estado = documentResult.EstadoProcesamiento,
                        confianza = documentResult.ConfianzaExtraccion,
                        requiereRevision = documentResult.RequiereRevision,
                        tiempoProcesamiento = documentResult.TiempoProcesamiento
                    },
                    poliza = new
                    {
                        id = polizaCreada.Id,
                        numero = polizaCreada.Conpol,
                        cliente = polizaCreada.Clinom,
                        vehiculo = new
                        {
                            matricula = polizaCreada.Conmataut,
                            marca = polizaCreada.Conmaraut,
                            modelo = polizaCreada.conmodaut
                        },
                        montos = new
                        {
                            premio = polizaCreada.Conpremio,
                            total = polizaCreada.Contot
                        },
                        fechas = new
                        {
                            desde = polizaCreada.Confchdes,
                            hasta = polizaCreada.Confchhas
                        },
                        guardadoEn = saveToVelneo ? "Velneo" : "Local"
                    }
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document and creating policy: {FileName}", file?.FileName ?? "unknown");

                return StatusCode(500, new
                {
                    error = "Error al procesar documento y crear póliza",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene información del modelo de Azure Document Intelligence
        /// </summary>
        [HttpGet("model-info")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetModelInfo()
        {
            try
            {
                var modelInfo = await _azureDocumentService.GetModelInfoAsync();
                return Ok(new { modelInfo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting model info");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Prueba la conexión con Azure Document Intelligence
        /// </summary>
        [HttpGet("test-connection")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var isConnected = await _azureDocumentService.TestConnectionAsync();

                if (isConnected)
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "Conexión exitosa con Azure Document Intelligence",
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "error",
                        message = "No se pudo conectar con Azure Document Intelligence"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error al probar la conexión",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Endpoint para debugging - muestra los campos extraídos sin procesarlos
        /// </summary>
        [HttpPost("debug-extract")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DebugExtractFields([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                var resultado = await _azureDocumentService.ProcessDocumentAsync(file);

                return Ok(new
                {
                    archivo = resultado.NombreArchivo,
                    estado = resultado.EstadoProcesamiento,
                    confianza = resultado.ConfianzaExtraccion,
                    camposExtraidos = resultado.CamposExtraidos,
                    requiereRevision = resultado.RequiereRevision,
                    polizaProcesada = resultado.PolizaProcesada,
                    tiempoProcesamiento = $"{resultado.TiempoProcesamiento}ms"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug extract: {FileName}", file?.FileName ?? "unknown");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Procesa múltiples documentos en lote
        /// </summary>
        [HttpPost("process-batch")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ProcessDocumentsBatch([Required] List<IFormFile> files, [FromForm] bool saveToVelneo = true)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { error = "No se han proporcionado archivos" });
                }

                if (files.Count > 10)
                {
                    return BadRequest(new { error = "Máximo 10 archivos por lote" });
                }

                var resultados = new List<object>();
                var errores = new List<object>();

                foreach (var file in files)
                {
                    try
                    {
                        var documentResult = await _azureDocumentService.ProcessDocumentAsync(file);

                        if (documentResult.EstadoProcesamiento == "ERROR")
                        {
                            errores.Add(new
                            {
                                archivo = file.FileName,
                                error = documentResult.MensajeError
                            });
                            continue;
                        }

                        var polizaDto = _azureDocumentService.MapDocumentToPoliza(documentResult);

                        PolizaDto polizaCreada;
                        if (saveToVelneo)
                        {
                            polizaCreada = await _polizaService.CreatePolizaAsync(polizaDto);
                        }
                        else
                        {
                            polizaCreada = await _polizaService.CreatePolizaLocalAsync(polizaDto);
                        }

                        resultados.Add(new
                        {
                            archivo = file.FileName,
                            polizaId = polizaCreada.Id,
                            numeroPoliza = polizaCreada.Conpol,
                            cliente = polizaCreada.Clinom,
                            confianza = documentResult.ConfianzaExtraccion,
                            requiereRevision = documentResult.RequiereRevision
                        });
                    }
                    catch (Exception ex)
                    {
                        errores.Add(new
                        {
                            archivo = file.FileName,
                            error = ex.Message
                        });
                    }
                }

                return Ok(new
                {
                    procesados = resultados.Count,
                    errores = errores.Count,
                    resultados,
                    erroresDetalle = errores
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document batch");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}