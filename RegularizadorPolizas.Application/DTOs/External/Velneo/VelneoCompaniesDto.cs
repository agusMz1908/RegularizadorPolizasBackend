// 📁 RegularizadorPolizas.Application/DTOs/External/Velneo/VelneoCompanyDto.cs

using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.DTOs.External.Velneo
{
    public class VelneoCompaniesResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("companias")]
        public List<VelneoCompanyDto> Companias { get; set; } = new List<VelneoCompanyDto>();
    }

    public class VelneoCompanyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("comnom")]
        public string Comnom { get; set; } = string.Empty;

        [JsonPropertyName("comrazsoc")]
        public string Comrazsoc { get; set; } = string.Empty;

        [JsonPropertyName("comruc")]
        public string Comruc { get; set; } = string.Empty;

        [JsonPropertyName("comdom")]
        public string Comdom { get; set; } = string.Empty;

        [JsonPropertyName("comtel")]
        public string Comtel { get; set; } = string.Empty;

        [JsonPropertyName("comfax")]
        public string Comfax { get; set; } = string.Empty;

        [JsonPropertyName("comalias")]
        public string Comalias { get; set; } = string.Empty;

        [JsonPropertyName("broker")]
        public bool Broker { get; set; }

        [JsonPropertyName("cod_srvcompanias")]
        public string Cod_srvcompanias { get; set; } = string.Empty;

        [JsonPropertyName("no_utiles")]
        public string No_utiles { get; set; } = string.Empty;

        [JsonPropertyName("paq_dias")]
        public int Paq_dias { get; set; }

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

        [JsonPropertyName("comlog")]
        public string Comlog { get; set; } = string.Empty;

        [JsonPropertyName("comsumodia")]
        public string Comsumodia { get; set; } = string.Empty;

        /// <summary>
        /// En Velneo todas las compañías que devuelve están activas
        /// </summary>
        public bool Activo => true;
    }
}