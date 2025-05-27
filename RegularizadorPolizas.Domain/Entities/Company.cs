using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Alias { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
    }
}