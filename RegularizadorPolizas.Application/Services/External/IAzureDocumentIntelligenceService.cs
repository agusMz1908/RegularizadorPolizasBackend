using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAzureDocumentIntelligenceService
    {
        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file);
        PolizaDto MapDocumentToPoliza(DocumentResultDto documento);
    }
}