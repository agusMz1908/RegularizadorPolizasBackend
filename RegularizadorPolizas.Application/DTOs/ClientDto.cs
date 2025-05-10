using System;
using System.Collections.Generic;

namespace RegularizadorPolizas.Application.DTOs
{
    public class ClientDto
    {
        public int Id { get; set; }
        public int? Corrcod { get; set; }
        public int? Subcorr { get; set; }
        public string Clinom { get; set; }
        public string Telefono { get; set; }
        public string Clitelcel { get; set; }
        public DateTime? Clifchnac { get; set; }
        public DateTime? Clifching { get; set; }
        public DateTime? Clifchegr { get; set; }
        public string Clicargo { get; set; }
        public string Clicon { get; set; }
        public string Cliruc { get; set; }
        public string Clirsoc { get; set; }
        public string Cliced { get; set; }
        public string Clilib { get; set; }
        public string Clicatlib { get; set; }
        public string Clitpo { get; set; }
        public string Clidir { get; set; }
        public string Cliemail { get; set; }
        public DateTime? Clivtoced { get; set; }
        public DateTime? Clivtolib { get; set; }
        public int? Cliposcod { get; set; }
        public string Clitelcorr { get; set; }
        public string Clidptnom { get; set; }
        public string Clisex { get; set; }
        public string Clitelant { get; set; }
        public string Cliobse { get; set; }
        public string Clifax { get; set; }
        public string Cliclasif { get; set; }
        public string Clinumrel { get; set; }
        public string Clicasapt { get; set; }
        public string Clidircob { get; set; }
        public int? Clibse { get; set; }
        public string Clifoto { get; set; }
        public int? Pruebamillares { get; set; }
        public string Ingresado { get; set; }
        public string Clialias { get; set; }
        public string Clipor { get; set; }
        public string Clisancor { get; set; }
        public string Clirsa { get; set; }
        public int? Codposcob { get; set; }
        public string Clidptcob { get; set; }
        public bool Activo { get; set; }
        public string Cli_s_cris { get; set; }
        public DateTime? Clifchnac1 { get; set; }
        public string Clilocnom { get; set; }
        public string Cliloccob { get; set; }
        public int? Categorias_de_cliente { get; set; }
        public string Sc_departamentos { get; set; }
        public string Sc_localidades { get; set; }
        public DateTime? Fch_ingreso { get; set; }
        public int? Grupos_economicos { get; set; }
        public bool Etiquetas { get; set; }
        public bool Doc_digi { get; set; }
        public string Password { get; set; }
        public bool Habilita_app { get; set; }
        public string Referido { get; set; }
        public int? Altura { get; set; }
        public int? Peso { get; set; }
        public string Cliberkley { get; set; }
        public string Clifar { get; set; }
        public string Clisurco { get; set; }
        public string Clihdi { get; set; }
        public string Climapfre { get; set; }
        public string Climetlife { get; set; }
        public string Clisancris { get; set; }
        public string Clisbi { get; set; }
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

        // Propiedades de navegación simplificadas
        public List<PolizaResumidaDto> Polizas { get; set; }
    }

    // DTO simplificado para mostrar resumen de pólizas cuando se cargan con el cliente
    public class PolizaResumidaDto
    {
        public int Id { get; set; }
        public string Conpol { get; set; }
        public string Ramo { get; set; }
        public DateTime? Confchdes { get; set; }
        public DateTime? Confchhas { get; set; }
        public string Convig { get; set; }
        public string ComAlias { get; set; }
    }
}