using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class TipoContrato
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TpoConDssc { get; set; }

        public int Secciones { get; set; }

        [StringLength(500)]
        public string TpoDet { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}