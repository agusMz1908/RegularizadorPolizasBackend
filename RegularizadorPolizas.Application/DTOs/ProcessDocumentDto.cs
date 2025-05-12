namespace RegularizadorPolizas.Application.DTOs
{
    public class ProcessDocumentDto
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public string TipoDocumento { get; set; }
        public string EstadoProcesamiento { get; set; }
        public string ResultadoJson { get; set; }
        public int? PolizaId { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        public int? UsuarioId { get; set; }

        // Navigation properties
        public string UsuarioNombre { get; set; }
        public string PolizaNumero { get; set; }
        public decimal? TamanoArchivo { get; set; }
        public string Extension { get; set; }
        public string MensajeError { get; set; }

        // Application properties
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}