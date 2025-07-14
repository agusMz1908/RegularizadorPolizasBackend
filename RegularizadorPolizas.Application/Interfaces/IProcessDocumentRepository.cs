namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentRepository : IGenericRepository<ProcessingDocumentDto>
    {
        // ================================
        // MÉTODOS ESPECÍFICOS PARA DASHBOARD (nombres únicos)
        // ================================

        // Contadores básicos
        Task<int> CountAllDocumentsInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<int> CountDocumentsByStatusInRangeAsync(string status, DateTime fromDate, DateTime toDate);

        // Costos
        Task<decimal> GetTotalCostInRangeAsync(DateTime fromDate, DateTime toDate);

        // Documentos recientes
        Task<List<ProcessingDocumentDto>> GetRecentDocumentsWithLimitAsync(int limit);
        Task<List<ProcessingDocumentDto>> GetRecentDocumentsByStatusWithLimitAsync(string status, int limit);

        // Por compañía
        Task<List<ProcessingDocumentDto>> GetAllDocumentsByCompanyAsync(int companyId);
        Task<List<ProcessingDocumentDto>> GetDocumentsByCompanyInRangeAsync(int companyId, DateTime fromDate, DateTime toDate);

        // Tiempos de procesamiento
        Task<double> GetAverageProcessingTimeForAllAsync();
        Task<double> GetAverageProcessingTimeInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<double> GetAverageProcessingTimeForCompanyAsync(int companyId);

        // Estados específicos
        Task<List<ProcessingDocumentDto>> GetProcessingDocumentsAsync();
        Task<List<ProcessingDocumentDto>> GetPendingDocumentsAsync();
        Task<List<ProcessingDocumentDto>> GetCompletedDocumentsAsync();
        Task<List<ProcessingDocumentDto>> GetErrorDocumentsAsync();
    }
}