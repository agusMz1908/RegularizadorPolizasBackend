namespace RegularizadorPolizas.Application.DTOs
{
    public class RenovationDto // Renombrado de RenovationDto a RenovacionDto para mantener consistencia
    {
        public int Id { get; set; }
        public int PolizaId { get; set; }
        public int? PolizaNuevaId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public int? UsuarioId { get; set; }

        public string NombrePoliza { get; set; } // Para mostrar información básica de la póliza
        public string NombreUsuario { get; set; } // Para mostrar quién solicitó la renovación
    }
}