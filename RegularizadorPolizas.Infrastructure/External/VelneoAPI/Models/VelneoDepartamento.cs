using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoDepartamento
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("dptnom")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("dptbonint")]
        public string? BonificacionInteriorStr { get; set; }

        [JsonPropertyName("sc_cod")]
        public string? CodigoSC { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;
        public decimal? BonificacionInterior =>
            decimal.TryParse(BonificacionInteriorStr, out var value) ? value : null;
    }

    public class VelneoDepartamentosResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("departamentos")]
        public List<VelneoDepartamento> Departamentos { get; set; } = new();
    }
}