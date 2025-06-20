using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class ApiKeyDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty; // "VELNEO" | "LOCAL"
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string? Descripcion { get; set; }
        public string Environment { get; set; } = string.Empty;
        public int? MaxRequestsPerMinute { get; set; }
        public DateTime? LastUsed { get; set; }
        public string? ContactEmail { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableRetries { get; set; }
        public int TimeoutSeconds { get; set; }
        public string ApiVersion { get; set; } = string.Empty;
    }

    public class ApiKeyCreateDto
    {
        [Required(ErrorMessage = "La clave de API es requerida")]
        [MinLength(8, ErrorMessage = "La clave debe tener al menos 8 caracteres")]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "El TenantId es requerido")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]*$", ErrorMessage = "TenantId debe empezar con una letra y contener solo letras, números y guiones bajos")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "TenantId debe tener entre 2 y 50 caracteres")]
        public string TenantId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL base es requerida")]
        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string BaseUrl { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime? FechaExpiracion { get; set; }

        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El ambiente es requerido")]
        public string Environment { get; set; } = "Production";

        [Required(ErrorMessage = "El modo es requerido")]
        [RegularExpression("^(VELNEO|LOCAL)$", ErrorMessage = "El modo debe ser 'VELNEO' o 'LOCAL'")]
        public string Mode { get; set; } = "VELNEO";

        [Range(1, 10000, ErrorMessage = "El límite debe estar entre 1 y 10000 requests por minuto")]
        public int? MaxRequestsPerMinute { get; set; }

        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string? ContactEmail { get; set; }

        public bool EnableLogging { get; set; } = true;
        public bool EnableRetries { get; set; } = true;

        [Range(5, 300, ErrorMessage = "El timeout debe estar entre 5 y 300 segundos")]
        public int TimeoutSeconds { get; set; } = 30;

        public string ApiVersion { get; set; } = "v1";
    }

    public class ApiKeyUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [MinLength(8, ErrorMessage = "La clave debe tener al menos 8 caracteres")]
        public string? Key { get; set; }

        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string? BaseUrl { get; set; }

        public bool? Activo { get; set; }

        public DateTime? FechaExpiracion { get; set; }

        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
        public string? Descripcion { get; set; }

        public string? Environment { get; set; }

        [Range(1, 10000, ErrorMessage = "El límite debe estar entre 1 y 10000 requests por minuto")]
        public int? MaxRequestsPerMinute { get; set; }

        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string? ContactEmail { get; set; }

        public bool? EnableLogging { get; set; }
        public bool? EnableRetries { get; set; }

        [Range(5, 300, ErrorMessage = "El timeout debe estar entre 5 y 300 segundos")]
        public int? TimeoutSeconds { get; set; }

        public string? ApiVersion { get; set; }
    }

    public class ApiKeySearchDto
    {
        public string? SearchTerm { get; set; }
        public string? Environment { get; set; }
        public bool? Activo { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public bool? ShowExpired { get; set; }
        public int? UnusedDays { get; set; }
    }

    public class TenantConfigurationDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; }
        public bool EnableRetries { get; set; }
        public string ApiVersion { get; set; } = string.Empty;
        public DateTime? LastUsed { get; set; }
        public bool IsActive { get; set; }
    }
}