using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentRepository : IGenericRepository<ProcessDocument>
    {
        Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status);
        Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId);
        Task<ProcessDocument> GetDocumentWithDetailsAsync(int documentId);
        Task<int> CountAllDocumentsInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<int> CountDocumentsByStatusInRangeAsync(string status, DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalCostInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<List<ProcessingDocumentDto>> GetRecentDocumentsWithLimitAsync(int limit);
        Task<List<ProcessingDocumentDto>> GetRecentDocumentsByStatusWithLimitAsync(string status, int limit);
        Task<List<ProcessingDocumentDto>> GetAllDocumentsByCompanyAsync(int companyId);
        Task<List<ProcessingDocumentDto>> GetDocumentsByCompanyInRangeAsync(int companyId, DateTime fromDate, DateTime toDate);
        Task<double> GetAverageProcessingTimeForAllAsync();
        Task<double> GetAverageProcessingTimeInRangeAsync(DateTime fromDate, DateTime toDate);
        Task<double> GetAverageProcessingTimeForCompanyAsync(int companyId);
        Task<List<ProcessingDocumentDto>> GetProcessingDocumentsAsync();
        Task<List<ProcessingDocumentDto>> GetPendingDocumentsAsync();
        Task<List<ProcessingDocumentDto>> GetCompletedDocumentsAsync();
        Task<List<ProcessingDocumentDto>> GetErrorDocumentsAsync();
    }
}