using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(5)]
        public string Simbolo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Required]
        [StringLength(10)]
        public string Moneda { get; set; } = string.Empty;

        public DateTime? FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
    }
}