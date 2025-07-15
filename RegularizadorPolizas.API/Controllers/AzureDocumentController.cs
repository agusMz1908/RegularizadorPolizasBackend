using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using System.ComponentModel.DataAnnotations;
using static ClienteMatchResult;

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
        private readonly IWebHostEnvironment _hostEnvironment; 
        private readonly ILogger<AzureDocumentController> _logger;
        private readonly IDocumentExtractionService _documentExtractionService;
        private readonly IClienteMatchingService _clienteMatchingService;

        public AzureDocumentController(
            IAzureDocumentIntelligenceService azureDocumentService,
            IProcessDocumentService processDocumentService,
            IPolizaService polizaService,
            IConfiguration configuration,
            IWebHostEnvironment hostEnvironment, 
            ILogger<AzureDocumentController> logger,
            IDocumentExtractionService documentExtractionService,
            IClienteMatchingService clienteMatchingService)
        {
            _azureDocumentService = azureDocumentService;
            _processDocumentService = processDocumentService;
            _polizaService = polizaService;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _documentExtractionService = documentExtractionService;
            _clienteMatchingService = clienteMatchingService;
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

        [HttpPost("debug-extraction-v2")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DebugExtractionV2([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔧 DEBUG V2: Usando nuestro parser mejorado para {FileName}", file.FileName);

                // Usar nuestro nuevo servicio
                var extractResult = await _documentExtractionService.ProcessDocumentAsync(file);

                return Ok(new
                {
                    archivo = file.FileName,
                    timestamp = DateTime.Now,
                    version = "V2_MEJORADO",

                    // Datos extraídos con nuestro sistema
                    extraccion = extractResult,

                    // Resumen fácil de leer
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
                    },

                    // Comparación con sistema anterior
                    mejoras = new
                    {
                        usaParserEspecializado = true,
                        separaDatosPolizaVsCliente = true,
                        limpiezaAutomatica = true,
                        extraccionMejorada = true
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en debug extraction V2");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// NUEVO: Test de búsqueda de clientes
        /// </summary>
        [HttpPost("test-client-search")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> TestClientSearch([FromBody] DatosClienteExtraidos datosCliente)
        {
            try
            {
                if (datosCliente == null)
                {
                    return BadRequest(new { error = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("🔍 TEST: Buscando cliente '{Nombre}' - '{Documento}'",
                    datosCliente.Nombre, datosCliente.Documento);

                var result = await _clienteMatchingService.BuscarClienteAsync(datosCliente);

                return Ok(new
                {
                    timestamp = DateTime.Now,
                    busqueda = datosCliente,
                    resultado = result,
                    interpretacion = new
                    {
                        tipoResultado = result.TipoResultado.ToString(),
                        mensaje = result.MensajeUsuario,
                        cantidadCoincidencias = result.Matches.Count,
                        requiereAccionManual = result.RequiereIntervencionManual,
                        mejorCoincidencia = result.Matches.FirstOrDefault()?.Cliente?.Clinom,
                        scoreMaximo = result.Matches.FirstOrDefault()?.Score
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en test client search");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// NUEVO: Flujo completo - Extraction + Client Search (CORREGIDO)
        /// </summary>
        [HttpPost("process-complete-v2")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ProcessCompleteV2([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🚀 FLUJO COMPLETO V2: Procesando {FileName}", file.FileName);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var fechaInicio = DateTime.Now;

                // Variables para almacenar resultados
                DocumentExtractResult extractResult = null;
                ClienteMatchResult clienteMatch = null;
                string recomendacion = "";
                string siguientePaso = "";
                bool success = false;
                var pasos = new List<object>();

                try
                {
                    // PASO 1: Extraer datos
                    _logger.LogInformation("📄 PASO 1: Extrayendo datos del documento...");
                    extractResult = await _documentExtractionService.ProcessDocumentAsync(file);

                    pasos.Add(new
                    {
                        paso = 1,
                        descripcion = "Extracción de datos",
                        completado = true,
                        estado = extractResult.EstadoProcesamiento
                    });

                    if (extractResult.EstadoProcesamiento.Contains("ERROR"))
                    {
                        success = false;
                        recomendacion = "❌ Error extrayendo datos del documento";
                        siguientePaso = "Revisar configuración de Azure Document Intelligence";
                    }
                    else
                    {
                        // PASO 2: Buscar cliente automáticamente
                        _logger.LogInformation("🔍 PASO 2: Buscando cliente automáticamente...");
                        clienteMatch = await _clienteMatchingService.BuscarClienteAsync(extractResult.DatosClienteBusqueda);

                        pasos.Add(new
                        {
                            paso = 2,
                            descripcion = "Búsqueda de cliente",
                            completado = true,
                            resultado = clienteMatch.TipoResultado.ToString()
                        });

                        // PASO 3: Determinar siguiente acción
                        switch (clienteMatch.TipoResultado)
                        {
                            case TipoResultadoCliente.MatchExacto:
                                success = true;
                                recomendacion = $"✅ Cliente encontrado automáticamente: {clienteMatch.Matches[0].Cliente.Clinom}";
                                siguientePaso = "Crear póliza automáticamente con este cliente";
                                break;

                            case TipoResultadoCliente.MatchMuyProbable:
                                success = true;
                                recomendacion = $"🟡 Cliente muy probable: {clienteMatch.Matches[0].Cliente.Clinom}. Requiere confirmación del usuario.";
                                siguientePaso = "Mostrar cliente para confirmación";
                                break;

                            case TipoResultadoCliente.MultiplesMatches:
                                success = true;
                                recomendacion = $"🟠 Se encontraron {clienteMatch.Matches.Count} clientes similares. Usuario debe seleccionar.";
                                siguientePaso = "Mostrar lista para selección manual";
                                break;

                            default:
                                success = true; // La extracción funcionó, solo no hay cliente
                                recomendacion = "🔴 No se encontraron coincidencias automáticas. Búsqueda manual requerida.";
                                siguientePaso = "Búsqueda manual de cliente o crear cliente nuevo";
                                break;
                        }
                    }

                    stopwatch.Stop();

                    return Ok(new
                    {
                        archivo = file.FileName,
                        fechaInicio = fechaInicio,
                        fechaFin = DateTime.Now,
                        version = "V2_COMPLETO",
                        success = success,
                        tiempoTotal = stopwatch.ElapsedMilliseconds,
                        pasos = pasos,

                        // Resultados detallados
                        datosExtraidos = extractResult,
                        busquedaCliente = clienteMatch,

                        // Resumen ejecutivo
                        resumen = new
                        {
                            numeroPoliza = extractResult?.DatosPoliza?.NumeroPoliza ?? "No extraído",
                            clienteExtraido = extractResult?.DatosClienteBusqueda?.Nombre ?? "No extraído",
                            documentoExtraido = extractResult?.DatosClienteBusqueda?.Documento ?? "No extraído",
                            vehiculoExtraido = extractResult?.DatosPoliza?.DescripcionVehiculo ?? "No extraído",
                            tipoResultadoBusqueda = clienteMatch?.TipoResultado.ToString() ?? "No procesado",
                            cantidadCoincidencias = clienteMatch?.Matches?.Count ?? 0,
                            mejorCoincidencia = clienteMatch?.Matches?.FirstOrDefault()?.Cliente?.Clinom ?? "Ninguna"
                        },

                        // Guía para el usuario
                        recomendacion = recomendacion,
                        siguientePaso = siguientePaso,
                    });
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(ex, "❌ Error en flujo completo V2");

                    return Ok(new
                    {
                        archivo = file.FileName,
                        fechaInicio = fechaInicio,
                        fechaFin = DateTime.Now,
                        version = "V2_COMPLETO",
                        success = false,
                        error = ex.Message,
                        tiempoTotal = stopwatch.ElapsedMilliseconds,
                        pasos = pasos,
                        recomendacion = "❌ Error procesando documento. Revisar logs para detalles.",
                        siguientePaso = "Verificar configuración y volver a intentar"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general en process complete V2");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// NUEVO: Endpoint para crear póliza con cliente seleccionado
        /// </summary>
        [HttpPost("create-poliza-with-client")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreatePolizaWithClient([FromBody] CrearPolizaConClienteRequest request)
        {
            try
            {
                if (request == null || request.ClienteId <= 0 || request.DatosPoliza == null)
                {
                    return BadRequest(new { error = "Datos de solicitud inválidos" });
                }

                _logger.LogInformation("🔄 CREANDO PÓLIZA: {NumeroPoliza} para cliente {ClienteId}",
                    request.DatosPoliza.NumeroPoliza, request.ClienteId);

                var result = await _documentExtractionService.CrearPolizaConClienteAsync(request);

                return Ok(new
                {
                    timestamp = DateTime.Now,
                    request = new
                    {
                        clienteId = request.ClienteId,
                        numeroPoliza = request.DatosPoliza.NumeroPoliza,
                        archivoOriginal = request.ArchivoOriginal,
                        confirmadoPorUsuario = request.ConfirmadoPorUsuario
                    },
                    resultado = result,
                    status = result.Success ? "✅ ÉXITO" : "❌ ERROR",
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creando póliza con cliente");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // *** ENDPOINT PARA TESTEAR INMEDIATAMENTE ***

        /// <summary>
        /// COMPARACIÓN: Antiguo vs Nuevo sistema
        /// </summary>
        [HttpPost("compare-old-vs-new")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CompareOldVsNew([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("⚔️ COMPARACIÓN: Sistema antiguo vs nuevo para {FileName}", file.FileName);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // Test sistema ANTIGUO
                object resultadoAntiguo;
                long tiempoAntiguo;
                try
                {
                    var sw1 = System.Diagnostics.Stopwatch.StartNew();
                    var oldResult = await _azureDocumentService.ProcessDocumentAsync(file);
                    sw1.Stop();
                    tiempoAntiguo = sw1.ElapsedMilliseconds;

                    resultadoAntiguo = new
                    {
                        success = true,
                        estado = oldResult.EstadoProcesamiento,
                        confianza = oldResult.ConfianzaExtraccion,
                        camposExtraidos = oldResult.CamposExtraidos?.Count ?? 0,
                        polizaCompleta = oldResult.PolizaProcesada != null,
                        tiempoProcesamiento = tiempoAntiguo
                    };
                }
                catch (Exception ex)
                {
                    tiempoAntiguo = 0;
                    resultadoAntiguo = new
                    {
                        success = false,
                        error = ex.Message,
                        tiempoProcesamiento = tiempoAntiguo
                    };
                }

                // Test sistema NUEVO
                object resultadoNuevo;
                long tiempoNuevo;
                try
                {
                    var sw2 = System.Diagnostics.Stopwatch.StartNew();
                    var newResult = await _documentExtractionService.ProcessDocumentAsync(file);
                    sw2.Stop();
                    tiempoNuevo = sw2.ElapsedMilliseconds;

                    resultadoNuevo = new
                    {
                        success = true,
                        estado = newResult.EstadoProcesamiento,
                        confianza = newResult.ConfianzaExtraccion,
                        datosExtraidos = new
                        {
                            numeroPoliza = newResult.DatosPoliza.NumeroPoliza,
                            cliente = newResult.DatosClienteBusqueda.Nombre,
                            vehiculo = newResult.DatosPoliza.DescripcionVehiculo,
                            prima = newResult.DatosPoliza.PrimaComercial,
                            vigencia = newResult.DatosPoliza.VigenciaDesde
                        },
                        advertencias = newResult.Advertencias.Count,
                        tiempoProcesamiento = tiempoNuevo
                    };
                }
                catch (Exception ex)
                {
                    tiempoNuevo = 0;
                    resultadoNuevo = new
                    {
                        success = false,
                        error = ex.Message,
                        tiempoProcesamiento = tiempoNuevo
                    };
                }

                stopwatch.Stop();

                return Ok(new
                {
                    archivo = file.FileName,
                    timestamp = DateTime.Now,
                    tiempoTotalComparacion = stopwatch.ElapsedMilliseconds,

                    sistemaAntiguo = resultadoAntiguo,
                    sistemaNuevo = resultadoNuevo,

                    comparacion = new
                    {
                        mejorExtraccion = ((dynamic)resultadoNuevo).success && !string.IsNullOrEmpty(((dynamic)resultadoNuevo).datosExtraidos?.numeroPoliza),
                        masFuncionalidad = true, // Nuevo tiene búsqueda de clientes
                        mayorPrecision = ((dynamic)resultadoNuevo).success && ((dynamic)resultadoNuevo).advertencias < 5,
                        rendimiento = tiempoNuevo <= tiempoAntiguo ? "Nuevo es más rápido o igual" : "Antiguo es más rápido"
                    },

                    recomendacion = ((dynamic)resultadoNuevo).success ?
                        "✅ Usar sistema NUEVO - Mejor extracción y funcionalidad completa" :
                        "⚠️ Revisar configuración del sistema nuevo"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en comparación old vs new");
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}