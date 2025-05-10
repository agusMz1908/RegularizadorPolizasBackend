using RegularizadorPolizasBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class Poliza
    {
        [Key]
        public int Id { get; set; }

        // Propiedades básicas de identificación
        public int? Clinro { get; set; }
        public int? Comcod { get; set; }
        public int? Seccod { get; set; }
        [StringLength(255)]
        public string Condom { get; set; }

        // Datos del vehículo
        [StringLength(100)]
        public string Conmaraut { get; set; }
        public int? Conanioaut { get; set; }
        public int? Concodrev { get; set; }
        [StringLength(20)]
        public string Conmataut { get; set; }
        public int? Conficto { get; set; }
        [StringLength(50)]
        public string Conmotor { get; set; }
        [StringLength(50)]
        public string Conpadaut { get; set; }
        [StringLength(50)]
        public string Conchasis { get; set; }
        public int? Conclaaut { get; set; }
        public int? Condedaut { get; set; }
        public int? Conresciv { get; set; }

        // Bonificaciones y descuentos
        public int? Conbonnsin { get; set; }
        public int? Conbonant { get; set; }
        public int? Concaraut { get; set; }

        // Datos del cesionario
        [StringLength(100)]
        public string Concesnom { get; set; }
        [StringLength(100)]
        public string Concestel { get; set; }
        public int? Concapaut { get; set; }

        // Datos financieros
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conpremio { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Contot { get; set; }
        public int? Moncod { get; set; }
        public int? Concuo { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Concomcorr { get; set; }

        // Categorización
        public int? Catdsc { get; set; }
        public int? Desdsc { get; set; }
        public int? Caldsc { get; set; }
        public int? Flocod { get; set; }

        // Datos de la póliza
        [StringLength(50)]
        public string Concar { get; set; }
        [StringLength(50)]
        public string Conpol { get; set; }
        [StringLength(20)]
        public string Conend { get; set; }
        public DateTime? Confchdes { get; set; }
        public DateTime? Confchhas { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conimp { get; set; }
        public int? Connroser { get; set; }
        [StringLength(50)]
        public string Rieres { get; set; }

        // Gestión
        [StringLength(100)]
        public string Conges { get; set; }
        [StringLength(50)]
        public string Congesti { get; set; }
        public DateTime? Congesfi { get; set; }
        [StringLength(50)]
        public string Congeses { get; set; }
        [StringLength(20)]
        public string Convig { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concan { get; set; }
        [StringLength(50)]
        public string Congrucon { get; set; }

        // Relaciones entre pólizas
        public int? Conpadre { get; set; }
        public int? Conidpad { get; set; }
        public DateTime? Confchcan { get; set; }
        [StringLength(100)]
        public string Concaucan { get; set; }

        // Campos adicionales
        [StringLength(100)]
        public string Contipoemp { get; set; }
        [StringLength(100)]
        public string Conmatpar { get; set; }
        [StringLength(100)]
        public string Conmatte { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concapla { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflota { get; set; }
        public int? Condednum { get; set; }
        [StringLength(50)]
        public string Consta { get; set; }
        [StringLength(50)]
        public string Contra { get; set; }
        [StringLength(50)]
        public string Conconf { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conobjtot { get; set; }

        // Más campos adicionales
        [StringLength(100)]
        public string Contpoact { get; set; }
        [StringLength(50)]
        public string Conesp { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Convalacr { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Convallet { get; set; }
        [StringLength(100)]
        public string Condecram { get; set; }
        [StringLength(100)]
        public string Conmedtra { get; set; }
        [StringLength(100)]
        public string Conviades { get; set; }
        [StringLength(100)]
        public string Conviaa { get; set; }
        [StringLength(100)]
        public string Conviaenb { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviakb { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviakn { get; set; }
        [StringLength(50)]
        public string Conviatra { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviacos { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviafle { get; set; }

        // Ubicación y departamento
        public int? Dptnom { get; set; }
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
        [StringLength(255)]
        public string Conubi { get; set; }
        [StringLength(255)]
        public string Concaudsc { get; set; }
        [StringLength(100)]
        public string Conincuno { get; set; }

        // Datos de viaje
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviagas { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviarec { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviapri { get; set; }

        // Campos adicionales de líneas y observaciones
        public int? Linobs { get; set; }
        public DateTime? Concomdes { get; set; }
        [StringLength(50)]
        public string Concalcom { get; set; }

        // Tipos y modalidades
        public int? Tpoconcod { get; set; }
        public int? Tpovivcod { get; set; }
        public int? Tporiecod { get; set; }
        public int? Modcod { get; set; }

        // Capitales y primas
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concapase { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conpricap { get; set; }
        [StringLength(100)]
        public string Tposegdsc { get; set; }
        public int? Conriecod { get; set; }
        [StringLength(100)]
        public string Conriedsc { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conrecfin { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conimprf { get; set; }
        public int? Conafesin { get; set; }
        public int? Conautcor { get; set; }
        public int? Conlinrie { get; set; }
        public int? Conconesp { get; set; }

        // Campos adicionales
        [StringLength(100)]
        public string Conlimnav { get; set; }
        [StringLength(100)]
        public string Contpocob { get; set; }
        [StringLength(50)]
        public string Connomemb { get; set; }
        [StringLength(50)]
        public string Contpoemb { get; set; }
        public int? Lincarta { get; set; }
        public int? Cancecod { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concomotr { get; set; }
        [StringLength(50)]
        public string Conautcome { get; set; }
        [StringLength(50)]
        public string Conviafac { get; set; }
        public int? Conviamon { get; set; }
        [StringLength(50)]
        public string Conviatpo { get; set; }
        [StringLength(50)]
        public string Connrorc { get; set; }
        [StringLength(50)]
        public string Condedurc { get; set; }
        public int? Lininclu { get; set; }
        public int? Linexclu { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Concapret { get; set; }
        [StringLength(50)]
        public string Forpagvid { get; set; }

        // Datos del cliente y corredor
        [StringLength(150)]
        public string Clinom { get; set; }
        public int? Tarcod { get; set; }
        public int? Corrnom { get; set; }
        public int? Connroint { get; set; }
        [StringLength(50)]
        public string Conautnd { get; set; }
        public int? Conpadend { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Contotpri { get; set; }
        public int? Padreaux { get; set; }

        // Datos de flota
        public int? Conlinflot { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflotimp { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflottotal { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conflotsaldo { get; set; }

        // Datos de certificación
        [StringLength(50)]
        public string Conaccicer { get; set; }
        public DateTime? Concerfin { get; set; }

        // Datos de embarcación
        [StringLength(100)]
        public string Condetemb { get; set; }
        [StringLength(50)]
        public string Conclaemb { get; set; }
        [StringLength(50)]
        public string Confabemb { get; set; }
        [StringLength(50)]
        public string Conbanemb { get; set; }
        [StringLength(50)]
        public string Conmatemb { get; set; }
        [StringLength(50)]
        public string Convelemb { get; set; }
        [StringLength(50)]
        public string Conmatriemb { get; set; }
        [StringLength(50)]
        public string Conptoemb { get; set; }

        // Datos de corredores y subcorredores
        public int? Otrcorrcod { get; set; }
        [StringLength(100)]
        public string Condeta { get; set; }
        public string Observaciones { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Clipcupfia { get; set; }
        [StringLength(50)]
        public string Conclieda { get; set; }
        [StringLength(100)]
        public string Condecrea { get; set; }
        [StringLength(100)]
        public string Condecaju { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Conviatot { get; set; }
        [StringLength(100)]
        public string Contpoemp { get; set; }
        [StringLength(100)]
        public string Congaran { get; set; }
        [StringLength(50)]
        public string Congarantel { get; set; }
        [StringLength(50)]
        public string Mot_no_ren { get; set; }
        [StringLength(100)]
        public string Condetrc { get; set; }
        public bool Conautcort { get; set; }
        [StringLength(255)]
        public string Condetail { get; set; }
        public int? Clinro1 { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Consumsal { get; set; }
        [StringLength(100)]
        public string Conespbon { get; set; }

        // Estados de la póliza
        public bool Leer { get; set; }
        public bool Enviado { get; set; }
        public bool Sob_recib { get; set; }
        public bool Leer_obs { get; set; }
        public string Sublistas { get; set; }
        public int? Com_sub_corr { get; set; }
        public int? Tipos_de_alarma { get; set; }
        public bool Tiene_alarma { get; set; }
        public int? Coberturas_bicicleta { get; set; }
        public int? Com_bro { get; set; }
        public int? Com_bo { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Contotant { get; set; }
        public string Cotizacion { get; set; }
        public int? Motivos_no_renovacion { get; set; }
        [StringLength(50)]
        public string Com_alias { get; set; }
        [StringLength(50)]
        public string Ramo { get; set; }
        [StringLength(50)]
        public string Clausula { get; set; }

        // Propiedades de transporte
        public bool Aereo { get; set; }
        public bool Maritimo { get; set; }
        public bool Terrestre { get; set; }
        public int? Max_aereo { get; set; }
        public int? Max_mar { get; set; }
        public int? Max_terrestre { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Tasa { get; set; }
        [StringLength(50)]
        public string Facturacion { get; set; }

        // Propiedades de tipo de operación
        public bool Importacion { get; set; }
        public bool Exportacion { get; set; }
        public bool Offshore { get; set; }
        public bool Transito_interno { get; set; }
        [StringLength(100)]
        public string Coning { get; set; }
        public int? Cat_cli { get; set; }
        public bool Llamar { get; set; }
        public bool Granizo { get; set; }
        [StringLength(50)]
        public string Idorden { get; set; }
        public bool Var_ubi { get; set; }
        public bool Mis_rie { get; set; }

        // Fechas y actualizaciones
        public DateTime? Ingresado { get; set; }
        public DateTime? Last_update { get; set; }
        public int? Comcod1 { get; set; }
        public int? Comcod2 { get; set; }
        public int? Pagos_efectivo { get; set; }
        public int? Productos_de_vida { get; set; }
        public int? App_id { get; set; }
        public DateTime? Update_date { get; set; }
        [StringLength(100)]
        public string Gestion { get; set; }
        public int? Asignado { get; set; }
        [StringLength(50)]
        public string Combustibles { get; set; }

        // Campos propios de la aplicación
        [StringLength(255)]
        public string DocumentoPdf { get; set; }
        public bool Procesado { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("Clinro")]
        public virtual Client Client { get; set; }

        [ForeignKey("Conpadre")]
        public virtual Poliza PolizaPadre { get; set; }
        public virtual ICollection<Poliza> PolizasHijas { get; set; }

        public virtual ICollection<ProcessDocument> ProcessDocuments { get; set; }
        public virtual ICollection<Renovation> RenovacionesOrigen { get; set; }
        public virtual ICollection<Renovation> RenovacionesDestino { get; set; }
    }
}