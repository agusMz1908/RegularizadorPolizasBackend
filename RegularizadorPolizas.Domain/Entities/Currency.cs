using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(10)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(5)]
        public string Simbolo { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
    }
}