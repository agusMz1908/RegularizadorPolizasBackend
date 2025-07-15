using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAzureDocumentIntelligenceService
    {
        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file);
        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file, string modelId);
        PolizaDto MapDocumentToPoliza(DocumentResultDto documento);
        Task<string> GetModelInfoAsync();
        Task<bool> TestConnectionAsync();
        Task<bool> TestConnectionWithDocumentAsync();
        Task<string> DebugAllModelsAsync();
    }
}