namespace RegularizadorPolizas.Application.DTOs
{
    public class DocumentResultDto
    {
        public int DocumentoId { get; set; }

        public string NombreArchivo { get; set; } = string.Empty;

        public string EstadoProcesamiento { get; set; } = string.Empty;

        public Dictionary<string, string> CamposExtraidos { get; set; } = new Dictionary<string, string>();

        public decimal? ConfianzaExtraccion { get; set; }

        public bool RequiereRevision { get; set; }

        public string MensajeError { get; set; } = string.Empty;

        // Constructor por defecto que inicializa las propiedades requeridas
        public DocumentResultDto()
        {
            NombreArchivo = string.Empty;
            EstadoProcesamiento = string.Empty;
            CamposExtraidos = new Dictionary<string, string>();
            MensajeError = string.Empty;
            RequiereRevision = false;
        }

        // Constructor con parámetros básicos
        public DocumentResultDto(int documentoId, string nombreArchivo, string estadoProcesamiento)
        {
            DocumentoId = documentoId;
            NombreArchivo = nombreArchivo ?? string.Empty;
            EstadoProcesamiento = estadoProcesamiento ?? string.Empty;
            CamposExtraidos = new Dictionary<string, string>();
            MensajeError = string.Empty;
            RequiereRevision = false;
        }

        // Método helper para agregar campos extraídos de forma segura
        public void AgregarCampo(string clave, string? valor)
        {
            if (!string.IsNullOrEmpty(clave) && valor != null)
            {
                CamposExtraidos[clave] = valor;
            }
        }

        // Método para obtener un campo de forma segura
        public string? ObtenerCampo(string clave)
        {
            return CamposExtraidos.TryGetValue(clave, out var valor) ? valor : null;
        }

        // Propiedad para verificar si el procesamiento fue exitoso
        public bool ProcesamientoExitoso =>
            EstadoProcesamiento.Equals("PROCESADO", StringComparison.OrdinalIgnoreCase) ||
            EstadoProcesamiento.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);

        // Propiedad para obtener el número total de campos extraídos
        public int TotalCamposExtraidos => CamposExtraidos?.Count ?? 0;

        // Método para validar que el documento tiene la información mínima
        public bool TieneInformacionMinima()
        {
            return DocumentoId > 0 &&
                   !string.IsNullOrEmpty(NombreArchivo) &&
                   !string.IsNullOrEmpty(EstadoProcesamiento);
        }

        // Método para obtener un resumen del documento
        public string ObtenerResumen()
        {
            return $"Doc ID: {DocumentoId}, Archivo: {NombreArchivo}, " +
                   $"Estado: {EstadoProcesamiento}, Campos: {TotalCamposExtraidos}, " +
                   $"Confianza: {ConfianzaExtraccion?.ToString("P") ?? "N/A"}";
        }
    }
}