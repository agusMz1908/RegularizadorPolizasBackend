using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Tarjeta
    {
        [Key]
        public int Id { get; set; }
        public int Clientes { get; set; }

        [Required]
        [StringLength(50)]
        public string Emisor { get; set; }

        [Required]
        [StringLength(100)]
        public string Titular { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Nombre de la tarjeta

        [Required]
        public DateTime Vencimiento { get; set; }

        [Required]
        [StringLength(50)]
        public string Numero { get; set; }

        public int Dato { get; set; }
        public int Con { get; set; }

        [StringLength(10)]
        public string Control { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relación
        [ForeignKey("Clientes")]
        public virtual Client Cliente { get; set; }
    }
}