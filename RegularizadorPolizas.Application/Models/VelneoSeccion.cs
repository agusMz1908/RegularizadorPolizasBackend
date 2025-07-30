using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    /// <summary>
    /// ✅ VelneoSeccion - CORREGIDA para usar "seccion" en lugar de "secdsc"
    /// </summary>
    public class VelneoSeccion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("seccion")]  // ✅ CAMBIO: era "secdsc", ahora "seccion"
        public string Seccion { get; set; } = string.Empty;

        [JsonPropertyName("icono")]  // ✅ NUEVO: campo icono que viene de Velneo
        public string Icono { get; set; } = string.Empty;

        [JsonPropertyName("seccod")]  // ✅ Campo código (puede estar o no)
        public string? Seccod { get; set; } = string.Empty;

        [JsonPropertyName("comcod")]  // ✅ Campo compañía (puede estar o no)
        public int? CompanyId { get; set; }

        [JsonPropertyName("activo")]  // ✅ Campo activo (puede no venir, asumir true)
        public bool Activo { get; set; } = true;

        // ✅ Propiedades de compatibilidad para mappers existentes
        public string Secdsc => Seccion;  // Alias para mantener compatibilidad con código existente
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