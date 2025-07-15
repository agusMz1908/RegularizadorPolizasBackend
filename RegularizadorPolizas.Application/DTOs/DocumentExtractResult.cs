using RegularizadorPolizas.Application.DTOs;

public class DocumentExtractResult
{
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaProcesamiento { get; set; } = DateTime.Now;
    public string EstadoProcesamiento { get; set; } = string.Empty;

    // Datos específicos de la póliza (del documento)
    public DatosPolizaExtraidos DatosPoliza { get; set; } = new();

    // Datos del cliente (para búsqueda)
    public DatosClienteExtraidos DatosClienteBusqueda { get; set; } = new();

    // Metadatos
    public decimal ConfianzaExtraccion { get; set; }
    public long TiempoProcesamiento { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public string RutaArchivoOriginal { get; set; } = string.Empty;
}

