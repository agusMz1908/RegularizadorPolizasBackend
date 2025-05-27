using System.ComponentModel.DataAnnotations;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Moneda { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public virtual ICollection<Poliza> Polizas { get; set; }
    }
}