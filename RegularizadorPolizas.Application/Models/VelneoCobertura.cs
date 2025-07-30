using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoCobertura
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

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
        public int Total_Count { get; set; }

        [JsonPropertyName("coberturas")]
        public List<VelneoCobertura> Coberturas { get; set; } = new();
    }
}