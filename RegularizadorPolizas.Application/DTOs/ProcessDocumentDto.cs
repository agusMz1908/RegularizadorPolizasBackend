namespace RegularizadorPolizas.Application.DTOs
{
    public class ProcessDocumentDto // Renombrado de ProcessDocumentDto para mantener consistencia
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

        // Propiedades adicionales
        public string UsuarioNombre { get; set; } // Nombre del usuario que procesó el documento
        public string PolizaNumero { get; set; } // Número de la póliza asociada (si existe)
        public decimal? TamanoArchivo { get; set; } // Tamaño del archivo en KB
        public string Extension { get; set; } // Extensión del archivo (PDF, JPG, etc.)
        public string MensajeError { get; set; } // Mensaje de error si hubo problemas en el procesamiento

        // Propiedades para la aplicación
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}