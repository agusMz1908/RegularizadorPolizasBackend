using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ImpuestoPorSeccion
    {
        [Key]
        public int Id { get; set; }

        public int Companias { get; set; } 

        public int Impuestos { get; set; } 

        public int Secciones { get; set; } 

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("Companias")]
        public virtual Company Company { get; set; }

        [ForeignKey("Secciones")]
        public virtual Seccion Seccion { get; set; }
    }
}