using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentService
    {
        Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentId);
        Task<IEnumerable<ProcessingDocumentDto>> GetAllProcessedDocumentsAsync();
        Task<DocumentResultDto> GetDocumentProcessingResultAsync(int documentId);
        Task<IEnumerable<ProcessingDocumentDto>> GetDocumentsByPolizaAsync(int polizaId);
        Task<IEnumerable<ProcessingDocumentDto>> GetDocumentsByStatusAsync(string status);
        Task<ProcessingDocumentDto> LinkDocumentToPolizaAsync(int documentId, int polizaId);
        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file);
    }
}