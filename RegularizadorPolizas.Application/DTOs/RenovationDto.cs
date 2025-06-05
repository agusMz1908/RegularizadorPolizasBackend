namespace RegularizadorPolizas.Application.DTOs
{
    public class RenovationDto
    {
        public int Id { get; set; }
        public int PolizaId { get; set; }
        public int? PolizaNuevaId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }

        // Propiedades para mostrar información básica
        public string NombrePoliza { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}