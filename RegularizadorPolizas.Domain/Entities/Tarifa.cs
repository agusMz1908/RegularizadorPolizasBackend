using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Tarifa
    {
        [Key]
        public int Id { get; set; }

        public int Companias { get; set; } 

        [Required]
        [StringLength(100)]
        public string Tarnom { get; set; } 

        [StringLength(50)]
        public string Tarcod { get; set; }

        [StringLength(200)]
        public string Tardsc { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("Companias")]
        public virtual Company Company { get; set; }
    }
}