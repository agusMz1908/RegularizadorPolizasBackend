using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoCobertura
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("cobdsc")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("cobcod")]
        public string? Codigo { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("observaciones")]
        public string? Observaciones { get; set; }
    }

    public class VelneoCoberturasResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("coberturas")]
        public List<VelneoCobertura> Coberturas { get; set; } = new();
    }
}