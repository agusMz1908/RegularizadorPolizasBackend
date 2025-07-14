public class ServiceStatusDto
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; } 
    public double ResponseTime { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime LastCheck { get; set; }
}