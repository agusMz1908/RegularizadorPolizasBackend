using Microsoft.AspNetCore.Http;

namespace RegularizadorPolizas.Application.Interfaces
{
    /// <summary>
    /// Servicio para extraer datos de documentos y crear pólizas
    /// </summary>
    public interface IDocumentExtractionService
    {
        /// <summary>
        /// Procesa un documento y extrae datos de póliza y cliente
        /// </summary>
        Task<DocumentExtractResult> ProcessDocumentAsync(IFormFile file);

        /// <summary>
        /// Crea una póliza vinculada a un cliente específico
        /// </summary>
        Task<CrearPolizaResponse> CrearPolizaConClienteAsync(CrearPolizaConClienteRequest request);
    }
}