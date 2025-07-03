using Microsoft.AspNetCore.Http;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SavePdfAsync(IFormFile file, int clientId);
        Task<string> GetPdfUrlAsync(string fileName);
        Task<bool> DeletePdfAsync(string fileName);
        Task<byte[]> GetPdfBytesAsync(string fileName);
        Task<bool> PdfExistsAsync(string fileName);
        Task<long> GetPdfSizeAsync(string fileName);
    }
}