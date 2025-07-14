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