using Azure.AI.DocumentIntelligence;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegularizadorPolizas.Application.Interfaces.External.AzureDocumentIntelligence;

namespace RegularizadorPolizas.Infrastructure.External
{
    public class AzureDocumentIntelligenceService : IAzureDocumentIntelligenceService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _modelId;
        private readonly DocumentIntelligenceClient _client;
        private readonly DocumentResultParser _parser;
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

            _client = new DocumentIntelligenceClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_apiKey));

            _parser = new DocumentResultParser(); // ✅ Sin logger para evitar type mismatch

            _logger.LogInformation("Azure Document Intelligence Service initialized:");
            _logger.LogInformation("- Endpoint: {Endpoint}", _endpoint);
            _logger.LogInformation("- Model ID: {ModelId}", _modelId);
            _logger.LogInformation("- API Key configured: {HasKey}", !string.IsNullOrEmpty(_apiKey));
            _logger.LogInformation("- API Key length: {KeyLength}", _apiKey?.Length ?? 0);
        }

        #region Métodos Principales de Procesamiento

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
                var binaryData = BinaryData.FromStream(stream);

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    _modelId,
                    binaryData);

                var analyzeResult = operation.Value;
                var polizaDto = _parser.ParseToPolizaDto(analyzeResult);
                var camposExtraidos = ExtractFieldsDictionary(analyzeResult);
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
                    PolizaProcesada = polizaDto
                };

                _logger.LogInformation("Document processed successfully in {ElapsedMs}ms. Policy: {PolicyNumber}, Client: {ClientName}",
                    stopwatch.ElapsedMilliseconds, polizaDto?.Conpol, polizaDto?.Clinom);

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

        public async Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file, string modelId)
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
                _logger.LogInformation("Starting document processing for file: {FileName}, Size: {FileSize} bytes with model: {ModelId}",
                    file.FileName, file.Length, modelId);

                using var stream = file.OpenReadStream();
                var binaryData = BinaryData.FromStream(stream);

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    modelId,
                    binaryData);

                var analyzeResult = operation.Value;
                var polizaDto = _parser.ParseToPolizaDto(analyzeResult);
                var camposExtraidos = ExtractFieldsDictionary(analyzeResult);
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
                    PolizaProcesada = polizaDto
                };

                _logger.LogInformation("Document processed successfully in {ElapsedMs}ms. Policy: {PolicyNumber}, Client: {ClientName}",
                    stopwatch.ElapsedMilliseconds, polizaDto?.Conpol, polizaDto?.Clinom);

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
                return _parser.ParseToPolizaDto(documento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping document {DocumentName} to poliza", documento.NombreArchivo);

                return new PolizaDto
                {
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    Procesado = false
                };
            }
        }

        #endregion

        #region Métodos Auxiliares

        private Dictionary<string, string> ExtractFieldsDictionary(AnalyzeResult analyzeResult)
        {
            var campos = new Dictionary<string, string>();

            try
            {
                if (analyzeResult?.Documents == null)
                    return campos;

                foreach (var document in analyzeResult.Documents)
                {
                    if (document.Fields == null) continue;

                    foreach (var field in document.Fields)
                    {
                        var value = ExtractFieldValue(field.Value);
                        if (!string.IsNullOrEmpty(value))
                        {
                            campos[field.Key] = value;
                        }
                    }
                }

                // También extraer de key-value pairs si están disponibles
                if (analyzeResult.KeyValuePairs != null)
                {
                    foreach (var kvp in analyzeResult.KeyValuePairs)
                    {
                        var key = kvp.Key?.Content?.ToLowerInvariant()
                            .Replace(" ", "_")
                            .Replace(":", "")
                            .Replace(".", "")
                            .Replace("-", "_") ?? "";

                        var value = kvp.Value?.Content?.Trim() ?? "";

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && value.Length > 1)
                        {
                            campos[key] = value;
                        }
                    }
                }

                _logger.LogDebug("Extracted {FieldCount} fields from Azure result", campos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting fields dictionary from Azure result");
            }

            return campos;
        }

        private string ExtractFieldValue(DocumentField field)
        {
            if (field == null) return string.Empty;

            // ✅ CORREGIDO: Solo usar propiedades que realmente existen
            if (!string.IsNullOrEmpty(field.Content))
                return field.Content;

            // Fallback a ValueString si Content está vacío
            if (!string.IsNullOrEmpty(field.ValueString))
                return field.ValueString;

            return string.Empty;
        }

        private bool DetermineIfReviewRequired(PolizaDto? poliza, AnalyzeResult analyzeResult)
        {
            if (poliza == null) return true;

            var requiresReview = false;
            var reasons = new List<string>();

            // Verificar confianza general
            var confidence = analyzeResult.Documents?.FirstOrDefault()?.Confidence ?? 0;
            if (confidence < 0.8)
            {
                requiresReview = true;
                reasons.Add($"Confianza baja del modelo: {confidence:P1}");
            }

            // Verificar campos críticos
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

                poliza.Observaciones += "\nREQUIERE REVISIÓN:\n- " + string.Join("\n- ", reasons);
            }

            return requiresReview;
        }

        #endregion

        #region Métodos de Diagnóstico

        public async Task<string> GetModelInfoAsync()
        {
            try
            {
                _logger.LogInformation("Getting model information for model: {ModelId}", _modelId);

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                var endpointsToTry = new[]
                {
                    $"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2023-07-31",
                    $"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels/{_modelId}?api-version=2022-08-31",
                    $"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2024-02-29-preview",
                    $"{_endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{_modelId}"
                };

                foreach (var url in endpointsToTry)
                {
                    try
                    {
                        _logger.LogInformation("Trying endpoint: {Url}", url);
                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            _logger.LogInformation("✅ SUCCESS with endpoint: {Url}", url);

                            var result = new
                            {
                                ModelId = _modelId,
                                Endpoint = _endpoint,
                                Status = "Model Found",
                                WorkingApiUrl = url,
                                HttpStatus = response.StatusCode,
                                ModelDetails = JObject.Parse(content)
                            };

                            return JsonConvert.SerializeObject(result, Formatting.Indented);
                        }
                        else
                        {
                            _logger.LogWarning("❌ Failed with endpoint: {Url}, Status: {Status}", url, response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error with endpoint: {Url}", url);
                    }
                }

                return JsonConvert.SerializeObject(new
                {
                    ModelId = _modelId,
                    Endpoint = _endpoint,
                    Status = "Model Not Found",
                    Error = "No working endpoint found"
                }, Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting model information");

                var errorInfo = new
                {
                    ModelId = _modelId,
                    Endpoint = _endpoint,
                    Status = "Error",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                };

                return JsonConvert.SerializeObject(errorInfo, Formatting.Indented);
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                _logger.LogInformation("Testing Azure Document Intelligence connection");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                var listEndpoints = new[]
                {
                    $"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2023-07-31",
                    $"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels?api-version=2022-08-31",
                    $"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2024-02-29-preview"
                };

                foreach (var listUrl in listEndpoints)
                {
                    try
                    {
                        _logger.LogInformation("Testing models list with: {Url}", listUrl);
                        var response = await httpClient.GetAsync(listUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            _logger.LogInformation("✅ Successfully connected! Found models list.");

                            if (content.Contains(_modelId))
                            {
                                _logger.LogInformation("✅ Model '{ModelId}' found in models list!", _modelId);
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Model '{ModelId}' not found in models list", _modelId);
                            }

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to test endpoint: {Url}", listUrl);
                    }
                }

                _logger.LogError("❌ All connection tests failed");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection");
                return false;
            }
        }

        public async Task<bool> TestConnectionWithDocumentAsync()
        {
            try
            {
                _logger.LogInformation("Testing connection with a small document");

                // Crear un PDF mínimo para testing
                var testPdfBytes = Convert.FromBase64String(
                    "JVBERi0xLjMKJcTl8uXrp/Og0MTGCjEgMCBvYmoKPDwKL1R5cGUgL0NhdGFsb2cKL091dGxpbmVzIDIgMCBSCi9QYWdlcyAzIDAgUgo+PgplbmRvYmoKMiAwIG9iago8PAovVHlwZSAvT3V0bGluZXMKL0NvdW50IDAKPD4KZW5kb2JqCjMgMCBvYmoKPDwKL1R5cGUgL1BhZ2VzCi9Db3VudCAxCi9LaWRzIFs0IDAgUl0KPj4KZW5kb2JqCjQgMCBvYmoKPDwKL1R5cGUgL1BhZ2UKL1BhcmVudCAzIDAgUgovUmVzb3VyY2VzIDw8Ci9Gb250IDw8Ci9GMSA5IDAgUgo+Pgo+PgovTWVkaWFCb3ggWzAgMCA2MTIgNzkyXQovQ29udGVudHMgNSAwIFIKPj4KZW5kb2JqCjUgMCBvYmoKPDwKL0xlbmd0aCA0NAo+PgpzdHJlYW0KQlQKL0YxIDEyIFRmCjEwMCA3MDAgVGQKKFRlc3QpIFRqCkVUCmVuZHN0cmVhbQplbmRvYmoKOSAwIG9iago8PAovVHlwZSAvRm9udAovU3VidHlwZSAvVHlwZTEKL0Jhc2VGb250IC9UaW1lcy1Sb21hbgo+PgplbmRvYmoKeHJlZgowIDEwCjAwMDAwMDAwMDAgNjU1MzUgZiAKMDAwMDAwMDAwOSAwMDAwMCBuIAowMDAwMDAwMDc0IDAwMDAwIG4gCjAwMDAwMDAxMjAgMDAwMDAgbiAKMDAwMDAwMDE3NyAwMDAwMCBuIAowMDAwMDAwMzY0IDAwMDAwIG4gCjAwMDAwMDA0NTggMDAwMDAgbiAKdHJhaWxlcgo8PAovU2l6ZSAxMAovUm9vdCAxIDAgUgo+PgpzdGFydHhyZWYKNTE4CiUlRU9G");

                using var memoryStream = new MemoryStream(testPdfBytes);
                var binaryData = BinaryData.FromStream(memoryStream);

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    _modelId,
                    binaryData);

                var result = operation.Value;
                _logger.LogInformation("✅ Document processing test successful!");

                return true;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 401)
            {
                _logger.LogError("❌ Authentication failed: {Message}", ex.Message);
                return false;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogError("❌ Model not found: {Message}", ex.Message);
                return false;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 400)
            {
                _logger.LogWarning("⚠️ Document format issue but authentication works: {Message}", ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during document connection test");
                return false;
            }
        }

        public async Task<string> DebugAllModelsAsync()
        {
            var results = new List<string>();

            try
            {
                results.Add("=== AZURE DOCUMENT INTELLIGENCE COMPLETE DEBUG ===");
                results.Add($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                results.Add($"Configured Endpoint: {_endpoint}");
                results.Add($"Configured Model ID: {_modelId}");
                results.Add($"API Key Length: {_apiKey?.Length ?? 0}");
                results.Add("");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                var listEndpoints = new[]
                {
                    ($"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2023-07-31", "Document Intelligence v3.1"),
                    ($"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels?api-version=2022-08-31", "Form Recognizer v3.0"),
                    ($"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels?api-version=2024-02-29-preview", "Document Intelligence Preview"),
                    ($"{_endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models", "Form Recognizer v2.1 (Legacy)")
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

                            try
                            {
                                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);
                                var models = jsonResponse?.value ?? jsonResponse?.modelList;

                                if (models != null)
                                {
                                    results.Add($"📋 Available Models ({((IEnumerable<dynamic>)models).Count()}):");
                                    foreach (var model in models)
                                    {
                                        var modelId = model?.modelId?.ToString() ?? "unknown";
                                        var status = model?.status?.ToString() ?? "unknown";
                                        var createdDate = model?.createdDateTime?.ToString() ?? "unknown";

                                        var indicator = modelId == _modelId ? "👉 [TARGET MODEL]" : "   ";
                                        results.Add($"{indicator} ID: {modelId} | Status: {status} | Created: {createdDate}");
                                    }
                                }
                                else
                                {
                                    results.Add("⚠️ No models array found in response");
                                }
                            }
                            catch (Exception parseEx)
                            {
                                results.Add($"⚠️ Error parsing models list: {parseEx.Message}");
                                results.Add($"Raw response preview: {content.Substring(0, Math.Min(200, content.Length))}...");
                            }
                        }
                        else
                        {
                            results.Add($"❌ FAILED with {name}: {response.StatusCode}");
                            if (response.Content != null)
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                results.Add($"   Error: {errorContent.Substring(0, Math.Min(100, errorContent.Length))}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"❌ EXCEPTION with {name}: {ex.Message}");
                    }
                    results.Add("");
                }

                // Test direct model access
                var modelEndpoints = new[]
                {
                    ($"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2023-07-31", "Document Intelligence v3.1"),
                    ($"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels/{_modelId}?api-version=2022-08-31", "Form Recognizer v3.0"),
                    ($"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2024-02-29-preview", "Document Intelligence Preview"),
                    ($"{_endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{_modelId}", "Form Recognizer v2.1 (Legacy)")
                };

                results.Add("=== DIRECT MODEL ACCESS TESTS ===");
                foreach (var (url, name) in modelEndpoints)
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            results.Add($"✅ DIRECT ACCESS SUCCESS: {name}");
                            results.Add($"   URL: {url}");

                            var content = await response.Content.ReadAsStringAsync();
                            results.Add($"   Response preview: {content.Substring(0, Math.Min(200, content.Length))}...");
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

                results.Add("");
                results.Add("=== SUMMARY ===");
                results.Add($"Target Model ID: {_modelId}");
                results.Add($"Model Found: {(results.Any(r => r.Contains("[TARGET MODEL]")) ? "YES" : "NO")}");
                results.Add($"Connection Working: {(results.Any(r => r.Contains("SUCCESS")) ? "YES" : "NO")}");
                results.Add("");
                results.Add("Debug completed successfully!");

                return string.Join("\n", results);
            }
            catch (Exception ex)
            {
                results.Add($"FATAL ERROR during debug: {ex.Message}");
                return string.Join("\n", results);
            }
        }

        #endregion
    }
}