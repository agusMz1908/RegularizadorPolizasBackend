using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Poliza
    {
        [Key]
        public int Id { get; set; }
        public int? Clinro { get; set; }
        public int? Comcod { get; set; }
        public int? Seccod { get; set; }
        public string Condom { get; set; }

        [StringLength(20)]
        public string Conmaraut { get; set; }

        public int? Conanioaut { get; set; }
        public int? Concodrev { get; set; }

        [StringLength(30)]
        public string Conmataut { get; set; }

        public int? Conficto { get; set; }

        [StringLength(30)]
        public string Conmotor { get; set; }

        [StringLength(30)]
        public string Conpadaut { get; set; }

        [StringLength(30)]
        public string Conchasis { get; set; }
        public int? Conclaaut { get; set; }
        public int? Condedaut { get; set; }
        public int? Conresciv { get; set; }
        public int? Conbonnsin { get; set; }
        public int? Conbonant { get; set; }
        public int? Concaraut { get; set; }

        [StringLength(40)]
        public string Concesnom { get; set; } // Nombre cesionario

        [StringLength(20)]
        public string Concestel { get; set; } // Teléfono cesionario

        public int? Concapaut { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conpremio { get; set; } // Premio

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Contot { get; set; } // Total

        public int? Moncod { get; set; } // Moneda
        public int? Concuo { get; set; } // Cuotas

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Concomcorr { get; set; } // Comisión corredor
        public int? Catdsc { get; set; }
        public int? Desdsc { get; set; }
        public int? Caldsc { get; set; }
        public int? Flocod { get; set; }
        [StringLength(30)]
        public string Concar { get; set; }

        [StringLength(40)]
        public string Conpol { get; set; } // Número de póliza

        [StringLength(3)]
        public string Conend { get; set; } // Endoso

        public DateTime? Confchdes { get; set; } // Fecha desde
        public DateTime? Confchhas { get; set; } // Fecha hasta

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conimp { get; set; } // Impuesto

        public int? Connroser { get; set; }

        [StringLength(30)]
        public string Rieres { get; set; }

        [StringLength(80)]
        public string Conges { get; set; }

        [StringLength(30)]
        public string Congesti { get; set; }

        public DateTime? Congesfi { get; set; }

        [StringLength(30)]
        public string Congeses { get; set; }

        [StringLength(15)]
        public string Convig { get; set; } // Vigencia

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concan { get; set; }

        [StringLength(30)]
        public string Congrucon { get; set; }

        public int? Conpadre { get; set; } // Póliza padre
        public int? Conidpad { get; set; }
        public DateTime? Confchcan { get; set; } // Fecha cancelación

        [StringLength(80)]
        public string Concaucan { get; set; } // Causa cancelación

        [StringLength(80)]
        public string Contipoemp { get; set; }

        [StringLength(80)]
        public string Conmatpar { get; set; }

        [StringLength(80)]
        public string Conmatte { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concapla { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflota { get; set; }

        public int? Condednum { get; set; }

        [StringLength(30)]
        public string Consta { get; set; } // Forma de pago

        [StringLength(30)]
        public string Contra { get; set; } // Trámite

        [StringLength(30)]
        public string Conconf { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conobjtot { get; set; }

        [StringLength(80)]
        public string Contpoact { get; set; }

        [StringLength(30)]
        public string Conesp { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Convalacr { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Convallet { get; set; }

        [StringLength(80)]
        public string Condecram { get; set; }

        [StringLength(80)]
        public string Conmedtra { get; set; } // Medio de transporte

        [StringLength(80)]
        public string Conviades { get; set; }

        [StringLength(80)]
        public string Conviaa { get; set; }

        [StringLength(80)]
        public string Conviaenb { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviakb { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviakn { get; set; }

        [StringLength(30)]
        public string Conviatra { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviacos { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviafle { get; set; }

        public int? Dptnom { get; set; } // Departamento

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conedaret { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Congar { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Condecpri { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Condecpro { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Condecptj { get; set; }

        [StringLength(200)]
        public string Conubi { get; set; } // Ubicación

        [StringLength(200)]
        public string Concaudsc { get; set; }

        [StringLength(80)]
        public string Conincuno { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviagas { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviarec { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviapri { get; set; }

        public int? Linobs { get; set; }
        public DateTime? Concomdes { get; set; }

        [StringLength(30)]
        public string Concalcom { get; set; }

        public int? Tpoconcod { get; set; }
        public int? Tpovivcod { get; set; }
        public int? Tporiecod { get; set; }
        public int? Modcod { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concapase { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conpricap { get; set; }

        [StringLength(80)]
        public string Tposegdsc { get; set; }

        public int? Conriecod { get; set; }

        [StringLength(80)]
        public string Conriedsc { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conrecfin { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conimprf { get; set; }

        public int? Conafesin { get; set; }
        public int? Conautcor { get; set; }
        public int? Conlinrie { get; set; }
        public int? Conconesp { get; set; }

        [StringLength(80)]
        public string Conlimnav { get; set; }

        [StringLength(80)]
        public string Contpocob { get; set; } // Tipo de cobertura

        [StringLength(40)]
        public string Connomemb { get; set; }

        [StringLength(30)]
        public string Contpoemb { get; set; }

        public int? Lincarta { get; set; }
        public int? Cancecod { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concomotr { get; set; }

        [StringLength(30)]
        public string Conautcome { get; set; }

        [StringLength(30)]
        public string Conviafac { get; set; }

        public int? Conviamon { get; set; }

        [StringLength(30)]
        public string Conviatpo { get; set; }

        [StringLength(30)]
        public string Connrorc { get; set; }

        [StringLength(30)]
        public string Condedurc { get; set; }

        public int? Lininclu { get; set; }
        public int? Linexclu { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concapret { get; set; }

        [StringLength(30)]
        public string Forpagvid { get; set; }

        [StringLength(120)]
        public string Clinom { get; set; } // Nombre cliente

        public int? Tarcod { get; set; }
        public int? Corrnom { get; set; } // Corredor
        public int? Connroint { get; set; }

        [StringLength(30)]
        public string Conautnd { get; set; }

        public int? Conpadend { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Contotpri { get; set; }

        public int? Padreaux { get; set; }

        public int? Conlinflot { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflotimp { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflottotal { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflotsaldo { get; set; }

        [StringLength(30)]
        public string Conaccicer { get; set; }

        public DateTime? Concerfin { get; set; }

        [StringLength(80)]
        public string Condetemb { get; set; }

        [StringLength(30)]
        public string Conclaemb { get; set; }

        [StringLength(30)]
        public string Confabemb { get; set; }

        [StringLength(30)]
        public string Conbanemb { get; set; }

        [StringLength(30)]
        public string Conmatemb { get; set; }

        [StringLength(30)]
        public string Convelemb { get; set; }

        [StringLength(30)]
        public string Conmatriemb { get; set; }

        [StringLength(30)]
        public string Conptoemb { get; set; }
        public int? Otrcorrcod { get; set; }

        [StringLength(80)]
        public string Condeta { get; set; }

        public string Observaciones { get; set; } // TEXT

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Clipcupfia { get; set; }

        [StringLength(30)]
        public string Conclieda { get; set; }

        [StringLength(80)]
        public string Condecrea { get; set; }

        [StringLength(80)]
        public string Condecaju { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviatot { get; set; }

        [StringLength(80)]
        public string Contpoemp { get; set; }

        [StringLength(80)]
        public string Congaran { get; set; }

        [StringLength(20)]
        public string Congarantel { get; set; }

        [StringLength(30)]
        public string Mot_no_ren { get; set; }

        [StringLength(80)]
        public string Condetrc { get; set; }

        public bool Conautcort { get; set; }

        [StringLength(200)]
        public string Condetail { get; set; }

        public int? Clinro1 { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Consumsal { get; set; }

        [StringLength(80)]
        public string Conespbon { get; set; }
        public bool Leer { get; set; }
        public bool Enviado { get; set; }
        public bool Sob_recib { get; set; }
        public bool Leer_obs { get; set; }
        public string Sublistas { get; set; } // TEXT

        public int? Com_sub_corr { get; set; }
        public int? Tipos_de_alarma { get; set; }
        public bool Tiene_alarma { get; set; }
        public int? Coberturas_bicicleta { get; set; }
        public int? Com_bro { get; set; }
        public int? Com_bo { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Contotant { get; set; }
        public string Cotizacion { get; set; } // TEXT

        public int? Motivos_no_renovacion { get; set; }

        [StringLength(40)]
        public string Com_alias { get; set; }

        [StringLength(30)]
        public string Ramo { get; set; }

        [StringLength(30)]
        public string Clausula { get; set; }

        public bool Aereo { get; set; }
        public bool Maritimo { get; set; }
        public bool Terrestre { get; set; }
        public int? Max_aereo { get; set; }
        public int? Max_mar { get; set; }
        public int? Max_terrestre { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Tasa { get; set; }

        [StringLength(30)]
        public string Facturacion { get; set; }
        public bool Importacion { get; set; }
        public bool Exportacion { get; set; }
        public bool Offshore { get; set; }
        public bool Transito_interno { get; set; }

        [StringLength(80)]
        public string Coning { get; set; }

        public int? Cat_cli { get; set; }
        public bool Llamar { get; set; }
        public bool Granizo { get; set; }

        [StringLength(30)]
        public string Idorden { get; set; }

        public bool Var_ubi { get; set; }
        public bool Mis_rie { get; set; }
        public DateTime? Ingresado { get; set; }
        public DateTime? Last_update { get; set; }
        public int? Comcod1 { get; set; }
        public int? Comcod2 { get; set; }
        public int? Pagos_efectivo { get; set; }
        public int? Productos_de_vida { get; set; }
        public int? App_id { get; set; }
        public DateTime? Update_date { get; set; }

        [StringLength(80)]
        public string Gestion { get; set; }

        public int? Asignado { get; set; }

        [StringLength(30)]
        public string Combustibles { get; set; }

        [StringLength(250)]
        public string DocumentoPdf { get; set; }

        public bool Procesado { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        [ForeignKey("Clinro")]
        public virtual Client Client { get; set; }

        [ForeignKey("Comcod")]
        public virtual Company Company { get; set; }

        [ForeignKey("Corrnom")]
        public virtual Broker Broker { get; set; }

        [ForeignKey("Moncod")]
        public virtual Currency Currency { get; set; }

        [ForeignKey("Conpadre")]
        public virtual Poliza PolizaPadre { get; set; }
        public virtual ICollection<Poliza> PolizasHijas { get; set; } = new List<Poliza>();
        public virtual ICollection<ProcessDocument> ProcessDocuments { get; set; } = new List<ProcessDocument>();
        public virtual ICollection<Renovation> RenovacionesOrigen { get; set; } = new List<Renovation>();
        public virtual ICollection<Renovation> RenovacionesDestino { get; set; } = new List<Renovation>();
    }
}