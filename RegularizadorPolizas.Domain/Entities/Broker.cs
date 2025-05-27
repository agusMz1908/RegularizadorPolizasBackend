using System.ComponentModel.DataAnnotations;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Broker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Telefono { get; set; }
        [StringLength(255)]
        public string Direccion { get; set; }
        public string Observaciones { get; set; }
        public byte[] Foto { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        public virtual ICollection<Poliza> Polizas { get; set; }
    }
}