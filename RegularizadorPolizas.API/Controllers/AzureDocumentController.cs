using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using System.ComponentModel.DataAnnotations;
using static ClienteMatchResult;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AzureDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureDocumentController> _logger;
        private readonly IClienteMatchingService _clienteMatchingService;
        private readonly SmartDocumentParser _smartParser;

        public AzureDocumentController(
            IConfiguration configuration,
            ILogger<AzureDocumentController> logger,
            IClienteMatchingService clienteMatchingService,
            SmartDocumentParser smartParser)
        {
            _configuration = configuration;
            _logger = logger;
            _clienteMatchingService = clienteMatchingService;
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

                _logger.LogInformation("✅PASO 1 COMPLETADO: {CamposCount} campos raw extraídos", camposRaw.Count);
                _logger.LogInformation("🧠 PASO 2: Procesando datos...");
                var datosFormateados = _smartParser.ExtraerDatosInteligente(camposRaw);

                _logger.LogInformation("✅ PASO 2 COMPLETADO: Datos formateados exitosamente");
                _logger.LogInformation("🔍 PASO 3: Buscando cliente automáticamente...");

                ClienteMatchResult? resultadoBusqueda = null;

                try
                {
                    var datosParaBusqueda = _smartParser.CrearDatosClienteBusqueda(datosFormateados);
                    resultadoBusqueda = await _clienteMatchingService.BuscarClienteAsync(datosParaBusqueda);

                    _logger.LogInformation("✅ PASO 3 COMPLETADO: {TipoResultado} - {MatchCount} clientes encontrados",
                        resultadoBusqueda.TipoResultado, resultadoBusqueda.Matches.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en búsqueda de cliente");
                    resultadoBusqueda = new ClienteMatchResult
                    {
                        TipoResultado = TipoResultadoCliente.SinCoincidencias,
                        MensajeUsuario = $"Error en búsqueda: {ex.Message}",
                        RequiereIntervencionManual = true,
                        Matches = new List<ClienteMatch>()
                    };
                }

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

                    BusquedaCliente = new AzureBusquedaClienteDto
                    {
                        TipoResultado = resultadoBusqueda?.TipoResultado.ToString() ?? "SinCoincidencias",
                        Mensaje = resultadoBusqueda?.MensajeUsuario ?? "No se pudo realizar búsqueda",
                        RequiereIntervencion = resultadoBusqueda?.RequiereIntervencionManual ?? true,
                        ClientesEncontrados = resultadoBusqueda?.Matches.Count ?? 0,
                        Matches = resultadoBusqueda?.Matches.Select(m => new AzureClienteMatchDto
                        {
                            Cliente = new AzureClienteInfoDto
                            {
                                Id = m.Cliente.Clinro,
                                Nombre = m.Cliente.Clinom,
                                Documento = m.Cliente.Cliruc,
                                Telefono = m.Cliente.Clitelcel,
                                Email = m.Cliente.Cliemail,
                                Direccion = m.Cliente.Clidir
                            },
                            Score = m.Score,
                            Criterio = m.Criterio,
                            Coincidencias = m.Coincidencias
                        }).ToList() ?? new List<AzureClienteMatchDto>()
                    },

                    SiguientePaso = DeterminarSiguientePaso(resultadoBusqueda),

                    Resumen = new AzureResumenDto
                    {
                        ProcesamientoExitoso = true,
                        NumeroPolizaExtraido = datosFormateados.NumeroPoliza,
                        ClienteExtraido = datosFormateados.Asegurado,
                        DocumentoExtraido = datosFormateados.Documento,
                        VehiculoExtraido = datosFormateados.Vehiculo,
                        ClienteEncontrado = resultadoBusqueda?.TipoResultado == TipoResultadoCliente.MatchExacto,
                        ListoParaVelneo = resultadoBusqueda?.TipoResultado == TipoResultadoCliente.MatchExacto &&
                                         !string.IsNullOrEmpty(datosFormateados.NumeroPoliza)
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

        [HttpPost("search-client")]
        [ProducesResponseType(typeof(ClienteMatchResult), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 400)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> SearchClient([FromBody] DatosClienteExtraidos datosExtraidos)
        {
            try
            {
                if (datosExtraidos == null)
                {
                    return BadRequest(AzureErrorResponseDto.ErrorGeneral("Datos del cliente requeridos"));
                }

                _logger.LogInformation("🔍 BÚSQUEDA MANUAL: Nombre='{Nombre}', Doc='{Documento}'",
                    datosExtraidos.Nombre, datosExtraidos.Documento);

                var resultado = await _clienteMatchingService.BuscarClienteAsync(datosExtraidos);

                _logger.LogInformation("✅ BÚSQUEDA COMPLETADA: {TipoResultado} - {Count} matches",
                    resultado.TipoResultado, resultado.Matches.Count);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda manual de cliente");
                return StatusCode(500, AzureErrorResponseDto.ErrorBusquedaCliente(ex.Message));
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

            ClienteMatchResult? resultadoBusqueda = null;
            try
            {
                var datosParaBusqueda = _smartParser.CrearDatosClienteBusqueda(datosFormateados);
                resultadoBusqueda = await _clienteMatchingService.BuscarClienteAsync(datosParaBusqueda);
            }
            catch (Exception ex)
            {
                resultadoBusqueda = new ClienteMatchResult
                {
                    TipoResultado = TipoResultadoCliente.SinCoincidencias,
                    MensajeUsuario = $"Error en búsqueda: {ex.Message}",
                    RequiereIntervencionManual = true,
                    Matches = new List<ClienteMatch>()
                };
            }

            stopwatch.Stop();

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
                BusquedaCliente = new AzureBusquedaClienteDto
                {
                    TipoResultado = resultadoBusqueda?.TipoResultado.ToString() ?? "SinCoincidencias",
                    Mensaje = resultadoBusqueda?.MensajeUsuario ?? "No se pudo realizar búsqueda",
                    RequiereIntervencion = resultadoBusqueda?.RequiereIntervencionManual ?? true,
                    ClientesEncontrados = resultadoBusqueda?.Matches.Count ?? 0,
                    Matches = resultadoBusqueda?.Matches.Select(m => new AzureClienteMatchDto
                    {
                        Cliente = new AzureClienteInfoDto
                        {
                            Id = m.Cliente.Clinro,
                            Nombre = m.Cliente.Clinom,
                            Documento = m.Cliente.Cliruc,
                            Telefono = m.Cliente.Clitelcel,
                            Email = m.Cliente.Cliemail,
                            Direccion = m.Cliente.Clidir
                        },
                        Score = m.Score,
                        Criterio = m.Criterio,
                        Coincidencias = m.Coincidencias
                    }).ToList() ?? new List<AzureClienteMatchDto>()
                },
                SiguientePaso = DeterminarSiguientePaso(resultadoBusqueda),
                Resumen = new AzureResumenDto
                {
                    ProcesamientoExitoso = true,
                    NumeroPolizaExtraido = datosFormateados.NumeroPoliza,
                    ClienteExtraido = datosFormateados.Asegurado,
                    DocumentoExtraido = datosFormateados.Documento,
                    VehiculoExtraido = datosFormateados.Vehiculo,
                    ClienteEncontrado = resultadoBusqueda?.TipoResultado == TipoResultadoCliente.MatchExacto,
                    ListoParaVelneo = resultadoBusqueda?.TipoResultado == TipoResultadoCliente.MatchExacto &&
                                     !string.IsNullOrEmpty(datosFormateados.NumeroPoliza)
                }
            };
        }

        private string DeterminarSiguientePaso(ClienteMatchResult? resultado)
        {
            if (resultado == null) return "buscar_cliente_manualmente";

            return resultado.TipoResultado switch
            {
                TipoResultadoCliente.MatchExacto => "crear_poliza_automatico",
                TipoResultadoCliente.MatchMuyProbable => "confirmar_cliente",
                TipoResultadoCliente.MultiplesMatches => "seleccionar_cliente",
                _ => "buscar_cliente_manualmente"
            };
        }

        #endregion
    }
}