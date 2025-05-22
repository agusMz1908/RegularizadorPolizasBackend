namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaDto
    {
        // Propiedades básicas
        public int Id { get; set; }
        public int? Comcod { get; set; }
        public int? Seccod { get; set; }
        public int? Clinro { get; set; }
        public string Condom { get; set; }

        // Propiedades del vehículo
        public string Conmaraut { get; set; }
        public int? Conanioaut { get; set; }
        public int? Concodrev { get; set; }
        public string Conmataut { get; set; }
        public int? Conficto { get; set; }
        public string Conmotor { get; set; }
        public string Conpadaut { get; set; }
        public string Conchasis { get; set; }

        // Propiedades de cobertura
        public int? Conclaaut { get; set; }
        public int? Condedaut { get; set; }
        public int? Conresciv { get; set; }
        public int? Conbonnsin { get; set; }
        public int? Conbonant { get; set; }
        public int? Concaraut { get; set; }

        // Propiedades del cesionario
        public string Concesnom { get; set; }
        public string Concestel { get; set; }
        public int? Concapaut { get; set; }

        // Propiedades financieras
        public decimal? Conpremio { get; set; }
        public decimal? Contot { get; set; }
        public int? Moncod { get; set; }
        public int? Concuo { get; set; }
        public decimal? Concomcorr { get; set; }

        // Propiedades de categorización
        public int? Catdsc { get; set; }
        public int? Desdsc { get; set; }
        public int? Caldsc { get; set; }
        public int? Flocod { get; set; }

        // Propiedades de la póliza
        public string Concar { get; set; }
        public string Conpol { get; set; }
        public string Conend { get; set; }
        public DateTime? Confchdes { get; set; }
        public DateTime? Confchhas { get; set; }
        public decimal? Conimp { get; set; }
        public int? Connroser { get; set; }
        public string Rieres { get; set; }

        // Propiedades de gestión
        public string Conges { get; set; }
        public string Congesti { get; set; }
        public DateTime? Congesfi { get; set; }
        public string Congeses { get; set; }
        public string Convig { get; set; }
        public decimal? Concan { get; set; }
        public string Congrucon { get; set; }

        // Propiedades adicionales
        public string Contipoemp { get; set; }
        public string Conmatpar { get; set; }
        public string Conmatte { get; set; }
        public decimal? Concapla { get; set; }
        public decimal? Conflota { get; set; }
        public int? Condednum { get; set; }
        public string Consta { get; set; }
        public string Contra { get; set; }
        public string Conconf { get; set; }
        public int? Conpadre { get; set; }
        public DateTime? Confchcan { get; set; }
        public string Concaucan { get; set; }
        public decimal? Conobjtot { get; set; }

        // Propiedades de tipo de actividad
        public string Contpoact { get; set; }
        public string Conesp { get; set; }
        public decimal? Convalacr { get; set; }
        public decimal? Convallet { get; set; }

        // Propiedades para transporte
        public string Condecram { get; set; }
        public string Conmedtra { get; set; }
        public string Conviades { get; set; }
        public string Conviaa { get; set; }
        public string Conviaenb { get; set; }
        public decimal? Conviakb { get; set; }
        public decimal? Conviakn { get; set; }
        public string Conviatra { get; set; }
        public decimal? Conviacos { get; set; }
        public decimal? Conviafle { get; set; }

        // Propiedades de departamento y ubicación
        public int? Dptnom { get; set; }
        public decimal? Conedaret { get; set; }
        public decimal? Congar { get; set; }

        // Propiedades de declaración
        public decimal? Condecpri { get; set; }
        public decimal? Condecpro { get; set; }
        public decimal? Condecptj { get; set; }
        public string Conubi { get; set; }
        public string Concaudsc { get; set; }
        public string Conincuno { get; set; }

        // Propiedades de viaje
        public decimal? Conviagas { get; set; }
        public decimal? Conviarec { get; set; }
        public decimal? Conviapri { get; set; }

        // Propiedades de líneas y comisiones
        public int? Linobs { get; set; }
        public DateTime? Concomdes { get; set; }
        public string Concalcom { get; set; }

        // Propiedades de tipo
        public int? Tpoconcod { get; set; }
        public int? Tpovivcod { get; set; }
        public int? Tporiecod { get; set; }
        public int? Modcod { get; set; }

        // Propiedades de capital
        public decimal? Concapase { get; set; }
        public decimal? Conpricap { get; set; }
        public string Tposegdsc { get; set; }
        public int? Conriecod { get; set; }
        public string Conriedsc { get; set; }
        public decimal? Conrecfin { get; set; }
        public decimal? Conimprf { get; set; }
        public int? Conafesin { get; set; }
        public int? Conautcor { get; set; }
        public int? Conlinrie { get; set; }
        public int? Conconesp { get; set; }

        // Propiedades de navegación
        public string Conlimnav { get; set; }
        public string Contpocob { get; set; }
        public string Connomemb { get; set; }
        public string Contpoemb { get; set; }
        public int? Lincarta { get; set; }
        public int? Cancecod { get; set; }
        public decimal? Concomotr { get; set; }
        public string Conautcome { get; set; }
        public string Conviafac { get; set; }
        public int? Conviamon { get; set; }
        public string Conviatpo { get; set; }
        public string Connrorc { get; set; }
        public string Condedurc { get; set; }
        public int? Lininclu { get; set; }
        public int? Linexclu { get; set; }
        public decimal? Concapret { get; set; }
        public string Forpagvid { get; set; }

        // Propiedades de cliente y corredor
        public string Clinom { get; set; }
        public int? Tarcod { get; set; }
        public int? Corrnom { get; set; }
        public int? Connroint { get; set; }
        public string Conautnd { get; set; }
        public int? Conpadend { get; set; }
        public decimal? Contotpri { get; set; }
        public int? Padreaux { get; set; }

        // Propiedades de flota
        public int? Conlinflot { get; set; }
        public decimal? Conflotimp { get; set; }
        public decimal? Conflottotal { get; set; }
        public decimal? Conflotsaldo { get; set; }

        // Propiedades de certificación
        public string Conaccicer { get; set; }
        public DateTime? Concerfin { get; set; }

        // Propiedades de embarcación
        public string Condetemb { get; set; }
        public string Conclaemb { get; set; }
        public string Confabemb { get; set; }
        public string Conbanemb { get; set; }
        public string Conmatemb { get; set; }
        public string Convelemb { get; set; }
        public string Conmatriemb { get; set; }
        public string Conptoemb { get; set; }

        // Propiedades de corredores
        public int? Otrcorrcod { get; set; }
        public string Condeta { get; set; }
        public string Observaciones { get; set; }
        public decimal? Clipcupfia { get; set; }
        public string Conclieda { get; set; }
        public string Condecrea { get; set; }
        public string Condecaju { get; set; }
        public decimal? Conviatot { get; set; }
        public string Contpoemp { get; set; }
        public string Congaran { get; set; }
        public string Congarantel { get; set; }
        public string Mot_no_ren { get; set; }
        public string Condetrc { get; set; }
        public bool Conautcort { get; set; }
        public string Condetail { get; set; }
        public int? Clinro1 { get; set; }
        public decimal? Consumsal { get; set; }
        public string Conespbon { get; set; }

        // Propiedades de estado
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
        public decimal? Contotant { get; set; }
        public string Cotizacion { get; set; }
        public int? Motivos_no_renovacion { get; set; }
        public string Com_alias { get; set; }
        public string Ramo { get; set; }
        public string Clausula { get; set; }

        // Propiedades de tipo de transporte
        public bool Aereo { get; set; }
        public bool Maritimo { get; set; }
        public bool Terrestre { get; set; }
        public decimal? Max_aereo { get; set; }
        public decimal? Max_mar { get; set; }
        public decimal? Max_terrestre { get; set; }
        public decimal? Tasa { get; set; }
        public string Facturacion { get; set; }

        // Propiedades de comercio
        public bool Importacion { get; set; }
        public bool Exportacion { get; set; }
        public bool Offshore { get; set; }
        public bool Transito_interno { get; set; }
        public string Coning { get; set; }
        public int? Cat_cli { get; set; }
        public bool Llamar { get; set; }
        public bool Granizo { get; set; }
        public string Idorden { get; set; }
        public bool Var_ubi { get; set; }
        public bool Mis_rie { get; set; }

        // Propiedades de fecha y actualización
        public DateTime? Ingresado { get; set; }
        public DateTime? Last_update { get; set; }
        public int? Comcod1 { get; set; }
        public int? Comcod2 { get; set; }
        public int? Pagos_efectivo { get; set; }
        public int? Productos_de_vida { get; set; }
        public int? App_id { get; set; }
        public DateTime? Update_date { get; set; }
        public string Gestion { get; set; }
        public int? Asignado { get; set; }
        public string Combustibles { get; set; }
        public int? Conidpad { get; set; }

        // Propiedades adicionales para la aplicación
        public bool Procesado { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        // Propiedades de navegación para la aplicación
        public ClientDto Cliente { get; set; }
        public string Cliruc { get; set; }
        public string Clilocnom { get; set; }
        public string Clidptnom { get; set; }
        public string Clitelcel { get; set; }
        public string Cliemail { get; set; }
        public object Cliposcod { get; set; }
    }
}