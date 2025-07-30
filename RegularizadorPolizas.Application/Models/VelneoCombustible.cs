using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoCombustible
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class VelneoCombustiblesResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("combustibles")]
        public List<VelneoCombustible> Combustibles { get; set; } = new();
    }
}
