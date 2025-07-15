using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
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

        /// <summary>
        /// Debug completo - Lista todos los modelos disponibles y encuentra el correcto
        /// </summary>
        [HttpGet("debug-all-models")]
        [AllowAnonymous] // Temporal para debugging
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> DebugAllModels()
        {
            try
            {
                // Si tu AzureDocumentIntelligenceService no tiene el método DebugAllModelsAsync,
                // puedes usar este código directamente aquí:

                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                var results = new List<string>();

                results.Add("=== AZURE DOCUMENT INTELLIGENCE COMPLETE DEBUG ===");
                results.Add($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                results.Add($"Configured Endpoint: {endpoint}");
                results.Add($"Configured Model ID: {modelId}");
                results.Add($"API Key Length: {apiKey?.Length ?? 0}");
                results.Add("");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                // Probar diferentes endpoints para listar modelos
                var listEndpoints = new[]
                {
            ($"{endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2023-07-31", "Document Intelligence v3.1"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/documentModels?api-version=2022-08-31", "Form Recognizer v3.0"),
            ($"{endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2024-02-29-preview", "Document Intelligence Preview"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models", "Form Recognizer v2.1 (Legacy)")
        };

                foreach (var (url, name) in listEndpoints)
                {
                    results.Add($"--- Testing {name} ---");
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            results.Add($"✅ SUCCESS with {name}");

                            // Parsear y mostrar modelos disponibles
                            try
                            {
                                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);

                                // Diferentes estructuras según la API
                                var models = jsonResponse?.value ?? jsonResponse?.modelList;

                                if (models != null)
                                {
                                    results.Add("Available models:");
                                    foreach (var model in models)
                                    {
                                        var modelIdFound = model?.modelId?.ToString() ?? model?.modelInfo?.modelId?.ToString();
                                        var description = model?.description?.ToString() ?? model?.modelInfo?.description?.ToString() ?? "No description";
                                        var status = model?.status?.ToString() ?? model?.modelInfo?.status?.ToString() ?? "Unknown";

                                        results.Add($"  - {modelIdFound} | Status: {status} | Desc: {description}");

                                        // Marcar si es nuestro modelo
                                        if (modelIdFound == modelId)
                                        {
                                            results.Add($"    ⭐ THIS IS YOUR MODEL! Use API: {name}");
                                        }
                                    }
                                }
                                else
                                {
                                    results.Add($"No 'value' or 'modelList' property found. Raw structure:");
                                    results.Add($"{content.Substring(0, Math.Min(500, content.Length))}...");
                                }
                            }
                            catch (Exception parseEx)
                            {
                                results.Add($"Parse error: {parseEx.Message}");
                                results.Add($"Raw response: {content.Substring(0, Math.Min(500, content.Length))}...");
                            }
                        }
                        else
                        {
                            results.Add($"❌ {name}: {response.StatusCode} - {response.ReasonPhrase}");
                            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                results.Add("   → Check your API key");
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                results.Add("   → This API version/endpoint is not available");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"❌ {name}: ERROR - {ex.Message}");
                    }
                    results.Add("");
                }

                // Probar acceso directo al modelo específico
                results.Add("--- TESTING DIRECT MODEL ACCESS ---");
                var modelEndpoints = new[]
                {
            ($"{endpoint.TrimEnd('/')}/documentintelligence/documentModels/{modelId}?api-version=2023-07-31", "Document Intelligence v3.1"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/documentModels/{modelId}?api-version=2022-08-31", "Form Recognizer v3.0"),
            ($"{endpoint.TrimEnd('/')}/documentintelligence/documentModels/{modelId}?api-version=2024-02-29-preview", "Document Intelligence Preview"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{modelId}", "Form Recognizer v2.1 (Legacy)")
        };

                foreach (var (url, name) in modelEndpoints)
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            results.Add($"✅ DIRECT ACCESS SUCCESS: {name}");
                            results.Add($"   URL: {url}");
                        }
                        else
                        {
                            results.Add($"❌ DIRECT ACCESS FAILED: {name} - {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"❌ DIRECT ACCESS ERROR: {name} - {ex.Message}");
                    }
                }

                return Ok(new
                {
                    debugResults = string.Join("\n", results),
                    timestamp = DateTime.UtcNow,
                    summary = new
                    {
                        endpoint = endpoint,
                        modelId = modelId,
                        apiKeyConfigured = !string.IsNullOrEmpty(apiKey)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during complete debug");
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("test-corrected-config")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> TestCorrectedConfig()
        {
            try
            {
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelIdOriginal = _configuration["AzureDocumentIntelligence:ModelId"];

                // Corregir el model ID quitando la "A"
                var modelId = "poliza_vehiculos_bse"; // Nombre correcto

                _logger.LogInformation("Testing with corrected configuration...");
                _logger.LogInformation("Original Model ID: {OriginalModelId}", modelIdOriginal);
                _logger.LogInformation("Corrected Model ID: {CorrectedModelId}", modelId);

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var results = new List<object>();

                // Probar diferentes combinaciones de endpoints y versiones de API
                var endpointsToTry = new[]
                {
            // Azure Document Intelligence (más nuevos)
            ($"{endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2023-07-31", "Document Intelligence v3.1", "documentintelligence"),
            ($"{endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2024-02-29-preview", "Document Intelligence v4 Preview", "documentintelligence"),
            
            // Azure AI Document Intelligence (nombre actualizado)
            ($"{endpoint.TrimEnd('/')}/documentIntelligence/documentModels?api-version=2023-07-31", "AI Document Intelligence v3.1", "documentIntelligence"),
            ($"{endpoint.TrimEnd('/')}/documentIntelligence/documentModels?api-version=2024-02-29-preview", "AI Document Intelligence Preview", "documentIntelligence"),
            
            // Form Recognizer (legacy pero aún usado)
            ($"{endpoint.TrimEnd('/')}/formrecognizer/documentModels?api-version=2022-08-31", "Form Recognizer v3.0", "formrecognizer"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/documentModels?api-version=2023-07-31", "Form Recognizer v3.1", "formrecognizer"),
            
            // Versiones legacy
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models", "Form Recognizer v2.1 Legacy", "formrecognizer/v2.1")
        };

                foreach (var (url, name, apiPath) in endpointsToTry)
                {
                    try
                    {
                        _logger.LogInformation("Testing: {Name} - {Url}", name, url);
                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            // Verificar si nuestro modelo corregido está en la lista
                            bool modelFound = content.Contains(modelId);
                            bool originalModelFound = content.Contains(modelIdOriginal);

                            // Intentar parsear los modelos
                            var modelsInfo = new List<object>();
                            try
                            {
                                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);
                                var models = jsonResponse?.value ?? jsonResponse?.modelList;

                                if (models != null)
                                {
                                    foreach (var model in models)
                                    {
                                        var foundModelId = model?.modelId?.ToString() ?? model?.modelInfo?.modelId?.ToString();
                                        modelsInfo.Add(new
                                        {
                                            modelId = foundModelId,
                                            description = model?.description?.ToString() ?? model?.modelInfo?.description?.ToString() ?? "No description",
                                            status = model?.status?.ToString() ?? model?.modelInfo?.status?.ToString() ?? "Unknown"
                                        });
                                    }
                                }
                            }
                            catch (Exception parseEx)
                            {
                                _logger.LogWarning("Error parsing models: {Error}", parseEx.Message);
                            }

                            // Probar acceso directo al modelo si lo encontramos
                            object directAccess = null;
                            if (modelFound)
                            {
                                directAccess = await TestDirectModelAccess(httpClient, endpoint, apiPath, modelId, url);
                            }

                            results.Add(new
                            {
                                api = name,
                                url = url,
                                success = true,
                                statusCode = response.StatusCode.ToString(),
                                correctedModelFound = modelFound,
                                originalModelFound = originalModelFound,
                                totalModels = modelsInfo.Count,
                                models = modelsInfo,
                                directAccess = directAccess
                            });
                        }
                        else
                        {
                            results.Add(new
                            {
                                api = name,
                                url = url,
                                success = false,
                                statusCode = response.StatusCode.ToString(),
                                reason = response.ReasonPhrase,
                                correctedModelFound = false,
                                originalModelFound = false,
                                totalModels = 0,
                                models = new List<object>(),
                                directAccess = (object)null
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            api = name,
                            url = url,
                            success = false,
                            error = ex.Message,
                            correctedModelFound = false,
                            originalModelFound = false,
                            totalModels = 0,
                            models = new List<object>(),
                            directAccess = (object)null
                        });
                    }
                }

                // Buscar APIs que funcionaron
                var workingApis = results.Where(r =>
                {
                    var result = (dynamic)r;
                    return result.success == true;
                }).ToList();

                var modelFoundIn = results.Where(r =>
                {
                    var result = (dynamic)r;
                    return result.success == true && result.correctedModelFound == true;
                }).ToList();

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    originalModelId = modelIdOriginal,
                    correctedModelId = modelId,
                    endpointTested = endpoint,
                    results = results,
                    summary = new
                    {
                        workingApis = workingApis.Count,
                        modelFoundInApis = modelFoundIn.Count,
                        recommendation = GetRecommendation(workingApis.Count, modelFoundIn.Count, modelId)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing corrected configuration");
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private async Task<object> TestDirectModelAccess(HttpClient httpClient, string endpoint, string apiPath, string modelId, string listUrl)
        {
            try
            {
                // Construir URL de acceso directo basada en la API que funcionó
                string directUrl;

                if (apiPath.Contains("documentintelligence") || apiPath.Contains("documentIntelligence"))
                {
                    var version = listUrl.Contains("2024-02-29") ? "2024-02-29-preview" : "2023-07-31";
                    directUrl = $"{endpoint.TrimEnd('/')}/{apiPath}/documentModels/{modelId}?api-version={version}";
                }
                else if (apiPath.Contains("formrecognizer/v2.1"))
                {
                    directUrl = $"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{modelId}";
                }
                else
                {
                    var version = listUrl.Contains("2023-07-31") ? "2023-07-31" : "2022-08-31";
                    directUrl = $"{endpoint.TrimEnd('/')}/formrecognizer/documentModels/{modelId}?api-version={version}";
                }

                var response = await httpClient.GetAsync(directUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return new
                    {
                        url = directUrl,
                        success = true,
                        statusCode = response.StatusCode.ToString(),
                        contentPreview = content.Length > 200 ? content.Substring(0, 200) + "..." : content
                    };
                }
                else
                {
                    return new
                    {
                        url = directUrl,
                        success = false,
                        statusCode = response.StatusCode.ToString(),
                        reason = response.ReasonPhrase
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    error = ex.Message,
                    success = false
                };
            }
        }

        private string GetRecommendation(int workingApis, int modelFoundIn, string modelId)
        {
            if (workingApis == 0)
            {
                return "❌ No hay APIs funcionando. Verificar API key y endpoint.";
            }

            if (modelFoundIn == 0)
            {
                return $"⚠️ Las APIs funcionan pero el modelo '{modelId}' no se encuentra. Verificar el nombre del modelo en Azure.";
            }

            return $"✅ Modelo '{modelId}' encontrado y accesible. Configuración lista para usar.";
        }

        [HttpGet("discover-correct-endpoint")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> DiscoverCorrectEndpoint()
        {
            try
            {
                var baseEndpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                _logger.LogInformation("Discovering correct endpoint for Document Intelligence...");
                _logger.LogInformation("Base endpoint: {Endpoint}", baseEndpoint);

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var results = new List<object>();

                // Variantes del endpoint base
                var endpointVariants = new[]
                {
            baseEndpoint.TrimEnd('/'),
            baseEndpoint.TrimEnd('/').Replace("cognitiveservices", "documentintelligence"),
            baseEndpoint.TrimEnd('/').Replace("cognitiveservices", "formrecognizer"),
            // Probar diferentes regiones si es necesario
            baseEndpoint.TrimEnd('/').Replace("extraccion-polizas", "extraccion-polizas-di"),
            baseEndpoint.TrimEnd('/').Replace("extraccion-polizas", "polizas-intelligence")
        };

                // APIs a probar
                var apiPaths = new[]
                {
            "/documentintelligence/documentModels?api-version=2023-07-31",
            "/documentintelligence/documentModels?api-version=2024-02-29-preview",
            "/documentIntelligence/documentModels?api-version=2023-07-31",
            "/documentIntelligence/documentModels?api-version=2024-02-29-preview",
            "/formrecognizer/documentModels?api-version=2022-08-31",
            "/formrecognizer/documentModels?api-version=2023-07-31"
        };

                foreach (var endpoint in endpointVariants)
                {
                    foreach (var apiPath in apiPaths)
                    {
                        var fullUrl = $"{endpoint}{apiPath}";

                        try
                        {
                            _logger.LogInformation("Testing: {Url}", fullUrl);
                            var response = await httpClient.GetAsync(fullUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();

                                // Verificar si nuestro modelo está en la lista
                                bool modelFound = content.Contains(modelId);

                                var modelsInfo = new List<string>();
                                try
                                {
                                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);
                                    var models = jsonResponse?.value;

                                    if (models != null)
                                    {
                                        foreach (var model in models)
                                        {
                                            var foundModelId = model?.modelId?.ToString();
                                            if (!string.IsNullOrEmpty(foundModelId))
                                            {
                                                modelsInfo.Add(foundModelId);
                                            }
                                        }
                                    }
                                }
                                catch (Exception parseEx)
                                {
                                    _logger.LogWarning("Error parsing models: {Error}", parseEx.Message);
                                }

                                // Probar acceso directo si encontramos el modelo
                                object directAccess = null;
                                if (modelFound)
                                {
                                    var directAccessUrl = apiPath.Contains("documentintelligence") || apiPath.Contains("documentIntelligence") ?
                                        $"{endpoint}/documentintelligence/documentModels/{modelId}?api-version=2023-07-31" :
                                        $"{endpoint}/formrecognizer/documentModels/{modelId}?api-version=2022-08-31";

                                    try
                                    {
                                        var directResponse = await httpClient.GetAsync(directAccessUrl);
                                        directAccess = new
                                        {
                                            url = directAccessUrl,
                                            success = directResponse.IsSuccessStatusCode,
                                            status = directResponse.StatusCode.ToString()
                                        };
                                    }
                                    catch (Exception directEx)
                                    {
                                        directAccess = new
                                        {
                                            url = directAccessUrl,
                                            success = false,
                                            error = directEx.Message
                                        };
                                    }
                                }

                                results.Add(new
                                {
                                    endpoint = endpoint,
                                    apiPath = apiPath,
                                    fullUrl = fullUrl,
                                    success = true,
                                    statusCode = response.StatusCode.ToString(),
                                    modelFound = modelFound,
                                    totalModels = modelsInfo.Count,
                                    availableModels = modelsInfo,
                                    isCorrectEndpoint = modelFound,
                                    directAccess = directAccess
                                });

                                // Si encontramos una combinación que funciona con nuestro modelo, podemos parar aquí
                                if (modelFound)
                                {
                                    _logger.LogInformation("✅ Found working configuration with model!");
                                    break;
                                }
                            }
                            else
                            {
                                results.Add(new
                                {
                                    endpoint = endpoint,
                                    apiPath = apiPath,
                                    fullUrl = fullUrl,
                                    success = false,
                                    statusCode = response.StatusCode.ToString(),
                                    reason = response.ReasonPhrase,
                                    modelFound = false,
                                    totalModels = 0,
                                    availableModels = new List<string>(),
                                    isCorrectEndpoint = false,
                                    directAccess = (object)null
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            results.Add(new
                            {
                                endpoint = endpoint,
                                apiPath = apiPath,
                                fullUrl = fullUrl,
                                success = false,
                                error = ex.Message,
                                modelFound = false,
                                totalModels = 0,
                                availableModels = new List<string>(),
                                isCorrectEndpoint = false,
                                directAccess = (object)null
                            });
                        }
                    }

                    // Si ya encontramos algo que funciona, salir del bucle externo también
                    var foundWorking = results.Any(r =>
                    {
                        var result = (dynamic)r;
                        return result.success == true && result.modelFound == true;
                    });

                    if (foundWorking)
                    {
                        break;
                    }
                }

                // Encontrar configuraciones que funcionaron
                var workingConfigs = results.Where(r =>
                {
                    var result = (dynamic)r;
                    return result.success == true;
                }).ToList();

                var correctConfigs = results.Where(r =>
                {
                    var result = (dynamic)r;
                    return result.success == true && result.modelFound == true;
                }).ToList();

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    originalEndpoint = baseEndpoint,
                    modelIdSearched = modelId,
                    results = results,
                    summary = new
                    {
                        totalTested = results.Count,
                        workingEndpoints = workingConfigs.Count,
                        endpointsWithModel = correctConfigs.Count,
                        recommendation = GetEndpointRecommendation(workingConfigs.Count, correctConfigs.Count),
                        correctConfiguration = correctConfigs.FirstOrDefault()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error discovering correct endpoint");
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private string GetEndpointRecommendation(int workingEndpoints, int correctEndpoints)
        {
            if (workingEndpoints == 0)
            {
                return "❌ No se encontró ningún endpoint funcional. Verificar API key y URL base en Azure Portal.";
            }

            if (correctEndpoints == 0)
            {
                return $"⚠️ Se encontraron {workingEndpoints} endpoints funcionales, pero ninguno contiene el modelo. Verificar nombre del modelo.";
            }

            return $"✅ Se encontró configuración correcta. Usar el endpoint y API path de 'correctConfiguration'.";
        }

        [HttpGet("test-form-recognizer-legacy")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> TestFormRecognizerLegacy()
        {
            try
            {
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                _logger.LogInformation("Testing Form Recognizer v2.1 Legacy endpoints...");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var results = new List<object>();

                // APIs específicas de Form Recognizer v2.1 que funcionan con servicios antiguos
                var endpointsToTry = new[]
                {
            // Form Recognizer v2.1 - API para listar modelos personalizados
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models", "Form Recognizer v2.1 - Custom Models List"),
            
            // Form Recognizer v2.1 - API para modelo específico
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{modelId}", "Form Recognizer v2.1 - Specific Model"),
            
            // APIs alternativas que a veces funcionan
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.0/custom/models", "Form Recognizer v2.0 - Custom Models List"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.0/custom/models/{modelId}", "Form Recognizer v2.0 - Specific Model"),
            
            // Probar sin "custom" (por si es un modelo preentrenado)
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/models", "Form Recognizer v2.1 - All Models"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/models/{modelId}", "Form Recognizer v2.1 - Prebuilt Model"),
            
            // Probar endpoints sin versión específica
            ($"{endpoint.TrimEnd('/')}/formrecognizer/models", "Form Recognizer - Models (No Version)"),
            ($"{endpoint.TrimEnd('/')}/models", "Generic Models Endpoint"),
            
            // Probar variantes del modelo ID por si tiene prefijo/sufijo
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/custom-{modelId}", "Form Recognizer v2.1 - With custom prefix"),
            ($"{endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{modelId}-model", "Form Recognizer v2.1 - With model suffix")
        };

                foreach (var (url, description) in endpointsToTry)
                {
                    try
                    {
                        _logger.LogInformation("Testing: {Description} - {Url}", description, url);
                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            // Para v2.1, la estructura puede ser diferente
                            bool modelFound = content.Contains(modelId);
                            var modelsInfo = new List<object>();

                            try
                            {
                                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);

                                // Form Recognizer v2.1 puede tener diferentes estructuras
                                var models = jsonResponse?.value ??
                                           jsonResponse?.modelList ??
                                           jsonResponse?.models ??
                                           jsonResponse; // A veces el response ES la lista directamente

                                if (models != null)
                                {
                                    // Si es un array directo
                                    if (models is Newtonsoft.Json.Linq.JArray modelsArray)
                                    {
                                        foreach (var model in modelsArray)
                                        {
                                            var modelInfo = ExtractModelInfo(model);
                                            if (modelInfo != null)
                                                modelsInfo.Add(modelInfo);
                                        }
                                    }
                                    // Si es un objeto con propiedades
                                    else if (models is Newtonsoft.Json.Linq.JObject)
                                    {
                                        var modelInfo = ExtractModelInfo(models);
                                        if (modelInfo != null)
                                            modelsInfo.Add(modelInfo);
                                    }
                                    // Si es un objeto dinámico
                                    else
                                    {
                                        try
                                        {
                                            foreach (var model in models)
                                            {
                                                var modelInfo = ExtractModelInfo(model);
                                                if (modelInfo != null)
                                                    modelsInfo.Add(modelInfo);
                                            }
                                        }
                                        catch
                                        {
                                            // Si no es iterable, tratar como modelo único
                                            var modelInfo = ExtractModelInfo(models);
                                            if (modelInfo != null)
                                                modelsInfo.Add(modelInfo);
                                        }
                                    }
                                }
                            }
                            catch (Exception parseEx)
                            {
                                _logger.LogWarning("Error parsing response for {Description}: {Error}", description, parseEx.Message);
                                // Buscar el modelo ID en el contenido crudo
                                modelFound = content.Contains(modelId);
                            }

                            results.Add(new
                            {
                                description = description,
                                url = url,
                                success = true,
                                statusCode = response.StatusCode.ToString(),
                                modelFound = modelFound,
                                totalModels = modelsInfo.Count,
                                modelsInfo = modelsInfo,
                                rawContentPreview = content.Length > 500 ? content.Substring(0, 500) + "..." : content,
                                isWorkingEndpoint = true
                            });

                            // Si encontramos nuestro modelo, no necesitamos seguir buscando
                            if (modelFound)
                            {
                                _logger.LogInformation("✅ Found model in: {Description}", description);
                                break;
                            }
                        }
                        else
                        {
                            results.Add(new
                            {
                                description = description,
                                url = url,
                                success = false,
                                statusCode = response.StatusCode.ToString(),
                                reason = response.ReasonPhrase,
                                modelFound = false,
                                totalModels = 0,
                                modelsInfo = new List<object>(),
                                rawContentPreview = "",
                                isWorkingEndpoint = false
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            description = description,
                            url = url,
                            success = false,
                            error = ex.Message,
                            modelFound = false,
                            totalModels = 0,
                            modelsInfo = new List<object>(),
                            rawContentPreview = "",
                            isWorkingEndpoint = false
                        });
                    }
                }

                var workingEndpoints = results.Where(r => ((dynamic)r).isWorkingEndpoint == true).ToList();
                var endpointsWithModel = results.Where(r => ((dynamic)r).modelFound == true).ToList();

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    endpoint = endpoint,
                    modelIdSearched = modelId,
                    results = results,
                    summary = new
                    {
                        totalTested = results.Count,
                        workingEndpoints = workingEndpoints.Count,
                        endpointsWithModel = endpointsWithModel.Count,
                        workingConfiguration = workingEndpoints.FirstOrDefault(),
                        correctConfiguration = endpointsWithModel.FirstOrDefault(),
                        recommendation = GetLegacyRecommendation(workingEndpoints.Count, endpointsWithModel.Count)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Form Recognizer legacy endpoints");
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private object ExtractModelInfo(dynamic model)
        {
            try
            {
                var modelId = model?.modelId?.ToString() ??
                             model?.id?.ToString() ??
                             model?.name?.ToString() ??
                             model?.modelInfo?.modelId?.ToString();

                var status = model?.status?.ToString() ??
                            model?.modelInfo?.status?.ToString() ??
                            "Unknown";

                var description = model?.description?.ToString() ??
                                 model?.modelInfo?.description?.ToString() ??
                                 "No description";

                var createdDateTime = model?.createdDateTime?.ToString() ??
                                     model?.modelInfo?.createdDateTime?.ToString() ??
                                     model?.created?.ToString() ??
                                     "Unknown";

                if (!string.IsNullOrEmpty(modelId))
                {
                    return new
                    {
                        modelId = modelId,
                        status = status,
                        description = description,
                        createdDateTime = createdDateTime
                    };
                }
            }
            catch
            {
                // Ignore errors in model info extraction
            }

            return null;
        }

        private string GetLegacyRecommendation(int workingEndpoints, int endpointsWithModel)
        {
            if (workingEndpoints == 0)
            {
                return "❌ Ningún endpoint de Form Recognizer funciona. El servicio puede ser de Document Intelligence (más nuevo) o la API key puede estar incorrecta.";
            }

            if (endpointsWithModel == 0)
            {
                return $"⚠️ Se encontraron {workingEndpoints} endpoints funcionales, pero el modelo '{_configuration["AzureDocumentIntelligence:ModelId"]}' no existe. Verificar nombre del modelo en Azure Portal.";
            }

            return $"✅ ¡Perfecto! Encontrado endpoint funcional con el modelo. Usar la configuración de 'correctConfiguration'.";
        }

        [HttpGet("test-new-service")]
        [AllowAnonymous]
        public async Task<ActionResult> TestNewService()
        {
            var endpoint = "https://extraccion-polizas-v2.cognitiveservices.azure.com/";
            var apiKey = "C4Ly22Or87RLPjzaS0obQ7BhJh6kj2r3VQl5n0MiJQQJ99BGACZoyfRXJ3w3AAALACOGqMA9";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            var listUrl = $"{endpoint}documentintelligence/documentModels?api-version=2024-11-30";
            var response = await httpClient.GetAsync(listUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(new
                {
                    success = true,
                    modelsFound = content.Contains("poliza_vehiculos_bse"),
                    content = content
                });
            }

            return Ok(new { success = false, status = response.StatusCode });
        }

        [HttpGet("diagnose-auth")]
        [AllowAnonymous]
        public async Task<ActionResult> DiagnoseAuth()
        {
            try
            {
                var endpoint = "https://extraccion-polizas-v2.cognitiveservices.azure.com/";
                var apiKey = "C4Ly22Or87RLPjzaS0obQ7BhJh6kj2r3VQl5n0MiJQQJ99BGACZoyfRXJ3w3AAALACOGqMA9";

                var results = new List<object>();

                // Test 1: Header clásico
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                    var url = $"{endpoint}documentintelligence/documentModels?api-version=2024-11-30";

                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        results.Add(new
                        {
                            method = "Ocp-Apim-Subscription-Key",
                            success = response.IsSuccessStatusCode,
                            status = response.StatusCode.ToString(),
                            url = url
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            method = "Ocp-Apim-Subscription-Key",
                            success = false,
                            error = ex.Message,
                            url = url
                        });
                    }
                }

                // Test 2: Authorization Bearer
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                    var url = $"{endpoint}documentintelligence/documentModels?api-version=2024-11-30";

                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        results.Add(new
                        {
                            method = "Authorization Bearer",
                            success = response.IsSuccessStatusCode,
                            status = response.StatusCode.ToString(),
                            url = url
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            method = "Authorization Bearer",
                            success = false,
                            error = ex.Message,
                            url = url
                        });
                    }
                }

                // Test 3: API Key en query parameter
                using (var httpClient = new HttpClient())
                {
                    var url = $"{endpoint}documentintelligence/documentModels?api-version=2024-11-30&subscription-key={apiKey}";

                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        results.Add(new
                        {
                            method = "Query Parameter",
                            success = response.IsSuccessStatusCode,
                            status = response.StatusCode.ToString(),
                            url = url.Replace(apiKey, "[HIDDEN]") // Ocultar key en respuesta
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            method = "Query Parameter",
                            success = false,
                            error = ex.Message,
                            url = "URL with query param"
                        });
                    }
                }

                // Test 4: Versiones de API alternativas
                var apiVersions = new[] { "2023-07-31", "2022-08-31", "2024-02-29-preview" };

                foreach (var version in apiVersions)
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                        var url = $"{endpoint}documentintelligence/documentModels?api-version={version}";

                        try
                        {
                            var response = await httpClient.GetAsync(url);
                            results.Add(new
                            {
                                method = $"API Version {version}",
                                success = response.IsSuccessStatusCode,
                                status = response.StatusCode.ToString(),
                                url = url
                            });
                        }
                        catch (Exception ex)
                        {
                            results.Add(new
                            {
                                method = $"API Version {version}",
                                success = false,
                                error = ex.Message,
                                url = url
                            });
                        }
                    }
                }

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    endpoint = endpoint,
                    apiKeyLength = apiKey?.Length ?? 0,
                    results = results,
                    workingMethods = results.Where(r => ((dynamic)r).success == true).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test-exact-endpoints")]
        [AllowAnonymous]
        public async Task<ActionResult> TestExactEndpoints()
        {
            try
            {
                var endpoint = "https://extraccion-polizas-v2.cognitiveservices.azure.com/";
                var apiKey = "C4Ly22Or87RLPjzaS0obQ7BhJh6kj2r3VQl5n0MiJQQJ99BGACZoyfRXJ3w3AAALACOGqMA9";

                var results = new List<object>();

                // URLs EXACTAS basadas en el callRateLimit del servicio
                var exactEndpoints = new[]
                {
            // Form Recognizer endpoints del JSON
            ($"{endpoint}formrecognizer/documentModels?api-version=2023-07-31", "Form Recognizer documentModels"),
            ($"{endpoint}formrecognizer/custom/models?api-version=2023-07-31", "Form Recognizer custom models"),
            ($"{endpoint}formrecognizer/info?api-version=2023-07-31", "Form Recognizer info"),
            
            // Document Intelligence endpoints del JSON  
            ($"{endpoint}documentintelligence/documentModels?api-version=2023-07-31", "Document Intelligence documentModels"),
            ($"{endpoint}documentintelligence/info?api-version=2023-07-31", "Document Intelligence info"),
            
            // Versiones alternativas
            ($"{endpoint}formrecognizer/documentModels?api-version=2022-08-31", "Form Recognizer v3.0"),
            ($"{endpoint}documentintelligence/documentModels?api-version=2024-11-30", "Document Intelligence v4.0"),
        };

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Clear();

                foreach (var (url, description) in exactEndpoints)
                {
                    try
                    {
                        // Probar diferentes formas de autenticación
                        var authMethods = new[]
                        {
                    ("Ocp-Apim-Subscription-Key", apiKey),
                    ("Authorization", $"Bearer {apiKey}"),
                    ("x-api-key", apiKey)
                };

                        foreach (var (headerName, headerValue) in authMethods)
                        {
                            httpClient.DefaultRequestHeaders.Clear();
                            httpClient.DefaultRequestHeaders.Add(headerName, headerValue);

                            var response = await httpClient.GetAsync(url);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var modelFound = content.Contains("poliza_vehiculos_bse");

                                results.Add(new
                                {
                                    description = description,
                                    url = url,
                                    authMethod = headerName,
                                    success = true,
                                    statusCode = response.StatusCode.ToString(),
                                    modelFound = modelFound,
                                    contentLength = content.Length,
                                    contentPreview = content.Substring(0, Math.Min(300, content.Length))
                                });

                                // Si encontramos el modelo, devolver inmediatamente
                                if (modelFound)
                                {
                                    return Ok(new
                                    {
                                        success = true,
                                        workingConfiguration = new
                                        {
                                            endpoint = endpoint,
                                            url = url,
                                            authHeader = headerName,
                                            apiVersion = url.Contains("2024-11-30") ? "2024-11-30" :
                                                       url.Contains("2023-07-31") ? "2023-07-31" : "2022-08-31",
                                            modelFound = true
                                        },
                                        message = "✅ Configuración encontrada y modelo existe!",
                                        nextSteps = new[]
                                        {
                                    "1. Actualizar AzureDocumentIntelligenceService con esta configuración",
                                    "2. Usar esta combinación de URL y header de autenticación",
                                    "3. Probar procesamiento de documentos"
                                }
                                    });
                                }
                            }
                            else
                            {
                                results.Add(new
                                {
                                    description = description,
                                    url = url,
                                    authMethod = headerName,
                                    success = false,
                                    statusCode = response.StatusCode.ToString(),
                                    reason = response.ReasonPhrase,
                                    modelFound = false,
                                    contentLength = 0,
                                    contentPreview = ""
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            description = description,
                            url = url,
                            authMethod = "Exception",
                            success = false,
                            statusCode = "Error",
                            reason = ex.Message,
                            modelFound = false,
                            contentLength = 0,
                            contentPreview = ""
                        });
                    }
                }

                var workingConfigs = results.Where(r => ((dynamic)r).success == true).ToList();

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    serviceKind = "FormRecognizer", // Del JSON
                    provisioningState = "Succeeded", // Del JSON
                    publicNetworkAccess = "Enabled", // Del JSON
                    results = results,
                    workingConfigurations = workingConfigs,
                    summary = workingConfigs.Count > 0 ?
                        $"✅ Encontradas {workingConfigs.Count} configuraciones funcionales" :
                        "❌ Ninguna configuración funciona - puede ser problema de permisos en el servicio"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test-portal-keys")]
        [AllowAnonymous]
        public async Task<ActionResult> TestPortalKeys()
        {
            try
            {
                var endpoint = "https://extraccion-polizas-v2.cognitiveservices.azure.com/";

                // API Keys exactas del Portal Azure
                var keys = new[]
                {
            ("Clave 1", "C4Ly22Od8RTlPjzaS0obQ7BbhJh6kj2r3VQl5n0MiJQQJ99BGACZoyfiXJ3w3AAALACOGTUJI"),
            ("Clave 2", "8LRGJdUk8ztE6smxd3GevW52nvBY165Y6wrgmuBpCxzTANDmiJCQJ99BGACZoyfiXJ3w3AAALACOGdCkw"),
            ("Tu Config", "C4Ly22Or87iRLPjza90obQ7EhJh6kj2AUwHg9k0gmj2r3VQl5n0MJQQJ99BGACZoyfiXJ3w3AAALACOGqMA9")
        };

                var results = new List<object>();

                foreach (var (keyName, apiKey) in keys)
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                        // Test con API más nueva según tu configuración
                        var testUrl = $"{endpoint}documentintelligence/documentModels?api-version=2024-11-30";
                        var response = await httpClient.GetAsync(testUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var modelFound = content.Contains("poliza_vehiculos_bse") ||
                                           content.Contains("poliza-vehiculo-bse");

                            results.Add(new
                            {
                                keyName = keyName,
                                success = true,
                                status = response.StatusCode.ToString(),
                                modelFound = modelFound,
                                contentPreview = content.Substring(0, Math.Min(300, content.Length)),
                                recommendation = modelFound ? "✅ USAR ESTA CONFIGURACIÓN" : "⚠️ Verificar nombre del modelo"
                            });

                            _logger.LogInformation("✅ {KeyName} - SUCCESS", keyName);

                            if (modelFound)
                            {
                                _logger.LogInformation("🎯 MODELO ENCONTRADO con {KeyName}!", keyName);
                            }
                        }
                        else
                        {
                            results.Add(new
                            {
                                keyName = keyName,
                                success = false,
                                status = response.StatusCode.ToString(),
                                reason = response.ReasonPhrase,
                                modelFound = false,
                                contentPreview = "",
                                recommendation = response.StatusCode == System.Net.HttpStatusCode.Unauthorized ?
                                    "❌ API Key inválida" : $"❌ Error: {response.StatusCode}"
                            });

                            _logger.LogWarning("❌ {KeyName} - {Status}", keyName, response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            keyName = keyName,
                            success = false,
                            error = ex.Message,
                            modelFound = false,
                            contentPreview = "",
                            recommendation = "❌ Error de conexión"
                        });

                        _logger.LogError(ex, "❌ {KeyName} - Exception", keyName);
                    }
                }

                // Encontrar configuración exitosa
                var workingConfig = results.FirstOrDefault(r => ((dynamic)r).success == true);
                var configWithModel = results.FirstOrDefault(r => ((dynamic)r).success == true && ((dynamic)r).modelFound == true);

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    endpoint = endpoint,
                    apiVersion = "2024-11-30",
                    results = results,
                    summary = new
                    {
                        workingConfigs = results.Count(r => ((dynamic)r).success == true),
                        configsWithModel = results.Count(r => ((dynamic)r).success == true && ((dynamic)r).modelFound == true),
                        bestConfiguration = configWithModel ?? workingConfig,
                        nextSteps = configWithModel != null ? new[]
                        {
                    "1. Usar la API Key que muestra 'USAR ESTA CONFIGURACIÓN'",
                    "2. Actualizar appsettings.json",
                    "3. Restart de la aplicación",
                    "4. Test final con /api/AzureDocument/test-connection"
                } : workingConfig != null ? new[]
                        {
                    "1. Autenticación funciona pero verificar nombre del modelo",
                    "2. Ir a Azure Portal → Model management",
                    "3. Copiar nombre exacto del modelo"
                } : new[]
                        {
                    "1. Todas las API Keys fallan",
                    "2. Verificar permisos en Azure Portal",
                    "3. Regenerar API Keys si es necesario"
                }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 TEST PORTAL KEYS FAILED");

                return StatusCode(500, new
                {
                    error = ex.Message,
                    recommendation = "Verificar conectividad y configuración básica"
                });
            }
        }

        /// <summary>
        /// TEST COMPLETO - Verificar que todo el flujo funciona
        /// </summary>
        [HttpGet("test-complete-service")]
        [AllowAnonymous]
        public async Task<ActionResult> TestCompleteService()
        {
            try
            {
                _logger.LogInformation("🧪 STARTING COMPLETE SERVICE TEST");

                var results = new List<object>();

                // Test 1: Verificar configuración actual
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                results.Add(new
                {
                    test = "Configuration Check",
                    endpoint = endpoint,
                    hasApiKey = !string.IsNullOrEmpty(apiKey),
                    apiKeyLength = apiKey?.Length ?? 0,
                    modelId = modelId,
                    status = "✅ Configured"
                });

                // Test 2: Test del servicio Azure Document Intelligence
                var serviceTest = await _azureDocumentService.TestConnectionAsync();

                results.Add(new
                {
                    test = "Azure Document Intelligence Service",
                    success = serviceTest,
                    status = serviceTest ? "✅ Connected" : "❌ Failed"
                });

                // Test 3: Test de información del modelo
                string modelInfo = null;
                try
                {
                    modelInfo = await _azureDocumentService.GetModelInfoAsync();
                    results.Add(new
                    {
                        test = "Model Information",
                        success = true,
                        modelInfo = modelInfo.Substring(0, Math.Min(300, modelInfo.Length)) + "...",
                        status = "✅ Model Accessible"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        test = "Model Information",
                        success = false,
                        error = ex.Message,
                        status = "❌ Model Access Failed"
                    });
                }

                // Test 4: Test con documento mínimo usando DocumentIntelligenceClient
                object documentTest = null;
                try
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                    // Crear documento de prueba mínimo
                    var testContent = "Test document for processing validation";
                    var testBytes = System.Text.Encoding.UTF8.GetBytes(testContent);

                    using var content = new ByteArrayContent(testBytes);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

                    // Usar el modelo específico para test
                    var analyzeUrl = $"{endpoint}documentintelligence/documentModels/{modelId}:analyze?api-version=2024-11-30";

                    var response = await httpClient.PostAsync(analyzeUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        documentTest = new
                        {
                            success = true,
                            status = response.StatusCode.ToString(),
                            message = "✅ Document processing endpoint accessible",
                            operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault()
                        };
                    }
                    else
                    {
                        documentTest = new
                        {
                            success = false,
                            status = response.StatusCode.ToString(),
                            reason = response.ReasonPhrase,
                            message = $"❌ Document processing failed: {response.StatusCode}"
                        };
                    }
                }
                catch (Exception ex)
                {
                    documentTest = new
                    {
                        success = false,
                        error = ex.Message,
                        message = "❌ Document processing test failed"
                    };
                }

                results.Add(new
                {
                    test = "Document Processing Test",
                    result = documentTest
                });

                // Test 5: Verificar endpoints disponibles
                var endpointTests = new List<object>();
                var endpointsToTest = new[]
                {
            ($"{endpoint}documentintelligence/info?api-version=2024-11-30", "Service Info"),
            ($"{endpoint}documentintelligence/documentModels/{modelId}?api-version=2024-11-30", "Specific Model"),
            ($"{endpoint}formrecognizer/documentModels?api-version=2023-07-31", "Legacy Form Recognizer")
        };

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                    foreach (var (url, description) in endpointsToTest)
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(url);
                            endpointTests.Add(new
                            {
                                endpoint = description,
                                url = url,
                                success = response.IsSuccessStatusCode,
                                status = response.StatusCode.ToString()
                            });
                        }
                        catch (Exception ex)
                        {
                            endpointTests.Add(new
                            {
                                endpoint = description,
                                url = url,
                                success = false,
                                error = ex.Message
                            });
                        }
                    }
                }

                results.Add(new
                {
                    test = "Endpoint Availability",
                    endpoints = endpointTests
                });

                // Análisis final
                var allTestsPassed = serviceTest &&
                                   !string.IsNullOrEmpty(modelInfo) &&
                                   ((dynamic)documentTest)?.success == true;

                var readinessStatus = allTestsPassed ? "🚀 READY FOR PRODUCTION" :
                                    serviceTest ? "⚠️ PARTIALLY READY - Some tests failed" :
                                    "❌ NOT READY - Service connection failed";

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    overallStatus = readinessStatus,
                    configuration = new
                    {
                        endpoint = endpoint,
                        modelId = modelId,
                        apiVersion = "2024-11-30",
                        serviceType = "Document Intelligence"
                    },
                    testResults = results,
                    recommendations = allTestsPassed ? new[]
                    {
                "✅ Todo está funcionando correctamente",
                "✅ Puedes proceder a procesar documentos reales",
                "✅ El servicio está listo para producción"
            } : new[]
                    {
                "1. Revisar los tests que fallaron",
                "2. Verificar permisos del modelo específico",
                "3. Considerar usar modelo preentrenado para tests iniciales"
            },
                    nextActions = new[]
                    {
                "1. Si todo está OK: Procesar un PDF real",
                "2. Test endpoint: /api/AzureDocument/process (POST con archivo)",
                "3. Verificar mapeo de campos extraídos"
            }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 COMPLETE SERVICE TEST FAILED");

                return StatusCode(500, new
                {
                    overallStatus = "❌ CRITICAL ERROR",
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    recommendation = "Verificar configuración básica y reiniciar servicio"
                });
            }
        }

        /// <summary>
        /// VERIFICACIÓN RÁPIDA - Confirmar que el cambio de endpoint funciona
        /// </summary>
        [HttpGet("verify-endpoint-fix")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEndpointFix()
        {
            try
            {
                // Endpoint correcto (con -v2)
                var correctEndpoint = "https://extraccion-polizas-v2.cognitiveservices.azure.com/";

                // Tu API Key que sabemos que funciona
                var workingApiKey = "C4Ly22Or87iRLPjza90obQ7EhJh6kj2AUwHg9k0gmj2r3VQl5n0MJQQJ99BGACZoyfiXJ3w3AAALACOGqMA9";

                // Endpoint actual de configuración
                var configEndpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var configApiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                var results = new List<object>();

                // Test 1: Con endpoint correcto y API Key que funciona
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", workingApiKey);

                    var testUrl = $"{correctEndpoint}documentintelligence/documentModels?api-version=2024-11-30";
                    var response = await httpClient.GetAsync(testUrl);

                    results.Add(new
                    {
                        test = "Endpoint Correcto + API Key Funcional",
                        endpoint = correctEndpoint,
                        success = response.IsSuccessStatusCode,
                        status = response.StatusCode.ToString(),
                        message = response.IsSuccessStatusCode ? "✅ CONFIGURACIÓN CORRECTA" : "❌ Aún hay problemas"
                    });
                }

                // Test 2: Con configuración actual
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configApiKey);

                    var testUrl = $"{configEndpoint}documentintelligence/documentModels?api-version=2024-11-30";
                    var response = await httpClient.GetAsync(testUrl);

                    results.Add(new
                    {
                        test = "Configuración Actual en appsettings.json",
                        endpoint = configEndpoint,
                        success = response.IsSuccessStatusCode,
                        status = response.StatusCode.ToString(),
                        message = response.IsSuccessStatusCode ? "✅ Ya está corregido" : "❌ Necesita actualización"
                    });
                }

                // Test 3: Comparación de endpoints
                var endpointComparison = new
                {
                    configurationEndpoint = configEndpoint,
                    correctEndpoint = correctEndpoint,
                    needsUpdate = configEndpoint != correctEndpoint,
                    difference = configEndpoint != correctEndpoint ? "Falta '-v2' en el endpoint" : "Endpoint correcto"
                };

                // Test 4: DocumentIntelligenceClient con configuración corregida
                object clientTest = null;
                try
                {
                    var client = new DocumentIntelligenceClient(
                        new Uri(correctEndpoint),
                        new AzureKeyCredential(workingApiKey)
                    );

                    // Test básico de conectividad
                    var testContent = BinaryData.FromString("Test document");
                    var operation = await client.AnalyzeDocumentAsync(
                        WaitUntil.Started,
                        "prebuilt-document", // Modelo preentrenado para test
                        testContent
                    );

                    clientTest = new
                    {
                        success = true,
                        operationId = operation.Id,
                        message = "✅ DocumentIntelligenceClient funciona correctamente"
                    };
                }
                catch (Exception ex)
                {
                    clientTest = new
                    {
                        success = false,
                        error = ex.Message,
                        message = "❌ Error en DocumentIntelligenceClient"
                    };
                }

                var allWorking = results.All(r => ((dynamic)r).success == true) &&
                                ((dynamic)clientTest)?.success == true;

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    status = allWorking ? "✅ TODO FUNCIONANDO" : "⚠️ NECESITA CORRECCIÓN",
                    results = results,
                    endpointComparison = endpointComparison,
                    clientTest = clientTest,
                    recommendation = endpointComparison.needsUpdate ?
                        "🔧 Actualizar appsettings.json con endpoint correcto (agregar -v2)" :
                        "✅ Configuración está correcta",
                    exactConfiguration = new
                    {
                        endpoint = correctEndpoint,
                        apiKey = workingApiKey,
                        modelId = modelId,
                        apiVersion = "2024-11-30"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    recommendation = "Verificar configuración básica"
                });
            }
        }

        /// <summary>
        /// IMPROVED PARSER - Limpiar y extraer datos correctamente
        /// </summary>
        [HttpPost("improved-parser")]
        [AllowAnonymous]
        public async Task<ActionResult> ImprovedParser([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🧹 IMPROVED PARSER - Limpiando datos extraídos");

                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                var client = new DocumentIntelligenceClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(apiKey));

                using var stream = file.OpenReadStream();
                var binaryData = BinaryData.FromStream(stream);

                var operation = await client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    modelId,
                    binaryData);

                var analyzeResult = operation.Value;

                // Extraer y limpiar campos
                var cleanedData = new Dictionary<string, string>();

                if (analyzeResult.Documents?.Count > 0)
                {
                    var document = analyzeResult.Documents[0];

                    foreach (var field in document.Fields)
                    {
                        var fieldName = field.Key;
                        var fieldValue = field.Value;

                        // Obtener valor limpio
                        string rawValue = fieldValue.ValueString ?? fieldValue.Content ?? "";
                        string cleanValue = CleanExtractedValue(rawValue, fieldName);

                        if (!string.IsNullOrWhiteSpace(cleanValue))
                        {
                            cleanedData[fieldName] = cleanValue;
                        }
                    }
                }

                // Crear PolizaDto con datos limpios
                var polizaDto = new PolizaDto
                {
                    // Datos básicos del sistema
                    Comcod = 1,
                    Seccod = 4,
                    Moncod = 1,
                    Convig = "1",
                    Consta = "1",
                    Contra = "2",
                    Ramo = "AUTOMOVILES",
                    Last_update = DateTime.Now,
                    Ingresado = DateTime.Now,

                    // Datos extraídos y limpiados
                    Conpol = ExtractPolizaNumber(cleanedData),
                    Clinom = GetCleanValue(cleanedData, "asegurado.nombre"),

                    // Vehículo
                    Conmaraut = BuildVehicleDescription(cleanedData),
                    Conmataut = GetCleanValue(cleanedData, "vehiculo.matricula"),
                    Conpadaut = GetCleanValue(cleanedData, "vehiculo.padron"),
                    Conmotor = GetCleanValue(cleanedData, "vehiculo.motor"),
                    Conchasis = GetCleanValue(cleanedData, "vehiculo.chasis"),
                    //Conanioaut = GetCleanValue(cleanedData, "vehiculo.anio"),

                    // Fechas
                    Confchdes = ParseCleanDate(GetCleanValue(cleanedData, "poliza.vigencia.desde")),
                    Confchhas = ParseCleanDate(GetCleanValue(cleanedData, "poliza.vigencia.hasta")),

                    // Montos
                    Conpremio = ParseCleanAmount(GetCleanValue(cleanedData, "financiero.prima_comercial")),
                    Contot = ParseCleanAmount(GetCleanValue(cleanedData, "financiero.premio_total")),

                    Observaciones = $"Procesado con parser mejorado - {DateTime.Now:yyyy-MM-dd HH:mm}"
                };

                // Validación de campos críticos
                var validation = ValidateExtractedData(polizaDto, cleanedData);

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    archivo = file.FileName,
                    estado = "PROCESADO_CON_PARSER_MEJORADO",

                    // Datos antes y después de limpieza
                    datosOriginales = analyzeResult.Documents?[0]?.Fields?.Take(5).ToDictionary(
                        f => f.Key,
                        f => f.Value.Content ?? f.Value.ValueString ?? ""
                    ),

                    datosLimpios = cleanedData.Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                    // Datos críticos extraídos
                    datosExtraidos = new
                    {
                        numeroPoliza = polizaDto.Conpol,
                        asegurado = polizaDto.Clinom,
                        vehiculo = polizaDto.Conmaraut,
                        matricula = polizaDto.Conmataut,
                        motor = polizaDto.Conmotor,
                        chasis = polizaDto.Conchasis,
                        primaComercial = polizaDto.Conpremio,
                        premioTotal = polizaDto.Contot,
                        vigenciaDesde = polizaDto.Confchdes,
                        vigenciaHasta = polizaDto.Confchhas
                    },

                    // Póliza final
                    polizaProcesada = polizaDto,

                    // Validación
                    validacion = validation,

                    estadisticas = new
                    {
                        camposTotalesExtraidos = cleanedData.Count,
                        camposLimpiosNoVacios = cleanedData.Values.Count(v => !string.IsNullOrWhiteSpace(v)),
                        //camposCriticosCompletos = validation.camposCriticosCompletos,
                        //camposCriticosFaltantes = validation.camposCriticosFaltantes,
                        //porcentajeCompletitud = validation.porcentajeCompletitud
                    },

                    //recomendacion = validation.porcentajeCompletitud >= 80 ?
                    //    "✅ EXCELENTE - Datos suficientes para crear póliza" :
                    //    validation.porcentajeCompletitud >= 60 ?
                    //    "⚠️ BUENO - Revisar campos faltantes" :
                    //    "❌ INSUFICIENTE - Muchos datos críticos faltantes"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in improved parser");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private string CleanExtractedValue(string rawValue, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(rawValue)) return "";

            // Limpiar saltos de línea y espacios extra
            string cleaned = rawValue.Replace("\n", " ").Replace("\r", "").Trim();

            // Remover etiquetas comunes
            var labelsToRemove = new[]
            {
        "MARCA", "AÑO", "MODELO", "MOTOR", "CHASIS", "Asegurado:", "Nombre:",
        "Prima Comercial:", "Premio Total a Pagar:", "Vigencia:", "Nº de Póliza"
    };

            foreach (var label in labelsToRemove)
            {
                if (cleaned.StartsWith(label, StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Substring(label.Length).Trim(' ', ':', '-');
                }
            }

            // Limpiar caracteres especiales al inicio/final
            cleaned = cleaned.Trim(' ', ':', '-', '.', ',');

            return cleaned;
        }

        private string GetCleanValue(Dictionary<string, string> data, string key)
        {
            return data.ContainsKey(key) ? data[key] : "";
        }

        private string ExtractPolizaNumber(Dictionary<string, string> data)
        {
            // Buscar número de póliza en diferentes campos
            var polizaContent = GetCleanValue(data, "datos_poliza");

            // Buscar patrón de número de póliza (ej: 9128263)
            var match = System.Text.RegularExpressions.Regex.Match(
                polizaContent,
                @"(?:Póliza.*?(\d{7,})|(\d{7,})\s*/\s*\d+)"
            );

            if (match.Success)
            {
                return match.Groups[1].Value.Length > 0 ? match.Groups[1].Value : match.Groups[2].Value;
            }

            return "";
        }

        private string BuildVehicleDescription(Dictionary<string, string> data)
        {
            var marca = GetCleanValue(data, "vehiculo.marca");
            var modelo = GetCleanValue(data, "vehiculo.modelo");
            var anio = GetCleanValue(data, "vehiculo.anio");

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(marca)) parts.Add(marca);
            if (!string.IsNullOrEmpty(modelo)) parts.Add(modelo);
            if (!string.IsNullOrEmpty(anio)) parts.Add($"({anio})");

            return string.Join(" ", parts);
        }

        private DateTime ParseCleanDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return DateTime.Now;

            // Buscar patrón de fecha (dd/mm/yyyy)
            var match = System.Text.RegularExpressions.Regex.Match(
                dateString,
                @"(\d{1,2})/(\d{1,2})/(\d{4})"
            );

            if (match.Success)
            {
                if (DateTime.TryParse($"{match.Groups[1]}/{match.Groups[2]}/{match.Groups[3]}", out var date))
                {
                    return date;
                }
            }

            return DateTime.Now;
        }

        private decimal? ParseCleanAmount(string amountString)
        {
            if (string.IsNullOrWhiteSpace(amountString)) return null;

            // Buscar patrón de monto ($ 123.584,47)
            var match = System.Text.RegularExpressions.Regex.Match(
                amountString,
                @"\$\s*([\d.,]+)"
            );

            if (match.Success)
            {
                var numberString = match.Groups[1].Value
                    .Replace(".", "") // Remover separadores de miles
                    .Replace(",", "."); // Convertir coma decimal a punto

                if (decimal.TryParse(numberString, out var amount))
                {
                    return amount;
                }
            }

            return null;
        }

        private object ValidateExtractedData(PolizaDto poliza, Dictionary<string, string> data)
        {
            var camposCriticos = new[]
            {
        ("Número de Póliza", !string.IsNullOrEmpty(poliza.Conpol)),
        ("Asegurado", !string.IsNullOrEmpty(poliza.Clinom)),
        ("Vehículo", !string.IsNullOrEmpty(poliza.Conmaraut)),
        ("Motor", !string.IsNullOrEmpty(poliza.Conmotor)),
        ("Chasis", !string.IsNullOrEmpty(poliza.Conchasis)),
        ("Prima Comercial", poliza.Conpremio.HasValue),
        ("Vigencia Desde", poliza.Confchdes != DateTime.Now.Date),
        ("Vigencia Hasta", poliza.Confchhas != DateTime.Now.Date)
    };

            var completos = camposCriticos.Count(c => c.Item2);
            var total = camposCriticos.Length;
            var porcentaje = Math.Round((double)completos / total * 100, 1);

            return new
            {
                camposCriticosCompletos = completos,
                camposCriticosTotales = total,
                porcentajeCompletitud = porcentaje,
                camposCriticosFaltantes = camposCriticos.Where(c => !c.Item2).Select(c => c.Item1).ToList(),
                esViableParaCreacion = porcentaje >= 70
            };
        }

    }
}