using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentService
    {
        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file);
        Task<DocumentResultDto> GetDocumentProcessingResultAsync(int documentId);
        Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentId);
        Task<IEnumerable<ProcessDocumentDto>> GetAllProcessedDocumentsAsync();
        Task<IEnumerable<ProcessDocumentDto>> GetDocumentsByStatusAsync(string status);
        Task<IEnumerable<ProcessDocumentDto>> GetDocumentsByPolizaAsync(int polizaId);
        Task<ProcessDocumentDto> LinkDocumentToPolizaAsync(int documentId, int polizaId);
    }
}