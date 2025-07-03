

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        // Propiedades básicas
        public int? Corrcod { get; set; }
        public int? Subcorr { get; set; }

        [Required]
        [StringLength(150)]
        public string Clinom { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(50)]
        public string Clitelcel { get; set; }

        public DateTime? Clifchnac { get; set; }

        public DateTime? Clifching { get; set; }

        public DateTime? Clifchegr { get; set; }

        [StringLength(100)]
        public string Clicargo { get; set; }

        [StringLength(100)]
        public string Clicon { get; set; }

        [StringLength(20)]
        public string Cliruc { get; set; }

        [StringLength(150)]
        public string Clirsoc { get; set; }

        [StringLength(20)]
        public string Cliced { get; set; }

        [StringLength(50)]
        public string Clilib { get; set; }

        [StringLength(50)]
        public string Clicatlib { get; set; }

        [StringLength(50)]
        public string Clitpo { get; set; }

        [StringLength(255)]
        public string Clidir { get; set; }

        [StringLength(100)]
        public string Cliemail { get; set; }

        public DateTime? Clivtoced { get; set; }

        public DateTime? Clivtolib { get; set; }

        public int? Cliposcod { get; set; }

        [StringLength(50)]
        public string Clitelcorr { get; set; }

        [StringLength(100)]
        public string Clidptnom { get; set; }

        [StringLength(10)]
        public string Clisex { get; set; }

        [StringLength(50)]
        public string Clitelant { get; set; }

        public string Cliobse { get; set; }

        [StringLength(50)]
        public string Clifax { get; set; }

        [StringLength(50)]
        public string Cliclasif { get; set; }

        [StringLength(50)]
        public string Clinumrel { get; set; }

        [StringLength(50)]
        public string Clicasapt { get; set; }

        [StringLength(100)]
        public string Clidircob { get; set; }

        public int? Clibse { get; set; }

        public byte[] Clifoto { get; set; }

        public int? Pruebamillares { get; set; }

        [StringLength(50)]
        public string Ingresado { get; set; }

        [StringLength(50)]
        public string Clialias { get; set; }

        [StringLength(20)]
        public string Clipor { get; set; }

        [StringLength(20)]
        public string Clisancor { get; set; }

        [StringLength(20)]
        public string Clirsa { get; set; }

        public int? Codposcob { get; set; }

        [StringLength(50)]
        public string Clidptcob { get; set; }

        public bool Activo { get; set; } = true;

        [StringLength(20)]
        public string Cli_s_cris { get; set; }

        public DateTime? Clifchnac1 { get; set; }

        [StringLength(50)]
        public string Clilocnom { get; set; }

        [StringLength(50)]
        public string Cliloccob { get; set; }

        public int? Categorias_de_cliente { get; set; }

        [StringLength(50)]
        public string Sc_departamentos { get; set; }

        [StringLength(50)]
        public string Sc_localidades { get; set; }

        public DateTime? Fch_ingreso { get; set; }

        public int? Grupos_economicos { get; set; }

        public bool Etiquetas { get; set; }

        public bool Doc_digi { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        public bool Habilita_app { get; set; }

        [StringLength(100)]
        public string Referido { get; set; }

        public int? Altura { get; set; }

        public int? Peso { get; set; }

        [StringLength(50)]
        public string Cliberkley { get; set; }

        [StringLength(50)]
        public string Clifar { get; set; }

        [StringLength(50)]
        public string Clisurco { get; set; }

        [StringLength(50)]
        public string Clihdi { get; set; }

        [StringLength(50)]
        public string Climapfre { get; set; }

        [StringLength(50)]
        public string Climetlife { get; set; }

        [StringLength(50)]
        public string Clisancris { get; set; }

        [StringLength(50)]
        public string Clisbi { get; set; }

        [StringLength(50)]
        public string Edo_civil { get; set; }

        public bool Not_bien_mail { get; set; }

        public bool Not_bien_wap { get; set; }

        public bool Ing_poliza_mail { get; set; }

        public bool Ing_poliza_wap { get; set; }

        public bool Ing_siniestro_mail { get; set; }

        public bool Ing_siniestro_wap { get; set; }

        public bool Noti_obs_sini_mail { get; set; }

        public bool Noti_obs_sini_wap { get; set; }

        public DateTime? Last_update { get; set; }

        public int? App_id { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual ICollection<AutorizaCliente> AutorizacionesCliente { get; set; } = new List<AutorizaCliente>();
        public virtual ICollection<CuentaBancaria> CuentasBancarias { get; set; } = new List<CuentaBancaria>();
        public virtual ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
        public virtual ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();
        public virtual ICollection<Poliza> Polizas { get; set; }

        [ForeignKey("Categorias_de_cliente")]
        public virtual CategoriaCliente CategoriaCliente { get; set; }

        [ForeignKey("Grupos_economicos")]
        public virtual GrupoEconomico GrupoEconomico { get; set; }
    }
}