using Azure.AI.DocumentIntelligence;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegularizadorPolizas.Application.Services;

namespace RegularizadorPolizas.Infrastructure.External
{
    public class AzureDocumentIntelligenceService : IAzureDocumentIntelligenceService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _modelId;
        private readonly DocumentIntelligenceClient _client;
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

            _client = new DocumentIntelligenceClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_apiKey));

            _parser = new VelneoDocumentResultParser(null);

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

                // Usar BinaryData directamente desde el stream
                var binaryData = BinaryData.FromStream(stream);

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    _modelId,
                    binaryData);

                var analyzeResult = operation.Value;
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

                // Usar el modelId pasado como parámetro en lugar del predeterminado
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    modelId,
                    binaryData);

                var analyzeResult = operation.Value;
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

        #endregion

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

        #region Métodos de Diagnóstico - CORREGIDOS

        public async Task<string> GetModelInfoAsync()
        {
            try
            {
                _logger.LogInformation("Getting model information for model: {ModelId}", _modelId);

                // Usar HttpClient directo con las URLs correctas
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                // PROBAR MÚLTIPLES ENDPOINTS HASTA ENCONTRAR EL CORRECTO
                var endpointsToTry = new[]
                {
                    // Document Intelligence v3.1 (más nuevo)
                    $"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2023-07-31",
                    
                    // Document Intelligence v3.0  
                    $"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels/{_modelId}?api-version=2022-08-31",
                    
                    // Document Intelligence v2.1 (legacy)
                    $"{_endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{_modelId}",
                    
                    // Nuevo endpoint 2024
                    $"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2024-02-29-preview"
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
                                Timestamp = DateTime.UtcNow,
                                ModelDetails = content.Substring(0, Math.Min(500, content.Length)) + "..."
                            };

                            return JsonConvert.SerializeObject(result, Formatting.Indented);
                        }
                        else
                        {
                            _logger.LogWarning("❌ Failed with {Url}: {Status}", url, response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("❌ Exception with {Url}: {Message}", url, ex.Message);
                    }
                }

                // Si ningún endpoint funciona
                var failInfo = new
                {
                    ModelId = _modelId,
                    Endpoint = _endpoint,
                    Status = "Model Not Found",
                    Message = "Tried multiple API versions, none worked",
                    Timestamp = DateTime.UtcNow
                };

                return JsonConvert.SerializeObject(failInfo, Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting model info for {ModelId}", _modelId);

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

                // MÉTODO 1: Probar listando todos los modelos disponibles
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

                            // Verificar si nuestro modelo está en la lista
                            if (content.Contains(_modelId))
                            {
                                _logger.LogInformation("✅ Model '{ModelId}' found in models list!", _modelId);
                                return true;
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Connected but model '{ModelId}' not found in list", _modelId);
                                _logger.LogInformation("Available models: {Content}", content.Substring(0, Math.Min(1000, content.Length)));
                                return false; // Conexión OK, pero modelo no existe
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            _logger.LogError("❌ Authentication failed: Invalid API key");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed with {Url}: {Message}", listUrl, ex.Message);
                    }
                }

                // MÉTODO 2: Si falla el listado, probar con documento mínimo
                _logger.LogInformation("Models list failed, trying with minimal document test...");
                return await TestConnectionWithDocumentAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed for Azure Document Intelligence");
                return false;
            }
        }

        public async Task<bool> TestConnectionWithDocumentAsync()
        {
            try
            {
                _logger.LogInformation("Testing connection with minimal document (fallback method)");

                var minimalPdf = CreateMinimalPdf();
                var binaryData = BinaryData.FromBytes(minimalPdf);

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Started,
                    _modelId,
                    binaryData);

                if (operation != null && !string.IsNullOrEmpty(operation.Id))
                {
                    _logger.LogInformation("Connection successful - Operation started with ID: {OperationId}", operation.Id);
                    return true;
                }

                return false;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 401)
            {
                _logger.LogError("Authentication failed: {Message}", ex.Message);
                return false;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogError("Model not found: {Message}", ex.Message);
                return false;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 400)
            {
                _logger.LogWarning("Document format issue but authentication works: {Message}", ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during document connection test");
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

                // PASO 1: Listar todos los modelos con diferentes APIs
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
                                        if (modelIdFound == _modelId)
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

                // PASO 2: Probar acceso directo al modelo específico
                results.Add("--- TESTING DIRECT MODEL ACCESS ---");
                var modelEndpoints = new[]
                {
                    ($"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2023-07-31", "Document Intelligence v3.1"),
                    ($"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels/{_modelId}?api-version=2022-08-31", "Form Recognizer v3.0"),
                    ($"{_endpoint.TrimEnd('/')}/documentintelligence/documentModels/{_modelId}?api-version=2024-02-29-preview", "Document Intelligence Preview"),
                    ($"{_endpoint.TrimEnd('/')}/formrecognizer/v2.1/custom/models/{_modelId}", "Form Recognizer v2.1 (Legacy)")
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

                return string.Join("\n", results);
            }
            catch (Exception ex)
            {
                results.Add($"CRITICAL ERROR: {ex.Message}");
                return string.Join("\n", results);
            }
        }

        // Método auxiliar para crear un PDF mínimo válido
        private byte[] CreateMinimalPdf()
        {
            var pdfContent = "%PDF-1.4\n" +
                            "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n" +
                            "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n" +
                            "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R >> endobj\n" +
                            "4 0 obj << /Length 44 >> stream\n" +
                            "BT /F1 12 Tf 100 700 Td (Test Document) Tj ET\n" +
                            "endstream endobj\n" +
                            "xref\n" +
                            "0 5\n" +
                            "0000000000 65535 f \n" +
                            "0000000009 00000 n \n" +
                            "0000000058 00000 n \n" +
                            "0000000115 00000 n \n" +
                            "0000000205 00000 n \n" +
                            "trailer << /Size 5 /Root 1 0 R >>\n" +
                            "startxref\n" +
                            "284\n" +
                            "%%EOF";

            return System.Text.Encoding.ASCII.GetBytes(pdfContent);
        }

        #endregion
    }
}