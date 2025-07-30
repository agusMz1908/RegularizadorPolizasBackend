using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{

    public class VelneoTarifa
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("companias")]
        public int Companias { get; set; }

        [JsonPropertyName("tarnom")]
        public string TarNom { get; set; } = string.Empty;

        [JsonPropertyName("tarcod")]
        public string? TarCod { get; set; }

        [JsonPropertyName("tardsc")]
        public string? TarDsc { get; set; }
    }

    public class VelneoTarifasResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("tarifas")]
        public List<VelneoTarifa> Tarifas { get; set; } = new();
    }
}