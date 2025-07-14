public class ServiceHealthDto
{
    public ServiceStatusDto AzureDocumentIntelligence { get; set; }
    public ServiceStatusDto VelneoApi { get; set; }
    public ServiceStatusDto Database { get; set; }
    public bool OverallHealthy { get; set; }
    public DateTime LastCheck { get; set; }
}