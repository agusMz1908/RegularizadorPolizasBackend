namespace RegularizadorPolizas.Application.DTOs.Frontend
{
    public class ClientSummaryDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }        // ClientDto.Clinom
        public string Documento { get; set; }     // ClientDto.Cliced (persona) o ClientDto.Cliruc (empresa)
        public string Rut { get; set; }           // ClientDto.Cliruc
        public string Telefono { get; set; }      // ClientDto.Telefono
        public string Celular { get; set; }       // ClientDto.Clitelcel
        public string Domicilio { get; set; }     // ClientDto.Clidir
        public string Email { get; set; }         // ClientDto.Cliemail
        public int TotalPolizas { get; set; }     // Calculado
        public bool TienePolizasVigentes { get; set; } // Calculado
        public DateTime? UltimaPoliza { get; set; }    // Calculado
        public string EstadoCliente { get; set; } = "Activo"; // ClientDto.Activo

        // Campos adicionales útiles
        public string TipoCliente { get; set; }   // "Persona" o "Empresa" 
        public string RazonSocial { get; set; }   // ClientDto.Clirsoc (si es empresa)
    }
}