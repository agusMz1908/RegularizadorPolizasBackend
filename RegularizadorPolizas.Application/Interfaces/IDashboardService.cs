public interface IDashboardService
{
    Task<DashboardOverviewDto> GetOverviewStatsAsync(DateTime? fromDate, DateTime? toDate);
    Task<List<CompanyStatsDto>> GetCompanyStatsAsync(DateTime? fromDate, DateTime? toDate);
    Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit, string status);
    Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(int days);
    Task<RealTimeStatsDto> GetRealTimeStatsAsync();
    Task<ServiceHealthDto> GetServicesHealthAsync();
}