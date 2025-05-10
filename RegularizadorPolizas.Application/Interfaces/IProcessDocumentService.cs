using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentService
    {
        Task<ProcessDocumentDto> ProcessDocumentAsync(IFormFile file);
        Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentoId);
        Task<ProcessDocumentDto> GetDocumentProcessingResultAsync(int documentoId);
    }
}