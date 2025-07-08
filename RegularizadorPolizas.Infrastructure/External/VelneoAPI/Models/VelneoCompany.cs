using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoCompany
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("comnom")]
        public string? Comnom { get; set; }

        [JsonPropertyName("comrazsoc")]
        public string? Comrazsoc { get; set; }

        [JsonPropertyName("comruc")]
        public string? Comruc { get; set; }

        [JsonPropertyName("comdom")]
        public string? Comdom { get; set; }

        [JsonPropertyName("comtel")]
        public string? Comtel { get; set; }

        [JsonPropertyName("comfax")]
        public string? Comfax { get; set; }

        [JsonPropertyName("comsumodia")]
        public string? Comsumodia { get; set; }

        [JsonPropertyName("comcntcli")]
        public int Comcntcli { get; set; }

        [JsonPropertyName("comcntcon")]
        public int Comcntcon { get; set; }

        [JsonPropertyName("comprepes")]
        public decimal Comprepes { get; set; }

        [JsonPropertyName("compredol")]
        public decimal Compredol { get; set; }

        [JsonPropertyName("comcomipe")]
        public decimal Comcomipe { get; set; }

        [JsonPropertyName("comcomido")]
        public decimal Comcomido { get; set; }

        [JsonPropertyName("comtotcomi")]
        public decimal Comtotcomi { get; set; }

        [JsonPropertyName("comtotpre")]
        public decimal Comtotpre { get; set; }

        [JsonPropertyName("comalias")]
        public string? Comalias { get; set; }

        [JsonPropertyName("comlog")]
        public string? Comlog { get; set; }

        [JsonPropertyName("broker")]
        public bool Broker { get; set; }

        [JsonPropertyName("cod_srvcompanias")]
        public string? Cod_srvcompanias { get; set; }

        [JsonPropertyName("no_utiles")]
        public string? No_utiles { get; set; } 

        [JsonPropertyName("paq_dias")]
        public int Paq_dias { get; set; }

        public string? Nombre => Comnom;
        public string? Codigo => Comalias;
        public string? Descripcion => Comrazsoc;
        public bool Activo => true; 
        public string? Direccion => Comdom;
        public string? Telefono => Comtel;
        public string? Email => string.Empty;
        public string? Website => string.Empty;
        public string? Ruc => Comruc;
        public string? ContactoPrincipal => string.Empty;
        public string? TelefonoContacto => string.Empty;
        public string? EmailContacto => string.Empty;
        public DateTime FechaCreacion => DateTime.Now;
        public DateTime FechaModificacion => DateTime.Now;
    }
    public class VelneoCompaniesResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("companias")]
        public List<VelneoCompany> Companias { get; set; } = new();
    }

    public class VelneoCompanyLookup
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("comnom")]
        public string? Comnom { get; set; }

        [JsonPropertyName("comalias")]
        public string? Comalias { get; set; }

        [JsonPropertyName("cod_srvcompanias")]
        public string? Cod_srvcompanias { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }

        public string? Nombre => Comnom;
        public string? Codigo => Comalias;
    }
}