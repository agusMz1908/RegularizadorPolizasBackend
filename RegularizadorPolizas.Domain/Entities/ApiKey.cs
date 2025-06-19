using System;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ApiKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaExpiracion { get; set; }
        public string? Descripcion { get; set; }
    }
}
