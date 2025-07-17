using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AzureDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureDocumentController> _logger;
        private readonly SmartDocumentParser _smartParser;

        public AzureDocumentController(
            IConfiguration configuration,
            ILogger<AzureDocumentController> logger,
            SmartDocumentParser smartParser)
        {
            _configuration = configuration;
            _logger = logger;
            _smartParser = smartParser;
        }

        [HttpPost("process")]
        [ProducesResponseType(typeof(AzureProcessResponseDto), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 400)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> ProcessDocument([Required] IFormFile file)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(AzureErrorResponseDto.ArchivoInvalido(file?.FileName));
                }

                _logger.LogInformation("🔄 PROCESAMIENTO PRINCIPAL: {FileName}", file.FileName);

                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                var client = new DocumentIntelligenceClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(apiKey));

                using var stream = file.OpenReadStream();
                var binaryData = BinaryData.FromStream(stream);

                _logger.LogInformation("📄 PASO 1: Extrayendo campos raw del PDF...");
                var operation = await client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    modelId,
                    binaryData);

                var analyzeResult = operation.Value;
                var camposRaw = new Dictionary<string, string>();

                if (analyzeResult.Documents?.Count > 0)
                {
                    var document = analyzeResult.Documents[0];
                    foreach (var field in document.Fields)
                    {
                        string rawValue = field.Value.ValueString ?? field.Value.Content ?? "";
                        camposRaw[field.Key] = rawValue.Trim();
                    }
                }

                _logger.LogInformation("✅ PASO 1 COMPLETADO: {CamposCount} campos raw extraídos", camposRaw.Count);
                _logger.LogInformation("🧠 PASO 2: Procesando datos...");

                var datosFormateados = _smartParser.ExtraerDatosInteligente(camposRaw);

                _logger.LogInformation("✅ PASO 2 COMPLETADO: Datos formateados exitosamente");

                stopwatch.Stop();

                var response = new AzureProcessResponseDto
                {
                    Archivo = file.FileName,
                    Timestamp = DateTime.UtcNow,
                    TiempoProcesamiento = stopwatch.ElapsedMilliseconds,
                    Estado = "PROCESADO_CON_SMART_EXTRACTION",

                    DatosFormateados = new AzureDatosFormateadosDto
                    {
                        NumeroPoliza = datosFormateados.NumeroPoliza,
                        Asegurado = datosFormateados.Asegurado,
                        Documento = datosFormateados.Documento,
                        Vehiculo = datosFormateados.Vehiculo,
                        Marca = datosFormateados.Marca,
                        Modelo = datosFormateados.Modelo,
                        Matricula = datosFormateados.Matricula,
                        Motor = datosFormateados.Motor,
                        Chasis = datosFormateados.Chasis,
                        PrimaComercial = datosFormateados.PrimaComercial,
                        PremioTotal = datosFormateados.PremioTotal,
                        VigenciaDesde = datosFormateados.VigenciaDesde,
                        VigenciaHasta = datosFormateados.VigenciaHasta,
                        Corredor = datosFormateados.Corredor,
                        Plan = datosFormateados.Plan,
                        Ramo = datosFormateados.Ramo,
                        Anio = datosFormateados.Anio,
                        Email = datosFormateados.Email,
                        Direccion = datosFormateados.Direccion,
                        Departamento = datosFormateados.Departamento,
                        Localidad = datosFormateados.Localidad
                    },

                    SiguientePaso = "completar_formulario", 

                    Resumen = new AzureResumenDto
                    {
                        ProcesamientoExitoso = true,
                        NumeroPolizaExtraido = datosFormateados.NumeroPoliza,
                        ClienteExtraido = datosFormateados.Asegurado,
                        DocumentoExtraido = datosFormateados.Documento,
                        VehiculoExtraido = datosFormateados.Vehiculo,
                        ClienteEncontrado = false, 
                        ListoParaVelneo = !string.IsNullOrEmpty(datosFormateados.NumeroPoliza) &&
                                         !string.IsNullOrEmpty(datosFormateados.Asegurado)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en procesamiento principal");
                return StatusCode(500, AzureErrorResponseDto.ErrorGeneral(ex.Message, file?.FileName));
            }
        }

        [HttpPost("process-batch")]
        [ProducesResponseType(typeof(AzureBatchResponseDto), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 400)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> ProcessBatch([Required] List<IFormFile> files)
        {
            var stopwatchTotal = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(AzureErrorResponseDto.ArchivoInvalido("Lista de archivos vacía"));
                }

                _logger.LogInformation("📦 PROCESAMIENTO EN LOTE: {FileCount} archivos", files.Count);

                var resultados = new List<AzureProcessResponseDto>();
                var errores = new List<AzureBatchErrorDto>();

                foreach (var file in files)
                {
                    try
                    {
                        var singleFileResult = await ProcessSingleFileInternal(file);
                        resultados.Add(singleFileResult);
                    }
                    catch (Exception ex)
                    {
                        errores.Add(new AzureBatchErrorDto
                        {
                            Archivo = file.FileName,
                            Error = ex.Message,
                            Timestamp = DateTime.UtcNow,
                            CodigoError = "ERROR_PROCESAMIENTO_INDIVIDUAL"
                        });
                    }
                }

                stopwatchTotal.Stop();

                var response = new AzureBatchResponseDto
                {
                    Procesados = resultados.Count,
                    Errores = errores.Count,
                    TotalArchivos = files.Count,
                    Resultados = resultados,
                    ErroresDetalle = errores,
                    FechaProcesamiento = DateTime.UtcNow,
                    TiempoTotalProcesamiento = stopwatchTotal.ElapsedMilliseconds
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en procesamiento en lote");
                return StatusCode(500, AzureErrorResponseDto.ErrorGeneral(ex.Message, "Procesamiento en lote"));
            }
        }

        [HttpGet("model-info")]
        [ProducesResponseType(typeof(AzureModelInfoResponseDto), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> GetModelInfo()
        {
            try
            {
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var endpointsToTry = new[]
                {
                    $"{endpoint.TrimEnd('/')}/documentintelligence/documentModels/{modelId}?api-version=2024-02-29-preview",
                    $"{endpoint.TrimEnd('/')}/documentintelligence/documentModels/{modelId}?api-version=2023-07-31",
                    $"{endpoint.TrimEnd('/')}/formrecognizer/documentModels/{modelId}?api-version=2022-08-31"
                };

                foreach (var url in endpointsToTry)
                {
                    try
                    {
                        _logger.LogInformation("Probando endpoint: {Url}", url);
                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            _logger.LogInformation("✅ ÉXITO con endpoint: {Url}", url);

                            var modelInfoResponse = new AzureModelInfoResponseDto
                            {
                                ModelId = modelId,
                                Endpoint = endpoint,
                                Status = "Modelo encontrado y activo",
                                WorkingApiUrl = url,
                                HttpStatus = response.StatusCode.ToString(),
                                ApiVersion = url.Contains("2024-02-29") ? "2024-02-29-preview" :
                                           url.Contains("2023-07-31") ? "2023-07-31" : "2022-08-31",
                                ConsultaTimestamp = DateTime.UtcNow
                            };

                            return Ok(modelInfoResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error con endpoint {Url}: {Error}", url, ex.Message);
                        continue;
                    }
                }

                var warningResponse = new AzureModelInfoResponseDto
                {
                    ModelId = modelId,
                    Endpoint = endpoint,
                    Status = "Configurado pero no accesible",
                    Warning = "El modelo está configurado pero no se pudo acceder a información detallada",
                    Message = "Esto es normal si el modelo es personalizado o está en una región específica",
                    ConsultaTimestamp = DateTime.UtcNow
                };

                return Ok(warningResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo información del modelo");
                return StatusCode(500, AzureErrorResponseDto.ErrorGeneral(ex.Message, "Consulta de modelo"));
            }
        }

        #region Métodos Privados de Soporte

        private async Task<AzureProcessResponseDto> ProcessSingleFileInternal(IFormFile file)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

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
            var camposRaw = new Dictionary<string, string>();

            if (analyzeResult.Documents?.Count > 0)
            {
                var document = analyzeResult.Documents[0];
                foreach (var field in document.Fields)
                {
                    string rawValue = field.Value.ValueString ?? field.Value.Content ?? "";
                    camposRaw[field.Key] = rawValue.Trim();
                }
            }

            var datosFormateados = _smartParser.ExtraerDatosInteligente(camposRaw);

            stopwatch.Stop();

            // ✅ RESPUESTA SIMPLIFICADA PARA PROCESAMIENTO EN LOTE
            return new AzureProcessResponseDto
            {
                Archivo = file.FileName,
                Timestamp = DateTime.UtcNow,
                TiempoProcesamiento = stopwatch.ElapsedMilliseconds,
                Estado = "PROCESADO_CON_SMART_EXTRACTION",
                DatosFormateados = new AzureDatosFormateadosDto
                {
                    NumeroPoliza = datosFormateados.NumeroPoliza,
                    Asegurado = datosFormateados.Asegurado,
                    Documento = datosFormateados.Documento,
                    Vehiculo = datosFormateados.Vehiculo,
                    Marca = datosFormateados.Marca,
                    Modelo = datosFormateados.Modelo,
                    Matricula = datosFormateados.Matricula,
                    Motor = datosFormateados.Motor,
                    Chasis = datosFormateados.Chasis,
                    PrimaComercial = datosFormateados.PrimaComercial,
                    PremioTotal = datosFormateados.PremioTotal,
                    VigenciaDesde = datosFormateados.VigenciaDesde,
                    VigenciaHasta = datosFormateados.VigenciaHasta,
                    Corredor = datosFormateados.Corredor,
                    Plan = datosFormateados.Plan,
                    Ramo = datosFormateados.Ramo,
                    Anio = datosFormateados.Anio,
                    Email = datosFormateados.Email,
                    Direccion = datosFormateados.Direccion,
                    Departamento = datosFormateados.Departamento,
                    Localidad = datosFormateados.Localidad
                },
                // ✅ SIN BÚSQUEDA DE CLIENTE EN PROCESAMIENTO BATCH
                // BusquedaCliente = null,
                SiguientePaso = "completar_formulario",
                Resumen = new AzureResumenDto
                {
                    ProcesamientoExitoso = true,
                    NumeroPolizaExtraido = datosFormateados.NumeroPoliza,
                    ClienteExtraido = datosFormateados.Asegurado,
                    DocumentoExtraido = datosFormateados.Documento,
                    VehiculoExtraido = datosFormateados.Vehiculo,
                    ClienteEncontrado = false, // ✅ Ya no buscamos cliente
                    ListoParaVelneo = !string.IsNullOrEmpty(datosFormateados.NumeroPoliza) &&
                                     !string.IsNullOrEmpty(datosFormateados.Asegurado)
                }
            };
        }

        #endregion
    }
}