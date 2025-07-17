using RegularizadorPolizas.Application.DTOs;

public class DocumentExtractResult
{
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaProcesamiento { get; set; } = DateTime.Now;
    public string EstadoProcesamiento { get; set; } = string.Empty;
    public DatosPolizaExtraidos DatosPoliza { get; set; } = new();

    public decimal ConfianzaExtraccion { get; set; }
    public long TiempoProcesamiento { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public string RutaArchivoOriginal { get; set; } = string.Empty;
}

