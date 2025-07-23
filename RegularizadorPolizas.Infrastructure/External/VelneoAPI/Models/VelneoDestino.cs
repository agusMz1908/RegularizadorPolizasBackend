using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoDestino
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("desnom")]
        public string Desnom { get; set; } = string.Empty;

        [JsonPropertyName("descod")]
        public string Descod { get; set; } = string.Empty;
    }

    public class VelneoDestinosResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("destinos")]
        public List<VelneoDestino> Destinos { get; set; } = new();
    }
}