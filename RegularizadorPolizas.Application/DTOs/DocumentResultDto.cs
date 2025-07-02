namespace RegularizadorPolizas.Application.DTOs
{
    public class DocumentResultDto
    {
        public int? DocumentoId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string EstadoProcesamiento { get; set; } = string.Empty;
        public string? MensajeError { get; set; }
        public Dictionary<string, string>? CamposExtraidos { get; set; }
        public decimal ConfianzaExtraccion { get; set; }
        public bool RequiereRevision { get; set; }
        public DateTime FechaProcesamiento { get; set; }
        public long TiempoProcesamiento { get; set; }
        public PolizaDto? PolizaProcesada { get; set; }
        public string? ModeloUsado { get; set; }
        public int? NumeroPaginas { get; set; }
        public string? TipoDocumento { get; set; }
        public int CamposDetectados { get; set; }
        public int CamposMapeados { get; set; }
        public List<string>? AdvertenciasMapeo { get; set; }

        public DocumentResultDto()
        {
            NombreArchivo = string.Empty;
            EstadoProcesamiento = string.Empty;
            CamposExtraidos = new Dictionary<string, string>();
            MensajeError = string.Empty;
            RequiereRevision = false;
        }

        public DocumentResultDto(int documentoId, string nombreArchivo, string estadoProcesamiento)
        {
            DocumentoId = documentoId;
            NombreArchivo = nombreArchivo ?? string.Empty;
            EstadoProcesamiento = estadoProcesamiento ?? string.Empty;
            CamposExtraidos = new Dictionary<string, string>();
            MensajeError = string.Empty;
            RequiereRevision = false;
        }

        public void AgregarCampo(string clave, string? valor)
        {
            if (!string.IsNullOrEmpty(clave) && valor != null)
            {
                CamposExtraidos[clave] = valor;
            }
        }

        public string? ObtenerCampo(string clave)
        {
            return CamposExtraidos.TryGetValue(clave, out var valor) ? valor : null;
        }

        public bool ProcesamientoExitoso =>
            EstadoProcesamiento.Equals("PROCESADO", StringComparison.OrdinalIgnoreCase) ||
            EstadoProcesamiento.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);

        public int TotalCamposExtraidos => CamposExtraidos?.Count ?? 0;

        public bool TieneInformacionMinima()
        {
            return DocumentoId > 0 &&
                   !string.IsNullOrEmpty(NombreArchivo) &&
                   !string.IsNullOrEmpty(EstadoProcesamiento);
        }

        public string ObtenerResumen()
        {
            return $"Doc ID: {DocumentoId}, Archivo: {NombreArchivo}, " +
                   $"Estado: {EstadoProcesamiento}, Campos: {TotalCamposExtraidos}, " +
                   $"Confianza: {ConfianzaExtraccion.ToString("P") ?? "N/A"}";
        }
    }
}