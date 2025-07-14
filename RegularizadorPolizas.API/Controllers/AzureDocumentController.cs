using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment; // ✅ Agregar
        private readonly ILogger<AzureDocumentController> _logger;

        public AzureDocumentController(
            IAzureDocumentIntelligenceService azureDocumentService,
            IProcessDocumentService processDocumentService,
            IPolizaService polizaService,
            IConfiguration configuration,
            IWebHostEnvironment hostEnvironment, // ✅ Agregar
            ILogger<AzureDocumentController> logger)
        {
            _azureDocumentService = azureDocumentService;
            _processDocumentService = processDocumentService;
            _polizaService = polizaService;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment; // ✅ Agregar
            _logger = logger;
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

                polizaCreada = await _polizaService.CreatePolizaAsync(polizaDto);

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
                            marcaModelo = polizaCreada.Conmaraut 
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
                        var polizaCreada = await _polizaService.CreatePolizaAsync(polizaDto);

                        resultados.Add(new
                        {
                            archivo = file.FileName,
                            polizaId = polizaCreada.Id,
                            numeroPoliza = polizaCreada.Conpol,
                            cliente = polizaCreada.Clinom,
                            confianza = documentResult.ConfianzaExtraccion,
                            requiereRevision = documentResult.RequiereRevision,
                            guardadoEn = saveToVelneo ? "Velneo" : "Local"
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
                    totalArchivos = files.Count,
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
        /// <summary>
        /// Test de conexión temporal sin autenticación (SOLO PARA DESARROLLO)
        /// </summary>
        [HttpGet("test-connection-no-auth")]
        [Authorize] // ⚠️ TEMPORAL - Solo para testing inicial
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> TestConnectionNoAuth()
        {
            try
            {
                _logger.LogInformation("Testing Azure Document Intelligence connection (no auth)");

                var isConnected = await _azureDocumentService.TestConnectionAsync();

                if (isConnected)
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "Conexión exitosa con Azure Document Intelligence",
                        endpoint = "https://extraccion-polizas.cognitiveservices.azure.com/",
                        modelId = "poliza-vehiculo-bse",
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
                _logger.LogError(ex, "Error testing Azure connection (no auth)");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Error al probar la conexión",
                    details = ex.Message
                });
            }
        }
        /// <summary>
        /// Debug: Verificar configuración de Azure (TEMPORAL)
        /// </summary>
        [HttpGet("debug-config")]
        [AllowAnonymous]
        public ActionResult DebugConfig()
        {
            try
            {
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                return Ok(new
                {
                    hasEndpoint = !string.IsNullOrEmpty(endpoint),
                    hasApiKey = !string.IsNullOrEmpty(apiKey),
                    hasModelId = !string.IsNullOrEmpty(modelId),
                    endpointValue = endpoint,
                    apiKeyLength = apiKey?.Length ?? 0,
                    modelIdValue = modelId,
                    configurationKeys = _configuration.AsEnumerable()
                        .Where(x => x.Key.StartsWith("AzureDocumentIntelligence"))
                        .Select(x => new { x.Key, HasValue = !string.IsNullOrEmpty(x.Value) })
                        .ToList()
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        /// <summary>
        /// Debug detallado de configuración Azure
        /// </summary>
        [HttpGet("debug-azure-config")]
        [AllowAnonymous]
        public ActionResult DebugAzureConfig()
        {
            try
            {
                // Leer directamente de la configuración
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    configuration = new
                    {
                        endpoint = new
                        {
                            value = endpoint,
                            hasValue = !string.IsNullOrEmpty(endpoint),
                            length = endpoint?.Length ?? 0
                        },
                        apiKey = new
                        {
                            hasValue = !string.IsNullOrEmpty(apiKey),
                            length = apiKey?.Length ?? 0,
                            firstChars = apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0)) ?? "null",
                            lastChars = apiKey?.Length > 10 ? apiKey.Substring(apiKey.Length - 10) : "N/A"
                        },
                        modelId = new
                        {
                            value = modelId,
                            hasValue = !string.IsNullOrEmpty(modelId)
                        }
                    },
                    allAzureKeys = _configuration.AsEnumerable()
                        .Where(x => x.Key.Contains("Azure", StringComparison.OrdinalIgnoreCase))
                        .Select(x => new {
                            Key = x.Key,
                            HasValue = !string.IsNullOrEmpty(x.Value),
                            ValueLength = x.Value?.Length ?? 0
                        })
                        .ToList(),
                    environmentInfo = new
                    {
                        environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                        contentRootPath = _hostEnvironment?.ContentRootPath,
                        configFiles = Directory.Exists(_hostEnvironment?.ContentRootPath)
                            ? Directory.GetFiles(_hostEnvironment.ContentRootPath, "appsettings*.json")
                            : new string[0]
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace?.Split('\n').Take(10).ToArray()
                });
            }
        }
    }
}