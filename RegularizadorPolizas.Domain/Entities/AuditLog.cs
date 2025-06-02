using System;
using Newtonsoft.Json;

namespace RegularizadorPolizas.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }                               
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
        public string EntityName { get; set; } 
        public int? RecordId { get; set; } 
        public string ActionType { get; set; }
        public string OldValues { get; set; } 
        public string NewValues { get; set; } 
        public string Changes { get; set; } // Opcional: Solo las diferencias entre OldValues y NewValues (JSON string) - puede ser complejo de implementar automáticamente al principio.
        public string OperationName { get; set; }
        public string OperationStatus { get; set; } 
        public string Details { get; set; } 
    }
}
