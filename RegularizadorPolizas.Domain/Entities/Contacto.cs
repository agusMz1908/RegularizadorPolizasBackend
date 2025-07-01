using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Contacto
    {
        [Key]
        public int Id { get; set; }
        public int Clientes { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string CargoRelacion { get; set; }

        [StringLength(30)]
        public string Cel { get; set; }

        [StringLength(200)]
        public string Domicilio { get; set; }

        [StringLength(100)]
        public string Mail { get; set; }

        [StringLength(500)]
        public string Obs { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("Clientes")]
        public virtual Client Cliente { get; set; }
    }
}