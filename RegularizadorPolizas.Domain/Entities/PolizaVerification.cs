using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    [Table("PolizaVerifications")]
    public class PolizaVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PolizaId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime FechaVerificacion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string EstadoGeneral { get; set; } // pendiente, verificado, requiere_correccion

        [Column(TypeName = "TEXT")]
        public string CamposVerificados { get; set; } 

        [StringLength(1000)]
        public string Observaciones { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}