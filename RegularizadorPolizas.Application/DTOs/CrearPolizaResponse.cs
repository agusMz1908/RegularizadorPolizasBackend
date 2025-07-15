using RegularizadorPolizas.Application.DTOs;

public class CrearPolizaResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Poliza { get; set; } 
    public ClientDto? Cliente { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}