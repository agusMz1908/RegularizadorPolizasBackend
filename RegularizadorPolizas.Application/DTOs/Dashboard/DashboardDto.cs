namespace RegularizadorPolizas.Application.DTOs.Dashboard
{
    public class DashboardOverviewDto
    {
        public int DocumentsToday { get; set; }
        public int DocumentsMonth { get; set; }
        public int DocumentsTotal { get; set; }
        public decimal CostToday { get; set; }
        public decimal CostMonth { get; set; }
        public decimal CostTotal { get; set; }
        public double AvgSuccessRate { get; set; }
        public double AvgProcessingTime { get; set; }
        public int ActiveCompanies { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class CompanyStatsDto
    {
        public string CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int DocumentsToday { get; set; }
        public int DocumentsMonth { get; set; }
        public decimal CostToday { get; set; }
        public decimal CostMonth { get; set; }
        public double SuccessRate { get; set; }
        public double AvgProcessingTime { get; set; }
        public DateTime? LastProcessed { get; set; }
        public bool IsActive { get; set; }
    }

    public class RecentActivityDto
    {
        public string Id { get; set; }
        public string DocumentName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal? ProcessingTime { get; set; }
        public string ErrorMessage { get; set; }
        public decimal? Cost { get; set; }
        public int PageCount { get; set; }
    }

    public class PerformanceMetricsDto
    {
        public double AvgProcessingTime { get; set; }
        public double MedianProcessingTime { get; set; }
        public double SuccessRate { get; set; }
        public int TotalDocuments { get; set; }
        public int SuccessfulDocuments { get; set; }
        public int FailedDocuments { get; set; }
        public List<DailyMetricsDto> DailyMetrics { get; set; } = new List<DailyMetricsDto>();
    }

    public class DailyMetricsDto
    {
        public DateTime Date { get; set; }
        public int DocumentCount { get; set; }
        public double SuccessRate { get; set; }
        public double AvgProcessingTime { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class RealTimeStatsDto
    {
        public int DocumentsProcessingNow { get; set; }
        public int QueueLength { get; set; }
        public List<ProcessingDocumentDto> CurrentProcessing { get; set; } = new List<ProcessingDocumentDto>(); // ✅ Usa la versión importada
        public DateTime LastUpdate { get; set; }
    }

    public class ServiceHealthDto
    {
        public ServiceStatusDto AzureDocumentIntelligence { get; set; }
        public ServiceStatusDto VelneoApi { get; set; }
        public ServiceStatusDto Database { get; set; }
        public bool OverallHealthy { get; set; }
        public DateTime LastCheck { get; set; }
    }

    public class ServiceStatusDto
    {
        public bool IsHealthy { get; set; }
        public string Status { get; set; }
        public double ResponseTime { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime LastCheck { get; set; }
    }
}