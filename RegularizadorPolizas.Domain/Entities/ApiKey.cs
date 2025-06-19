using System;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ApiKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaExpiracion { get; set; }
        public string? Descripcion { get; set; }

        public string Environment { get; set; } = "Production";
        public int? MaxRequestsPerMinute { get; set; } 
        public DateTime? LastUsed { get; set; }
        public string? ContactEmail { get; set; }

        public bool EnableLogging { get; set; } = true;
        public bool EnableRetries { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;

        public string ApiVersion { get; set; } = "v1";
    }
}