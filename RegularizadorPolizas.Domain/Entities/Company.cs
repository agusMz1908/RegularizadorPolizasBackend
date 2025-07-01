using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string Alias { get; set; } = string.Empty;

        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Required]
        [StringLength(100)]
        public string Comnom { get; set; } = string.Empty;

        [StringLength(150)]
        public string Comrazsoc { get; set; } = string.Empty;

        [StringLength(20)]
        public string Comruc { get; set; } = string.Empty;

        [StringLength(255)]
        public string Comdom { get; set; } = string.Empty;

        [StringLength(50)]
        public string Comtel { get; set; } = string.Empty;

        [StringLength(50)]
        public string Comfax { get; set; } = string.Empty;

        [StringLength(100)]
        public string Comsumodia { get; set; } = string.Empty;

        public int Comcntcli { get; set; }
        public int Comcntcon { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Comprepes { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Compredol { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Comcomipe { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Comcomido { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Comtotcomi { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Comtotpre { get; set; }

        [StringLength(50)]
        public string Comalias { get; set; } = string.Empty;

        [StringLength(255)]
        public string Comlog { get; set; } = string.Empty;

        public bool Broker { get; set; }

        [StringLength(50)]
        public string Cod_srvcompanias { get; set; } = string.Empty;

        [StringLength(100)]
        public string No_utiles { get; set; } = string.Empty;

        public int Paq_dias { get; set; }

        public DateTime? FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; } = DateTime.Now;

        public virtual ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
    }
}