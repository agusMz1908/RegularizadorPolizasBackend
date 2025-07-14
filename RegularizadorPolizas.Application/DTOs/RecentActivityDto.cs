public class RecentActivityDto
{
    public string Id { get; set; }
    public string DocumentName { get; set; }
    public string CompanyCode { get; set; }
    public string CompanyName { get; set; }
    public string Status { get; set; } 
    public DateTime Timestamp { get; set; }
    public double? ProcessingTime { get; set; }
    public string ErrorMessage { get; set; }
    public decimal? Cost { get; set; }
    public int PageCount { get; set; }
}