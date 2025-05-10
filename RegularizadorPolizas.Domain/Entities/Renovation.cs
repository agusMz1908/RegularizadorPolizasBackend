using RegularizadorPolizas.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Renovation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PolizaId { get; set; }

        public int? PolizaNuevaId { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Estado { get; set; } = "PENDIENTE";

        public string Observaciones { get; set; }

        public int? UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("PolizaId")]
        public virtual Poliza PolizaOriginal { get; set; }

        [ForeignKey("PolizaNuevaId")]
        public virtual Poliza PolizaNueva { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual User User { get; set; }
    }
}