using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentRepository : IGenericRepository<ProcessDocument>
    {
        Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status);
        Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId);
        Task<ProcessDocument> GetDocumentWithDetailsAsync(int id);
        Task<DashboardOverviewDto> GetOverviewStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<CompanyStatsDto>> GetCompanyStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit = 10, string status = null);
        Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(int days = 30);
        Task<List<ProcessingDocumentDto>> GetCurrentlyProcessingAsync();
        Task<int> CountDocumentsByStatusAndDateAsync(string status, DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalCostAsync(DateTime fromDate, DateTime toDate);
        Task<double> GetAverageProcessingTimeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<double> GetSuccessRateAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}