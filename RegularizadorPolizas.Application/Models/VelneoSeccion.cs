using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoSeccion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("secdsc")]
        public string Secdsc { get; set; } = string.Empty;

        [JsonPropertyName("seccod")]
        public string Seccod { get; set; } = string.Empty;

        [JsonPropertyName("comcod")]
        public int? CompanyId { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;
    }

    public class VelneoSeccionesResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("secciones")]
        public List<VelneoSeccion> Secciones { get; set; } = new();
    }

    public class VelneoSeccionResponse
    {
        [JsonPropertyName("seccion")]
        public VelneoSeccion Seccion { get; set; } = new();
    }
}