using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoPoliza
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("comcod")]
        public object? Comcod { get; set; }

        [JsonPropertyName("seccod")]
        public object? Seccod { get; set; }

        [JsonPropertyName("clinro")]
        public object? Clinro { get; set; }

        [JsonPropertyName("condom")]
        public string Condom { get; set; } = string.Empty;

        [JsonPropertyName("conmaraut")]
        public string Conmaraut { get; set; } = string.Empty;

        [JsonPropertyName("conanioaut")]
        public object? Conanioaut { get; set; }

        [JsonPropertyName("concodrev")]
        public object? Concodrev { get; set; }

        [JsonPropertyName("conmataut")]
        public string Conmataut { get; set; } = string.Empty;

        [JsonPropertyName("conficto")]
        public object? Conficto { get; set; }

        [JsonPropertyName("conmotor")]
        public string Conmotor { get; set; } = string.Empty;

        [JsonPropertyName("conpadaut")]
        public string Conpadaut { get; set; } = string.Empty;

        [JsonPropertyName("conchasis")]
        public string Conchasis { get; set; } = string.Empty;

        [JsonPropertyName("conclaaut")]
        public object? Conclaaut { get; set; }

        [JsonPropertyName("condedaut")]
        public object? Condedaut { get; set; }

        [JsonPropertyName("conresciv")]
        public object? Conresciv { get; set; }

        [JsonPropertyName("conbonnsin")]
        public object? Conbonnsin { get; set; }

        [JsonPropertyName("conbonant")]
        public object? Conbonant { get; set; } // ESTE ES EL QUE ESTÁ FALLANDO AHORA

        [JsonPropertyName("concaraut")]
        public object? Concaraut { get; set; }

        [JsonPropertyName("concesnom")]
        public string Concesnom { get; set; } = string.Empty;

        [JsonPropertyName("concestel")]
        public string Concestel { get; set; } = string.Empty;

        [JsonPropertyName("concapaut")]
        public object? Concapaut { get; set; }

        [JsonPropertyName("conpremio")]
        public object? Conpremio { get; set; }

        [JsonPropertyName("contot")]
        public object? Contot { get; set; }

        [JsonPropertyName("moncod")]
        public object? Moncod { get; set; }

        [JsonPropertyName("concuo")]
        public object? Concuo { get; set; }

        [JsonPropertyName("concomcorr")]
        public object? Concomcorr { get; set; }

        [JsonPropertyName("catdsc")]
        public int? Catdsc { get; set; }

        [JsonPropertyName("desdsc")]
        public int? Desdsc { get; set; }

        [JsonPropertyName("caldsc")]
        public int? Caldsc { get; set; }

        [JsonPropertyName("flocod")]
        public int? Flocod { get; set; }

        [JsonPropertyName("concar")]
        public string Concar { get; set; } = string.Empty;

        [JsonPropertyName("conpol")]
        public string Conpol { get; set; } = string.Empty;

        [JsonPropertyName("conend")]
        public string Conend { get; set; } = string.Empty;

        [JsonPropertyName("confchdes")]
        public string Confchdes { get; set; } = string.Empty;

        [JsonPropertyName("confchhas")]
        public string Confchhas { get; set; } = string.Empty;

        [JsonPropertyName("conimp")]
        public decimal? Conimp { get; set; }

        [JsonPropertyName("connroser")]
        public int? Connroser { get; set; }

        [JsonPropertyName("rieres")]
        public string Rieres { get; set; } = string.Empty;

        [JsonPropertyName("conges")]
        public string Conges { get; set; } = string.Empty;

        [JsonPropertyName("congesti")]
        public string Congesti { get; set; } = string.Empty;

        [JsonPropertyName("congesfi")]
        public string Congesfi { get; set; } = string.Empty;

        [JsonPropertyName("congeses")]
        public string Congeses { get; set; } = string.Empty;

        [JsonPropertyName("convig")]
        public string Convig { get; set; } = string.Empty;

        [JsonPropertyName("concan")]
        public decimal? Concan { get; set; }

        [JsonPropertyName("congrucon")]
        public string Congrucon { get; set; } = string.Empty;

        [JsonPropertyName("contipoemp")]
        public string Contipoemp { get; set; } = string.Empty;

        [JsonPropertyName("conmatpar")]
        public string Conmatpar { get; set; } = string.Empty;

        [JsonPropertyName("conmatte")]
        public string Conmatte { get; set; } = string.Empty;

        [JsonPropertyName("concapla")]
        public decimal? Concapla { get; set; }

        [JsonPropertyName("conflota")]
        public decimal? Conflota { get; set; }

        [JsonPropertyName("condednum")]
        public object? Condednum { get; set; } // CAMBIADO: de int? a object para manejar valores problemáticos

        [JsonPropertyName("consta")]
        public string Consta { get; set; } = string.Empty;

        [JsonPropertyName("contra")]
        public string Contra { get; set; } = string.Empty;

        [JsonPropertyName("conconf")]
        public string Conconf { get; set; } = string.Empty;

        [JsonPropertyName("conpadre")]
        public int? Conpadre { get; set; }

        [JsonPropertyName("confchcan")]
        public string Confchcan { get; set; } = string.Empty;

        [JsonPropertyName("concaucan")]
        public string Concaucan { get; set; } = string.Empty;

        [JsonPropertyName("conobjtot")]
        public decimal? Conobjtot { get; set; }

        // Nuevos campos que faltaban
        [JsonPropertyName("contpoact")]
        public string Contpoact { get; set; } = string.Empty;

        [JsonPropertyName("conesp")]
        public string Conesp { get; set; } = string.Empty;

        [JsonPropertyName("convalacr")]
        public decimal? Convalacr { get; set; }

        [JsonPropertyName("convallet")]
        public decimal? Convallet { get; set; }

        [JsonPropertyName("condecram")]
        public string Condecram { get; set; } = string.Empty;

        [JsonPropertyName("conmedtra")]
        public string Conmedtra { get; set; } = string.Empty;

        [JsonPropertyName("conviades")]
        public string Conviades { get; set; } = string.Empty;

        [JsonPropertyName("conviaa")]
        public string Conviaa { get; set; } = string.Empty;

        [JsonPropertyName("conviaenb")]
        public string Conviaenb { get; set; } = string.Empty;

        [JsonPropertyName("conviakb")]
        public decimal? Conviakb { get; set; }

        [JsonPropertyName("conviakn")]
        public decimal? Conviakn { get; set; }

        [JsonPropertyName("conviatra")]
        public string Conviatra { get; set; } = string.Empty;

        [JsonPropertyName("conviacos")]
        public decimal? Conviacos { get; set; }

        [JsonPropertyName("conviafle")]
        public decimal? Conviafle { get; set; }

        [JsonPropertyName("dptnom")]
        public int? Dptnom { get; set; }

        [JsonPropertyName("conedaret")]
        public decimal? Conedaret { get; set; }

        [JsonPropertyName("congar")]
        public decimal? Congar { get; set; }

        [JsonPropertyName("condecpri")]
        public decimal? Condecpri { get; set; }

        [JsonPropertyName("condecpro")]
        public decimal? Condecpro { get; set; }

        [JsonPropertyName("condecptj")]
        public decimal? Condecptj { get; set; }

        [JsonPropertyName("conubi")]
        public string Conubi { get; set; } = string.Empty;

        [JsonPropertyName("concaudsc")]
        public string Concaudsc { get; set; } = string.Empty;

        [JsonPropertyName("conincuno")]
        public string Conincuno { get; set; } = string.Empty;

        [JsonPropertyName("conviagas")]
        public decimal? Conviagas { get; set; }

        [JsonPropertyName("conviarec")]
        public decimal? Conviarec { get; set; }

        [JsonPropertyName("conviapri")]
        public decimal? Conviapri { get; set; }

        [JsonPropertyName("linobs")]
        public int? Linobs { get; set; }

        [JsonPropertyName("concomdes")]
        public string Concomdes { get; set; } = string.Empty;

        [JsonPropertyName("concalcom")]
        public string Concalcom { get; set; } = string.Empty;

        [JsonPropertyName("tpoconcod")]
        public int? Tpoconcod { get; set; }

        [JsonPropertyName("tpovivcod")]
        public int? Tpovivcod { get; set; }

        [JsonPropertyName("tporiecod")]
        public int? Tporiecod { get; set; }

        [JsonPropertyName("modcod")]
        public int? Modcod { get; set; }

        [JsonPropertyName("concapase")]
        public decimal? Concapase { get; set; }

        [JsonPropertyName("conpricap")]
        public decimal? Conpricap { get; set; }

        [JsonPropertyName("tposegdsc")]
        public string Tposegdsc { get; set; } = string.Empty;

        [JsonPropertyName("conriecod")]
        public int? Conriecod { get; set; }

        [JsonPropertyName("conriedsc")]
        public string Conriedsc { get; set; } = string.Empty;

        [JsonPropertyName("conrecfin")]
        public decimal? Conrecfin { get; set; }

        [JsonPropertyName("conimprf")]
        public decimal? Conimprf { get; set; }

        [JsonPropertyName("conafesin")]
        public int? Conafesin { get; set; }

        [JsonPropertyName("conautcor")]
        public int? Conautcor { get; set; }

        [JsonPropertyName("conlinrie")]
        public int? Conlinrie { get; set; }

        [JsonPropertyName("conconesp")]
        public int? Conconesp { get; set; }

        [JsonPropertyName("conlimnav")]
        public string Conlimnav { get; set; } = string.Empty;

        [JsonPropertyName("contpocob")]
        public string Contpocob { get; set; } = string.Empty;

        [JsonPropertyName("connomemb")]
        public string Connomemb { get; set; } = string.Empty;

        [JsonPropertyName("contpoemb")]
        public string Contpoemb { get; set; } = string.Empty;

        [JsonPropertyName("lincarta")]
        public int? Lincarta { get; set; }

        [JsonPropertyName("cancecod")]
        public int? Cancecod { get; set; }

        [JsonPropertyName("concomotr")]
        public decimal? Concomotr { get; set; }

        [JsonPropertyName("conautcome")]
        public string Conautcome { get; set; } = string.Empty;

        [JsonPropertyName("conviafac")]
        public string Conviafac { get; set; } = string.Empty;

        [JsonPropertyName("conviamon")]
        public int? Conviamon { get; set; }

        [JsonPropertyName("conviatpo")]
        public string Conviatpo { get; set; } = string.Empty;

        [JsonPropertyName("connrorc")]
        public string Connrorc { get; set; } = string.Empty;

        [JsonPropertyName("condedurc")]
        public string Condedurc { get; set; } = string.Empty;

        [JsonPropertyName("lininclu")]
        public int? Lininclu { get; set; }

        [JsonPropertyName("linexclu")]
        public int? Linexclu { get; set; }

        [JsonPropertyName("concapret")]
        public decimal? Concapret { get; set; }

        [JsonPropertyName("forpagvid")]
        public string Forpagvid { get; set; } = string.Empty;

        [JsonPropertyName("clinom")]
        public string Clinom { get; set; } = string.Empty;

        [JsonPropertyName("tarcod")]
        public int? Tarcod { get; set; }

        [JsonPropertyName("corrnom")]
        public int? Corrnom { get; set; }

        [JsonPropertyName("connroint")]
        public int? Connroint { get; set; }

        [JsonPropertyName("conautnd")]
        public string Conautnd { get; set; } = string.Empty;

        [JsonPropertyName("conpadend")]
        public int? Conpadend { get; set; }

        [JsonPropertyName("contotpri")]
        public decimal? Contotpri { get; set; }

        [JsonPropertyName("padreaux")]
        public int? Padreaux { get; set; }

        [JsonPropertyName("conlinflot")]
        public int? Conlinflot { get; set; }

        [JsonPropertyName("conflotimp")]
        public decimal? Conflotimp { get; set; }

        [JsonPropertyName("conflottotal")]
        public decimal? Conflottotal { get; set; }

        [JsonPropertyName("conflotsaldo")]
        public decimal? Conflotsaldo { get; set; }

        [JsonPropertyName("conaccicer")]
        public string Conaccicer { get; set; } = string.Empty;

        [JsonPropertyName("concerfin")]
        public string Concerfin { get; set; } = string.Empty;

        [JsonPropertyName("condetemb")]
        public string Condetemb { get; set; } = string.Empty;

        [JsonPropertyName("conclaemb")]
        public string Conclaemb { get; set; } = string.Empty;

        [JsonPropertyName("confabemb")]
        public string Confabemb { get; set; } = string.Empty;

        [JsonPropertyName("conbanemb")]
        public string Conbanemb { get; set; } = string.Empty;

        [JsonPropertyName("conmatemb")]
        public string Conmatemb { get; set; } = string.Empty;

        [JsonPropertyName("convelemb")]
        public string Convelemb { get; set; } = string.Empty;

        [JsonPropertyName("conmatriemb")]
        public string Conmatriemb { get; set; } = string.Empty;

        [JsonPropertyName("conptoemb")]
        public string Conptoemb { get; set; } = string.Empty;

        [JsonPropertyName("otrcorrcod")]
        public int? Otrcorrcod { get; set; }

        [JsonPropertyName("condeta")]
        public string Condeta { get; set; } = string.Empty;

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; } = string.Empty;

        [JsonPropertyName("clipcupfia")]
        public decimal? Clipcupfia { get; set; }

        [JsonPropertyName("conclieda")]
        public string Conclieda { get; set; } = string.Empty;

        [JsonPropertyName("condecrea")]
        public string Condecrea { get; set; } = string.Empty;

        [JsonPropertyName("condecaju")]
        public string Condecaju { get; set; } = string.Empty;

        [JsonPropertyName("conviatot")]
        public decimal? Conviatot { get; set; }

        [JsonPropertyName("contpoemp")]
        public string Contpoemp { get; set; } = string.Empty;

        [JsonPropertyName("congaran")]
        public string Congaran { get; set; } = string.Empty;

        [JsonPropertyName("congarantel")]
        public string Congarantel { get; set; } = string.Empty;

        [JsonPropertyName("mot_no_ren")]
        public string Mot_no_ren { get; set; } = string.Empty;

        [JsonPropertyName("condetrc")]
        public string Condetrc { get; set; } = string.Empty;

        [JsonPropertyName("conautcort")]
        public bool Conautcort { get; set; }

        [JsonPropertyName("condetail")]
        public string Condetail { get; set; } = string.Empty;

        [JsonPropertyName("clinro1")]
        public int? Clinro1 { get; set; }

        [JsonPropertyName("consumsal")]
        public decimal? Consumsal { get; set; }

        [JsonPropertyName("conespbon")]
        public string Conespbon { get; set; } = string.Empty;

        [JsonPropertyName("leer")]
        public bool Leer { get; set; }

        [JsonPropertyName("enviado")]
        public bool Enviado { get; set; }

        [JsonPropertyName("sob_recib")]
        public bool Sob_recib { get; set; }

        [JsonPropertyName("leer_obs")]
        public bool Leer_obs { get; set; }

        [JsonPropertyName("sublistas")]
        public object? Sublistas { get; set; }

        [JsonPropertyName("com_sub_corr")]
        public int? Com_sub_corr { get; set; }

        [JsonPropertyName("tipos_de_alarma")]
        public int? Tipos_de_alarma { get; set; }

        [JsonPropertyName("tiene_alarma")]
        public bool Tiene_alarma { get; set; }

        [JsonPropertyName("coberturas_bicicleta")]
        public int? Coberturas_bicicleta { get; set; }

        [JsonPropertyName("com_bro")]
        public int? Com_bro { get; set; }

        [JsonPropertyName("com_bo")]
        public int? Com_bo { get; set; }

        [JsonPropertyName("contotant")]
        public decimal? Contotant { get; set; }

        [JsonPropertyName("cotizacion")]
        public string Cotizacion { get; set; } = string.Empty;

        [JsonPropertyName("motivos_no_renovacion")]
        public int? Motivos_no_renovacion { get; set; }

        [JsonPropertyName("com_alias")]
        public string ComAlias { get; set; } = string.Empty;

        [JsonPropertyName("ramo")]
        public string Ramo { get; set; } = string.Empty;

        [JsonPropertyName("clausula")]
        public string Clausula { get; set; } = string.Empty;

        [JsonPropertyName("aereo")]
        public bool Aereo { get; set; }

        [JsonPropertyName("maritimo")]
        public bool Maritimo { get; set; }

        [JsonPropertyName("terrestre")]
        public bool Terrestre { get; set; }

        [JsonPropertyName("max_aereo")]
        public decimal? Max_aereo { get; set; }

        [JsonPropertyName("max_mar")]
        public decimal? Max_mar { get; set; }

        [JsonPropertyName("max_terrestre")]
        public decimal? Max_terrestre { get; set; }

        [JsonPropertyName("tasa")]
        public decimal? Tasa { get; set; }

        [JsonPropertyName("facturacion")]
        public string Facturacion { get; set; } = string.Empty;

        [JsonPropertyName("importacion")]
        public bool Importacion { get; set; }

        [JsonPropertyName("exportacion")]
        public bool Exportacion { get; set; }

        [JsonPropertyName("offshore")]
        public bool Offshore { get; set; }

        [JsonPropertyName("transito_interno")]
        public bool Transito_interno { get; set; }

        [JsonPropertyName("coning")]
        public string Coning { get; set; } = string.Empty;

        [JsonPropertyName("cat_cli")]
        public int? Cat_cli { get; set; }

        [JsonPropertyName("llamar")]
        public bool Llamar { get; set; }

        [JsonPropertyName("granizo")]
        public bool Granizo { get; set; }

        [JsonPropertyName("idorden")]
        public string Idorden { get; set; } = string.Empty;

        [JsonPropertyName("var_ubi")]
        public bool Var_ubi { get; set; }

        [JsonPropertyName("mis_rie")]
        public bool Mis_rie { get; set; }

        [JsonPropertyName("ingresado")]
        public string Ingresado { get; set; } = string.Empty;

        [JsonPropertyName("last_update")]
        public string Last_update { get; set; } = string.Empty;

        [JsonPropertyName("comcod1")]
        public int? Comcod1 { get; set; }

        [JsonPropertyName("comcod2")]
        public int? Comcod2 { get; set; }

        [JsonPropertyName("pagos_efectivo")]
        public int? Pagos_efectivo { get; set; }

        [JsonPropertyName("productos_de_vida")]
        public int? Productos_de_vida { get; set; }

        [JsonPropertyName("app_id")]
        public int? App_id { get; set; }

        [JsonPropertyName("update_date")]
        public string Update_date { get; set; } = string.Empty;

        [JsonPropertyName("gestion")]
        public string Gestion { get; set; } = string.Empty;

        [JsonPropertyName("asignado")]
        public int? Asignado { get; set; }

        [JsonPropertyName("combustibles")]
        public string Combustibles { get; set; } = string.Empty;

        [JsonPropertyName("conidpad")]
        public int? Conidpad { get; set; }
    }

    public class VelneoPolizaResponse
    {
        [JsonPropertyName("poliza")]
        public VelneoPoliza Poliza { get; set; } = new();

        [JsonPropertyName("contrato")]  
        public VelneoPoliza Contrato { get; set; } = new();
    }

    public class VelneoPolizasResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonIgnore]
        public int? TotalCount => Total_Count == 0 ? null : Total_Count;

        [JsonPropertyName("polizas")]
        public List<VelneoPoliza> Polizas { get; set; } = new();

        [JsonPropertyName("contratos")]  
        public List<VelneoPoliza> Contratos { get; set; } = new();

        [JsonPropertyName("has_more_data")]
        public bool? HasMoreData { get; set; }

        [JsonPropertyName("current_page")]
        public int? CurrentPage { get; set; }
    }
}