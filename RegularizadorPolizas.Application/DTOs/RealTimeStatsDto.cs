using RegularizadorPolizas.Application.DTOs;

public class RealTimeStatsDto
{
    public int DocumentsProcessingNow { get; set; }
    public int QueueLength { get; set; }
    public List<ProcessingDocumentDto> CurrentProcessing { get; set; }
    public DateTime LastUpdate { get; set; }
}