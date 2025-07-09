using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoCliente
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("corrcod")]
        public int? Corrcod { get; set; }

        [JsonPropertyName("subcorr")]
        public int? Subcorr { get; set; }

        [JsonPropertyName("clinom")]
        public string Clinom { get; set; } = string.Empty;

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [JsonPropertyName("clifchnac")]
        public string? Clifchnac { get; set; }

        [JsonPropertyName("clifching")]
        public string? Clifching { get; set; }

        [JsonPropertyName("clifchegr")]
        public string? Clifchegr { get; set; }

        [JsonPropertyName("clicargo")]
        public string Clicargo { get; set; } = string.Empty;

        [JsonPropertyName("clicon")]
        public string Clicon { get; set; } = string.Empty;

        [JsonPropertyName("cliruc")]
        public string Cliruc { get; set; } = string.Empty;

        [JsonPropertyName("clirsoc")]
        public string Clirsoc { get; set; } = string.Empty;

        [JsonPropertyName("cliced")]
        public string Cliced { get; set; } = string.Empty;

        [JsonPropertyName("clilib")]
        public string Clilib { get; set; } = string.Empty;

        [JsonPropertyName("clicatlib")]
        public string Clicatlib { get; set; } = string.Empty;

        [JsonPropertyName("clitpo")]
        public string Clitpo { get; set; } = string.Empty;

        [JsonPropertyName("clidir")]
        public string Clidir { get; set; } = string.Empty;

        [JsonPropertyName("cliemail")]
        public string Cliemail { get; set; } = string.Empty;

        [JsonPropertyName("clivtoced")]
        public string? Clivtoced { get; set; }

        [JsonPropertyName("clivtolib")]
        public string? Clivtolib { get; set; }

        [JsonPropertyName("cliposcod")]
        public int? Cliposcod { get; set; }

        [JsonPropertyName("clitelcorr")]
        public string Clitelcorr { get; set; } = string.Empty;

        [JsonPropertyName("clidptnom")]
        public string Clidptnom { get; set; } = string.Empty;

        [JsonPropertyName("clisex")]
        public string Clisex { get; set; } = string.Empty;

        [JsonPropertyName("clitelant")]
        public string Clitelant { get; set; } = string.Empty;

        [JsonPropertyName("cliobse")]
        public string Cliobse { get; set; } = string.Empty;

        [JsonPropertyName("clifax")]
        public string Clifax { get; set; } = string.Empty;

        [JsonPropertyName("clitelcel")]
        public string Clitelcel { get; set; } = string.Empty;

        [JsonPropertyName("cliclasif")]
        public string Cliclasif { get; set; } = string.Empty;

        [JsonPropertyName("clinumrel")]
        public string Clinumrel { get; set; } = string.Empty;

        [JsonPropertyName("clicasapt")]
        public string Clicasapt { get; set; } = string.Empty;

        [JsonPropertyName("clidircob")]
        public string Clidircob { get; set; } = string.Empty;

        [JsonPropertyName("clibse")]
        public int? Clibse { get; set; }

        [JsonPropertyName("clifoto")]
        public string Clifoto { get; set; } = string.Empty;

        [JsonPropertyName("pruebamillares")]
        public int? Pruebamillares { get; set; }

        [JsonPropertyName("ingresado")]
        public string Ingresado { get; set; } = string.Empty;

        [JsonPropertyName("clialias")]
        public string Clialias { get; set; } = string.Empty;

        [JsonPropertyName("clipor")]
        public string Clipor { get; set; } = string.Empty;

        [JsonPropertyName("clisancor")]
        public string Clisancor { get; set; } = string.Empty;

        [JsonPropertyName("clirsa")]
        public string Clirsa { get; set; } = string.Empty;

        [JsonPropertyName("codposcob")]
        public int? Codposcob { get; set; }

        [JsonPropertyName("clidptcob")]
        public string Clidptcob { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("cli_s_cris")]
        public string Cli_s_cris { get; set; } = string.Empty;

        [JsonPropertyName("clifchnac1")]
        public string? Clifchnac1 { get; set; }

        [JsonPropertyName("clilocnom")]
        public string Clilocnom { get; set; } = string.Empty;

        [JsonPropertyName("cliloccob")]
        public string Cliloccob { get; set; } = string.Empty;

        [JsonPropertyName("categorias_de_cliente")]
        public int? Categorias_de_cliente { get; set; }

        [JsonPropertyName("sc_departamentos")]
        public string Sc_departamentos { get; set; } = string.Empty;

        [JsonPropertyName("sc_localidades")]
        public string Sc_localidades { get; set; } = string.Empty;

        [JsonPropertyName("fch_ingreso")]
        public string? Fch_ingreso { get; set; }

        [JsonPropertyName("grupos_economicos")]
        public int? Grupos_economicos { get; set; }

        [JsonPropertyName("etiquetas")]
        public bool Etiquetas { get; set; }

        [JsonPropertyName("doc_digi")]
        public bool Doc_digi { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("habilita_app")]
        public bool Habilita_app { get; set; }

        [JsonPropertyName("referido")]
        public string Referido { get; set; } = string.Empty;

        [JsonPropertyName("altura")]
        public int? Altura { get; set; }

        [JsonPropertyName("peso")]
        public int? Peso { get; set; }

        [JsonPropertyName("cliberkley")]
        public string Cliberkley { get; set; } = string.Empty;

        [JsonPropertyName("clifar")]
        public string Clifar { get; set; } = string.Empty;

        [JsonPropertyName("clisurco")]
        public string Clisurco { get; set; } = string.Empty;

        [JsonPropertyName("clihdi")]
        public string Clihdi { get; set; } = string.Empty;

        [JsonPropertyName("climapfre")]
        public string Climapfre { get; set; } = string.Empty;

        [JsonPropertyName("climetlife")]
        public string Climetlife { get; set; } = string.Empty;

        [JsonPropertyName("clisancris")]
        public string Clisancris { get; set; } = string.Empty;

        [JsonPropertyName("clisbi")]
        public string Clisbi { get; set; } = string.Empty;

        [JsonPropertyName("edo_civil")]
        public string Edo_civil { get; set; } = string.Empty;

        [JsonPropertyName("not_bien_mail")]
        public bool Not_bien_mail { get; set; }

        [JsonPropertyName("not_bien_wap")]
        public bool Not_bien_wap { get; set; }

        [JsonPropertyName("ing_poliza_mail")]
        public bool Ing_poliza_mail { get; set; }

        [JsonPropertyName("ing_poliza_wap")]
        public bool Ing_poliza_wap { get; set; }

        [JsonPropertyName("ing_siniestro_mail")]
        public bool Ing_siniestro_mail { get; set; }

        [JsonPropertyName("ing_siniestro_wap")]
        public bool Ing_siniestro_wap { get; set; }

        [JsonPropertyName("noti_obs_sini_mail")]
        public bool Noti_obs_sini_mail { get; set; }

        [JsonPropertyName("noti_obs_sini_wap")]
        public bool Noti_obs_sini_wap { get; set; }

        [JsonPropertyName("last_update")]
        public string? Last_update { get; set; }

        [JsonPropertyName("app_id")]
        public int? App_id { get; set; }
    }
}