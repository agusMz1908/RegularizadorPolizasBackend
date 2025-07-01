using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Taller
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Contacto { get; set; }

        [StringLength(30)]
        public string Cel { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public bool CTMA { get; set; }
        public bool Disponible24h { get; set; }
        public bool Tarjeta { get; set; }

        [StringLength(100)]
        public string Web { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
    }
}