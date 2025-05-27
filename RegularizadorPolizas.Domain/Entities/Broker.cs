using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Broker
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(255)]
        public string Domicilio { get; set; }

        [Required]
        [StringLength(50)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
    }
}