using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentRepository : IGenericRepository<ProcessDocument>
    {
        // ================================
        // MÉTODOS BÁSICOS PARA ProcessDocumentService
        // ================================
        Task<ProcessDocument> GetDocumentWithDetailsAsync(int documentId);
        Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId);
        Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status);

        // ================================
        // MÉTODOS ESPECÍFICOS PARA DASHBOARD (nombres únicos)
        // ================================

        // Contadores básicos
        Task<int> CountAllDocumentsInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<int> CountDocumentsByStatusInRangeAsync(string status, DateTime fromDate, DateTime toDate);

        // Costos
        Task<decimal> GetTotalCostInRangeAsync(DateTime fromDate, DateTime toDate);

        // Documentos recientes
        Task<List<ProcessDocument>> GetRecentDocumentsWithLimitAsync(int limit);
        Task<List<ProcessDocument>> GetRecentDocumentsByStatusWithLimitAsync(string status, int limit);

        // Por compañía
        Task<List<ProcessDocument>> GetAllDocumentsByCompanyAsync(int companyId);
        Task<List<ProcessDocument>> GetDocumentsByCompanyInRangeAsync(int companyId, DateTime fromDate, DateTime toDate);

        // Tiempos de procesamiento
        Task<double> GetAverageProcessingTimeForAllAsync();
        Task<double> GetAverageProcessingTimeInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<double> GetAverageProcessingTimeForCompanyAsync(int companyId);

        // Estados específicos
        Task<List<ProcessDocument>> GetProcessingDocumentsAsync();
        Task<List<ProcessDocument>> GetPendingDocumentsAsync();
        Task<List<ProcessDocument>> GetCompletedDocumentsAsync();
        Task<List<ProcessDocument>> GetErrorDocumentsAsync();
    }
}