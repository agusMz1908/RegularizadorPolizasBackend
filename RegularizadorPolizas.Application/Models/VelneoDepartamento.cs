using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoDepartamento
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("bonificacion_interior")]
        public decimal BonificacionInterior { get; set; }

        [JsonPropertyName("codigo_sc")]
        public string CodigoSC { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;
    }

    public class VelneoDepartamentosResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("departamentos")]
        public List<VelneoDepartamento> Departamentos { get; set; } = new();
    }
}