using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAzureDocumentIntelligenceService
    {
        Task<DocumentResutDto> ProcessDocumentAsync(IFormFile file);
        PolizaDto MapDocumentToPoliza(DocumentResutDto documento);
    }
}