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
