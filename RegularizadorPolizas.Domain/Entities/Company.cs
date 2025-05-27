using System.ComponentModel.DataAnnotations;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Comnom { get; set; }
        [StringLength(150)]
        public string Comrazsoc { get; set; }
        [StringLength(20)]
        public string Comruc { get; set; }
        [StringLength(255)]
        public string Comdom { get; set; }
        [StringLength(50)]
        public string Comtel { get; set; }
        [StringLength(50)]
        public string Comfax { get; set; }
        [StringLength(100)]
        public string Comsumodia { get; set; }
        public int? Comcntcli { get; set; }
        public int? Comcntcon { get; set; }
        public decimal? Comprepes { get; set; }
        public decimal? Compredol { get; set; }
        public decimal? Comcomipe { get; set; }
        public decimal? Comcomido { get; set; }
        public decimal? Comtotcomi { get; set; }
        public decimal? Comtotpre { get; set; }
        [StringLength(50)]
        public string Comalias { get; set; }
        [StringLength(255)]
        public string Comlog { get; set; }
        public bool Broker { get; set; }
        [StringLength(50)]
        public string Cod_srvcompanias { get; set; }
        [StringLength(100)]
        public string No_utiles { get; set; }
        public int? Paq_dias { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        public virtual ICollection<Poliza> Polizas { get; set; }
    }
}