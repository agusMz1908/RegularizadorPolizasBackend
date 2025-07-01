using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class TipoRiesgo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TpoRieDssc { get; set; }

        [StringLength(500)]
        public string RieDesc { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}