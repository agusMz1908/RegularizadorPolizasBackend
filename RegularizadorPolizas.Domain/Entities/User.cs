using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public required string TenantId { get; set; }

        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual ICollection<ProcessDocument> ProcessDocuments { get; set; }
        public virtual ICollection<Renovation> Renovations { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}