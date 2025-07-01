using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ComisionPorSeccion
    {
        [Key]
        public int Id { get; set; }

        public int Compania { get; set; } 

        public int Seccion { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Comipor { get; set; } 

        [Column(TypeName = "decimal(5,2)")]
        public decimal Comi_bo { get; set; } = 0; 

        [StringLength(10)]
        public string Opera_comi { get; set; } = "N"; 

        [StringLength(200)]
        public string Detalle { get; set; } 

        public int Tipos_de_contrato { get; set; } = 0;

        public int Productos_de_vida { get; set; } = 0;

        public int Anios_d { get; set; } = 0; // Años desde

        public int Anios_h { get; set; } = 0; // Años hasta

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("Compania")]
        public virtual Company Company { get; set; }

        [ForeignKey("Seccion")]
        public virtual Seccion SeccionEntity { get; set; }
    }
}
