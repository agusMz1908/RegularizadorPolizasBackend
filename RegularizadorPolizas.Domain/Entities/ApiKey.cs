using System;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ApiKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty; // Nombre que va en api_name (KEYDEMO, BRIGNONI, etc.)
        public string BaseUrl { get; set; } = string.Empty; // https://app.uruguaycom.com/apid/Seguros_dat
        public string Mode { get; set; } = "VELNEO"; // "VELNEO" | "LOCAL"
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaExpiracion { get; set; }
        public string? Descripcion { get; set; }

        public string Environment { get; set; } = "Production"; // Production, Staging, Development
        public int? MaxRequestsPerMinute { get; set; } // Rate limiting
        public DateTime? LastUsed { get; set; } // Para auditoría
        public string? ContactEmail { get; set; } // Email del contacto técnico del cliente
        public bool EnableLogging { get; set; } = true;
        public bool EnableRetries { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;

        // Para futuras versiones de API
        public string ApiVersion { get; set; } = "v1";
    }
}