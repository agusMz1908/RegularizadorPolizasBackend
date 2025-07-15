using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IDocumentExtractionService
    {
        Task<DocumentExtractResult> ProcessDocumentAsync(IFormFile file);
        Task<CrearPolizaResponse> CrearPolizaConClienteAsync(CrearPolizaConClienteRequest request);
    }
}