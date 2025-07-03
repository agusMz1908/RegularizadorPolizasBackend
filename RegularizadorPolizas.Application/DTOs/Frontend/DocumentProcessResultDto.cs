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
        public string PdfViewerUrl { get; set; }
        public PolizaDto PolizaDatos { get; set; }
        public bool RequiereVerificacion { get; set; }
        public List<string> CamposConBajaConfianza { get; set; }
        public Dictionary<string, decimal> ConfianzaPorCampo { get; set; }
        public string EstadoFormulario { get; set; }
        public string TipoDocumentoDetectado { get; set; }
        public string CompaniaSeleccionada { get; set; }
        public string TipoOperacion { get; set; }
    }
}