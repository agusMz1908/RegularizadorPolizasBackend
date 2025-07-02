using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RegularizadorPolizas.Infrastructure.External
{
    public class AzureDocumentIntelligenceService : IAzureDocumentIntelligenceService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _modelId;
        private readonly DocumentAnalysisClient _client;
        private readonly VelneoDocumentResultParser _parser;
        private readonly ILogger<AzureDocumentIntelligenceService> _logger;

        public AzureDocumentIntelligenceService(
            IConfiguration configuration,
            ILogger<AzureDocumentIntelligenceService> logger)
        {
            _endpoint = configuration["AzureDocumentIntelligence:Endpoint"];
            _apiKey = configuration["AzureDocumentIntelligence:ApiKey"];
            _modelId = configuration["AzureDocumentIntelligence:ModelId"];
            _logger = logger;

            if (string.IsNullOrEmpty(_endpoint) || string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_modelId))
            {
                throw new InvalidOperationException("Azure Document Intelligence configuration is missing");
            }

            _client = new DocumentAnalysisClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_apiKey));

            _parser = new VelneoDocumentResultParser(logger);
        }

        public async Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new DocumentResultDto
                {
                    NombreArchivo = file?.FileName ?? "unknown",
                    EstadoProcesamiento = "ERROR",
                    MensajeError = "El archivo está vacío o no se ha proporcionado.",
                    FechaProcesamiento = DateTime.Now
                };
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting document processing for file: {FileName}, Size: {FileSize} bytes",
                    file.FileName, file.Length);

                using var stream = file.OpenReadStream();

                var analyzeOperation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    _modelId,
                    stream);

                var analyzeResult = analyzeOperation.Value;
                var azureResultJson = ConvertAnalyzeResultToJObject(analyzeResult);
                var polizaDto = _parser.ParseToPolizaDto(azureResultJson);
                var camposExtraidos = ExtractFieldsDictionary(azureResultJson);
                stopwatch.Stop();

                var resultado = new DocumentResultDto
                {
                    NombreArchivo = file.FileName,
                    EstadoProcesamiento = "PROCESADO",
                    CamposExtraidos = camposExtraidos,
                    ConfianzaExtraccion = (decimal)(analyzeResult.Documents?.FirstOrDefault()?.Confidence ?? 0),
                    RequiereRevision = DetermineIfReviewRequired(polizaDto, analyzeResult),
                    FechaProcesamiento = DateTime.Now,
                    TiempoProcesamiento = stopwatch.ElapsedMilliseconds,

                    // Datos de la póliza procesada para Velneo
                    PolizaProcesada = polizaDto
                };

                _logger.LogInformation("Document processed successfully in {ElapsedMs}ms. Policy: {PolicyNumber}, Client: {ClientName}",
                    stopwatch.ElapsedMilliseconds, polizaDto.Conpol, polizaDto.Clinom);

                return resultado;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing document {FileName} after {ElapsedMs}ms",
                    file.FileName, stopwatch.ElapsedMilliseconds);

                return new DocumentResultDto
                {
                    NombreArchivo = file.FileName,
                    EstadoProcesamiento = "ERROR",
                    MensajeError = $"Error al procesar el documento: {ex.Message}",
                    FechaProcesamiento = DateTime.Now,
                    TiempoProcesamiento = stopwatch.ElapsedMilliseconds
                };
            }
        }

        public PolizaDto MapDocumentToPoliza(DocumentResultDto documento)
        {
            try
            {
                if (documento.PolizaProcesada != null)
                {
                    _logger.LogDebug("Returning pre-processed policy data for document {DocumentName}", documento.NombreArchivo);
                    return documento.PolizaProcesada;
                }

                _logger.LogWarning("Using fallback mapping for document {DocumentName}", documento.NombreArchivo);

                var poliza = new PolizaDto
                {
                    Comcod = 1,
                    Seccod = 4,
                    Moncod = 1,
                    Convig = "1",
                    Consta = "1",
                    Contra = "2",
                    Ramo = "AUTOMOVILES",
                    Last_update = DateTime.Now,
                    Ingresado = DateTime.Now,
                    Observaciones = "Procesado con mapeo de fallback - revisar manualmente"
                };

                if (documento.CamposExtraidos != null)
                {
                    MapFromExtractedFields(poliza, documento.CamposExtraidos);
                }

                return poliza;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping document {DocumentName} to PolizaDto", documento.NombreArchivo);

                return new PolizaDto
                {
                    Comcod = 1,
                    Seccod = 4,
                    Moncod = 1,
                    Convig = "1",
                    Consta = "1",
                    Contra = "2",
                    Ramo = "AUTOMOVILES",
                    Last_update = DateTime.Now,
                    Ingresado = DateTime.Now,
                    Observaciones = $"Error en mapeo: {ex.Message}"
                };
            }
        }

        #region Métodos de Conversión y Utilidades

        private JObject ConvertAnalyzeResultToJObject(AnalyzeResult analyzeResult)
        {
            try
            {
                var azureJson = JsonConvert.SerializeObject(analyzeResult, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                });

                var result = new JObject
                {
                    ["status"] = "succeeded",
                    ["createdDateTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ["lastUpdatedDateTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ["analyzeResult"] = JObject.Parse(azureJson)
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting AnalyzeResult to JObject");

                return new JObject
                {
                    ["status"] = "succeeded",
                    ["analyzeResult"] = new JObject
                    {
                        ["content"] = analyzeResult.Content ?? "",
                        ["documents"] = new JArray(),
                        ["tables"] = new JArray(),
                        ["paragraphs"] = new JArray()
                    }
                };
            }
        }

        private Dictionary<string, string> ExtractFieldsDictionary(JObject azureResult)
        {
            var campos = new Dictionary<string, string>();

            try
            {
                var analyzeResult = azureResult["analyzeResult"];
                if (analyzeResult == null) return campos;

                var documents = analyzeResult["documents"] as JArray;
                if (documents?.Count > 0)
                {
                    var document = documents[0];
                    var fields = document["fields"];

                    if (fields != null)
                    {
                        foreach (var field in fields.Children<JProperty>())
                        {
                            var fieldValue = field.Value["content"]?.ToString() ??
                                           field.Value["valueString"]?.ToString() ?? "";

                            if (!string.IsNullOrEmpty(fieldValue))
                            {
                                campos[field.Name] = fieldValue;
                            }
                        }
                    }
                }

                var content = analyzeResult["content"]?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    campos["_content_completo"] = content;
                }

                _logger.LogDebug("Extracted {FieldCount} fields from Azure result", campos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting fields dictionary from Azure result");
            }

            return campos;
        }

        private bool DetermineIfReviewRequired(PolizaDto poliza, AnalyzeResult analyzeResult)
        {
            var requiresReview = false;
            var reasons = new List<string>();

            var confidence = analyzeResult.Documents?.FirstOrDefault()?.Confidence ?? 0;
            if (confidence < 0.8)
            {
                requiresReview = true;
                reasons.Add($"Confianza baja del modelo: {confidence:P1}");
            }

            if (string.IsNullOrEmpty(poliza.Conpol))
            {
                requiresReview = true;
                reasons.Add("Número de póliza no detectado");
            }

            if (string.IsNullOrEmpty(poliza.Clinom))
            {
                requiresReview = true;
                reasons.Add("Nombre del asegurado no detectado");
            }

            if (string.IsNullOrEmpty(poliza.Conmataut) && string.IsNullOrEmpty(poliza.Conpadaut))
            {
                requiresReview = true;
                reasons.Add("Datos del vehículo incompletos");
            }

            if (poliza.Conpremio <= 0 && poliza.Contot <= 0)
            {
                requiresReview = true;
                reasons.Add("Montos financieros no detectados");
            }

            if (requiresReview)
            {
                _logger.LogWarning("Document requires manual review. Reasons: {Reasons}", string.Join(", ", reasons));

                if (poliza.Observaciones == null)
                    poliza.Observaciones = "";

                poliza.Observaciones += "\nREQUIERE REVISIÓN:\n" + string.Join("\n- ", reasons);
            }

            return requiresReview;
        }

        private void MapFromExtractedFields(PolizaDto poliza, Dictionary<string, string> campos)
        {
            if (campos.TryGetValue("poliza.numero", out var numeroPoliza))
                poliza.Conpol = numeroPoliza;

            if (campos.TryGetValue("asegurado.nombre", out var nombreAsegurado))
                poliza.Clinom = nombreAsegurado;

            if (campos.TryGetValue("vehiculo.matricula", out var matricula))
                poliza.Conmataut = matricula;

            // Agregar más mapeos según sea necesario...
        }

        #endregion

        #region Métodos de Diagnóstico

        public async Task<string> GetModelInfoAsync()
        {
            try
            {
                var modelInfo = await _client.GetDocumentModelAsync(_modelId);
                return JsonConvert.SerializeObject(modelInfo.Value, Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting model info for {ModelId}", _modelId);
                return $"Error: {ex.Message}";
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await _client.GetDocumentModelAsync(_modelId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed for Azure Document Intelligence");
                return false;
            }
        }

        #endregion
    }
}