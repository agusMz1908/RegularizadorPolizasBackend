using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.DTOs.Audit
{
    public class AuditLogDto
    {
        public long Id { get; set; }
        public AuditEventType EventType { get; set; }
        public string EventTypeDescription { get; set; } = string.Empty;
        public AuditCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AdditionalData { get; set; }
        public DateTime Timestamp { get; set; }
        public string? TableName { get; set; }
        public string Severity { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public long? DurationMs { get; set; }
    }
}