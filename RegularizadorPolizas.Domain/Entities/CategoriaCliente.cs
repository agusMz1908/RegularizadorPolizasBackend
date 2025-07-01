using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class CategoriaCliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal ValMin { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal ValMax { get; set; }

        [StringLength(20)]
        public string Color { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        public virtual ICollection<Client> Clientes { get; set; } = new List<Client>();
    }
}