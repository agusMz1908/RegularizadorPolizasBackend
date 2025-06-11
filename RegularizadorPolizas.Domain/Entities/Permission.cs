using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Resource { get; set; } = string.Empty; // "Clients", "Polizas", "Documents", etc.

        [Required]
        [StringLength(20)]
        public string Action { get; set; } = string.Empty; // "Read", "Create", "Update", "Delete"

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}