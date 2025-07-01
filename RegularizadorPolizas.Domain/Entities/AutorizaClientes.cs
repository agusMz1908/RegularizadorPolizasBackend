using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class AutorizaCliente
    {
        [Key]
        public int Id { get; set; }
        public int Clientes { get; set; }
        public int AutorizacionesDeDatos { get; set; }

        public bool Autorizado { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("Clientes")]
        public virtual Client Cliente { get; set; }

        [ForeignKey("AutorizacionesDeDatos")]
        public virtual AutorizacionDatos AutorizacionDatos { get; set; }
    }
}