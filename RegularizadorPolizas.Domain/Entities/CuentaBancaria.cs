using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class CuentaBancaria
    {
        [Key]
        public int Id { get; set; }
        public int Clientes { get; set; }

        [Required]
        [StringLength(100)]
        public string Titular { get; set; }

        [StringLength(10)]
        public string Tipo { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } // Banco

        [StringLength(5)]
        public string MonedaCuenta { get; set; }

        [StringLength(10)]
        public string Sucursal { get; set; }

        [Required]
        [StringLength(50)]
        public string Numero { get; set; }

        [StringLength(20)]
        public string Subcuenta { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("Clientes")]
        public virtual Client Cliente { get; set; }
    }
}