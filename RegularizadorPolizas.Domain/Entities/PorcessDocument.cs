using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ProcessDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string NombreArchivo { get; set; }

        [Required]
        [StringLength(255)]
        public string RutaArchivo { get; set; }

        [StringLength(50)]
        public string TipoDocumento { get; set; }

        [StringLength(50)]
        public string EstadoProcesamiento { get; set; } = "PENDIENTE";

        public string ResultadoJson { get; set; }

        public int? PolizaId { get; set; }

        public DateTime? FechaProcesamiento { get; set; }

        public int? UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("PolizaId")]
        public virtual Poliza Poliza { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual User User { get; set; }
    }
}