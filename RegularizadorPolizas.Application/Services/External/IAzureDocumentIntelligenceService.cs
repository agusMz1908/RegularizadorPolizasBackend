using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAzureDocumentIntelligenceService
    {

        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file);
        PolizaDto MapDocumentToPoliza(DocumentResultDto documento);
        Task<string> GetModelInfoAsync();
        Task<bool> TestConnectionAsync();
    }
}