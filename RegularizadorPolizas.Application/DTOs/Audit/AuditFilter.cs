using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.DTOs.Audit
{
    public class AuditFilter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AuditCategory? Category { get; set; }
        public AuditEventType? EventType { get; set; }
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public bool? Success { get; set; }
        public string? Severity { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SearchTerm { get; set; }
        public string SortBy { get; set; } = "Timestamp";
        public string SortDirection { get; set; } = "DESC";
    }
}