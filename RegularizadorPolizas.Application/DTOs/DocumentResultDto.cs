namespace RegularizadorPolizas.Application.DTOs
{
    public class DocumentResultDto
    {
        public int DocumentoId { get; set; }
        public string NombreArchivo { get; set; }
        public string EstadoProcesamiento { get; set; }
        public Dictionary<string, string> CamposExtraidos { get; set; }
        public decimal? ConfianzaExtraccion { get; set; }
        public bool RequiereRevision { get; set; }
        public string MensajeError { get; set; }

        // Constructor
        public DocumentResultDto()
        {
            CamposExtraidos = new Dictionary<string, string>();
        }
    }
}