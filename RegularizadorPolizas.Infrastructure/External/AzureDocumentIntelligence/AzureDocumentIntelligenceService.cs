using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

public class AzureDocumentIntelligenceService : IAzureDocumentIntelligenceService
{
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly string _modelId;
    private readonly DocumentAnalysisClient _client;
    private readonly DocumentResultParser _parser;

    public AzureDocumentIntelligenceService(IConfiguration configuration)
    {
        _endpoint = configuration["AzureDocumentIntelligence:Endpoint"];
        _apiKey = configuration["AzureDocumentIntelligence:ApiKey"];
        _modelId = configuration["AzureDocumentIntelligence:ModelId"];

        _client = new DocumentAnalysisClient(
            new Uri(_endpoint),
            new AzureKeyCredential(_apiKey));

        _parser = new DocumentResultParser();
    }

    public async Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return new DocumentResultDto
            {
                EstadoProcesamiento = "ERROR",
                MensajeError = "El archivo está vacío o no se ha proporcionado."
            };
        }

        try
        {
            using var stream = file.OpenReadStream();
            var analyzeResult = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, _modelId, stream);
            var result = analyzeResult.Value;

            var camposExtraidos = new Dictionary<string, string>();

            foreach (var document in result.Documents)
            {
                foreach (var field in document.Fields)
                {
                    var fieldName = field.Key;
                    var fieldValue = field.Value.Content;
                    camposExtraidos[fieldName] = fieldValue;
                }
            }

            return new DocumentResultDto
            {
                NombreArchivo = file.FileName,
                EstadoProcesamiento = "PROCESADO",
                CamposExtraidos = camposExtraidos,
                ConfianzaExtraccion = result.Documents.Count > 0 ? (decimal)result.Documents[0].Confidence : 0,
                RequiereRevision = result.Documents.Count == 0 || result.Documents[0].Confidence < 0.8
            };
        }
        catch (Exception ex)
        {
            return new DocumentResultDto
            {
                NombreArchivo = file.FileName,
                EstadoProcesamiento = "ERROR",
                MensajeError = $"Error al procesar el documento: {ex.Message}"
            };
        }
    }

    public PolizaDto MapDocumentToPoliza(DocumentResultDto documento)
    {
        return _parser.ParseToPolizaDto(documento);
    }
}