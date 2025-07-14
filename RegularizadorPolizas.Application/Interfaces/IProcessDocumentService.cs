using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Application.DTOs.Dashboard;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentRepository : IGenericRepository<ProcessDocument>
    {
        // ================================
        // MÉTODOS EXISTENTES QUE USA ProcessDocumentService
        // ================================

        /// <summary>
        /// Obtiene documento con detalles completos (incluye relaciones)
        /// </summary>
        Task<ProcessDocument> GetDocumentWithDetailsAsync(int documentId);

        /// <summary>
        /// Obtiene documentos asociados a una póliza específica
        /// </summary>
        Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId);

        /// <summary>
        /// Obtiene documentos filtrados por estado
        /// </summary>
        Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status);

        // ================================
        // MÉTODOS PARA DASHBOARD (simplificados)
        // ================================

        /// <summary>
        /// Cuenta documentos en un rango de fechas
        /// </summary>
        Task<int> CountDocumentsByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Cuenta documentos por estado y fecha
        /// </summary>
        Task<int> CountDocumentsByStatusAndDateAsync(string status, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene costo total en un rango de fechas
        /// </summary>
        Task<decimal> GetTotalCostByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene documentos recientes
        /// </summary>
        Task<List<ProcessDocument>> GetRecentDocumentsAsync(int limit, string status = null);

        /// <summary>
        /// Obtiene documentos por compañía
        /// </summary>
        Task<List<ProcessDocument>> GetDocumentsByCompanyAsync(int? companyId, DateTime? fromDate = null);

        /// <summary>
        /// Obtiene tiempo promedio de procesamiento
        /// </summary>
        Task<double> GetAverageProcessingTimeAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}