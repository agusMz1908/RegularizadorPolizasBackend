namespace RegularizadorPolizas.Application.DTOs.Frontend
{
    public class DocumentProcessResultDto
    {
        public string NombreArchivo { get; set; }
        public string EstadoProcesamiento { get; set; }
        public decimal ConfianzaExtraccion { get; set; }
        public long TiempoProcesamiento { get; set; }
        public bool RequiereRevision { get; set; }
        public DateTime FechaProcesamiento { get; set; }

        // Para el frontend React
        public string PdfViewerUrl { get; set; }
        public ClientSummaryDto ClienteAsociado { get; set; }
        public PolizaDto PolizaDatos { get; set; }
        public bool RequiereVerificacion { get; set; }
        public List<string> CamposConBajaConfianza { get; set; }
        public Dictionary<string, decimal> ConfianzaPorCampo { get; set; }
        public string EstadoFormulario { get; set; } 

        // Información adicional
        public string TipoDocumentoDetectado { get; set; }
        public string CompaniaDetectada { get; set; }
        public bool EsRenovacion { get; set; }
    }
}