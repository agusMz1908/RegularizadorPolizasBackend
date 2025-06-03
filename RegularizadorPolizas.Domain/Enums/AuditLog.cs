using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public AuditEventType EventType { get; set; }

        [Required]
        public AuditCategory Category { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        public int? UserId { get; set; }

        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [StringLength(45)]
        public string IpAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string UserAgent { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Datos antes del cambio (para UPDATE y DELETE)
        [Column(TypeName = "TEXT")]
        public string? OldValues { get; set; }

        // Datos después del cambio (para CREATE y UPDATE)
        [Column(TypeName = "TEXT")]
        public string? NewValues { get; set; }

        [Column(TypeName = "TEXT")]
        public string? AdditionalData { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? TableName { get; set; }

        [StringLength(50)]
        public string? SessionId { get; set; }

        [StringLength(20)]
        public string Severity { get; set; } = "Info"; 

        [StringLength(50)]
        public string? RequestId { get; set; }

        public long? DurationMs { get; set; }

        public bool Success { get; set; } = true;

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        [StringLength(2000)]
        public string? StackTrace { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}