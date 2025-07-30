using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoMoneda
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("moneda")]
        public string? Moneda { get; set; } 
    }

    public class VelneeMonedasResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("monedas")]
        public List<VelneoMoneda> Monedas { get; set; } = new List<VelneoMoneda>();
    }
}