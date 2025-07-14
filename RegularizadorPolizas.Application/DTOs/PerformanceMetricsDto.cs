using RegularizadorPolizas.Application.DTOs;

public class PerformanceMetricsDto
{
    public double AvgProcessingTime { get; set; }
    public double MedianProcessingTime { get; set; }
    public double SuccessRate { get; set; }
    public int TotalDocuments { get; set; }
    public int SuccessfulDocuments { get; set; }
    public int FailedDocuments { get; set; }
    public List<DailyMetricsDto> DailyMetrics { get; set; }
}