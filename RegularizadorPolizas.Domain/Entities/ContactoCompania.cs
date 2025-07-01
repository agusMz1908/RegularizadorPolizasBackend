using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ContactoCompania
    {
        [Key]
        public int Id { get; set; }

        public int Companias { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string Mail { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("Companias")]
        public virtual Company Company { get; set; }
    }
}