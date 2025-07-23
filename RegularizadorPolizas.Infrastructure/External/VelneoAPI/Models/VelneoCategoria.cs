using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoCategoria
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("catdsc")]
        public string Catdsc { get; set; } = string.Empty;

        [JsonPropertyName("catcod")]
        public string Catcod { get; set; } = string.Empty;
    }

    public class VelneoCategoriasResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("categorias")]
        public List<VelneoCategoria> Categorias { get; set; } = new();
    }
}