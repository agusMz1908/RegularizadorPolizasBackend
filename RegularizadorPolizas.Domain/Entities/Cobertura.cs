using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Cobertura
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Cobdsc { get; set; } 

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}