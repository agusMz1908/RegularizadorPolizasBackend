using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoCalidad
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("caldsc")]
        public string Caldsc { get; set; } = string.Empty;

        [JsonPropertyName("calcod")]
        public string Calcod { get; set; } = string.Empty;
    }

    public class VelneoCalidadesResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("calidades")]
        public List<VelneoCalidad> Calidades { get; set; } = new();
    }
}