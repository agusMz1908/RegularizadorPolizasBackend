public class DailyMetricsDto
{
    public DateTime Date { get; set; }
    public int DocumentCount { get; set; }
    public double SuccessRate { get; set; }
    public double AvgProcessingTime { get; set; }
    public decimal TotalCost { get; set; }
}