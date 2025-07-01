using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Destino
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Desnom { get; set; }

        [StringLength(20)]
        public string Descod { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}