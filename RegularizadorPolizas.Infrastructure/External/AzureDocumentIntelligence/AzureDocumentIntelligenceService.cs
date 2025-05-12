using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence
{
    public class AzureDocumentIntelligenceService : IAzureDocumentIntelligenceService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _modelId;
        private readonly DocumentAnalysisClient _client;

        public AzureDocumentIntelligenceService(IConfiguration configuration)
        {
            _endpoint = configuration["AzureDocumentIntelligence:Endpoint"];
            _apiKey = configuration["AzureDocumentIntelligence:ApiKey"];
            _modelId = configuration["AzureDocumentIntelligence:ModelId"];

            _client = new DocumentAnalysisClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_apiKey));
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

                // Extraer los valores de los campos reconocidos
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
            if (documento == null || documento.CamposExtraidos == null || documento.CamposExtraidos.Count == 0)
            {
                return null;
            }

            var poliza = new PolizaDto();
            var campos = documento.CamposExtraidos;

            if (campos.TryGetValue("NumeroPóliza", out var numeroPóliza))
                poliza.Conpol = numeroPóliza;

            if (campos.TryGetValue("FechaInicio", out var fechaInicio) && DateTime.TryParse(fechaInicio, out var fechaInicioDate))
                poliza.Confchdes = fechaInicioDate;

            if (campos.TryGetValue("FechaVencimiento", out var fechaVenc) && DateTime.TryParse(fechaVenc, out var fechaVencDate))
                poliza.Confchhas = fechaVencDate;

            if (campos.TryGetValue("NombreAsegurado", out var nombreAsegurado))
                poliza.Clinom = nombreAsegurado;

            if (campos.TryGetValue("MarcaVehículo", out var marcaVehiculo))
                poliza.Conmaraut = marcaVehiculo;

            if (campos.TryGetValue("Matrícula", out var matricula))
                poliza.Conmataut = matricula;

            if (campos.TryGetValue("Motor", out var motor))
                poliza.Conmotor = motor;

            if (campos.TryGetValue("Chasis", out var chasis))
                poliza.Conchasis = chasis;

            if (campos.TryGetValue("Año", out var anio) && int.TryParse(anio, out var anioInt))
                poliza.Conanioaut = anioInt;

            if (campos.TryGetValue("Premio", out var premio) && decimal.TryParse(premio, out var premioDecimal))
                poliza.Conpremio = premioDecimal;

            if (campos.TryGetValue("Compañía", out var compania))
                poliza.Com_alias = compania;

            if (campos.TryGetValue("Ramo", out var ramo))
                poliza.Ramo = ramo;

            return poliza;
        }
    }
}