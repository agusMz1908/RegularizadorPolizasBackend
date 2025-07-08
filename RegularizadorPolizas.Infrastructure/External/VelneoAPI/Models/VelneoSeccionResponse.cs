using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoSeccionResponse
    {
        [JsonPropertyName("seccion")]
        public VelneoSeccion Seccion { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoSeccionesResponse
    {
        [JsonPropertyName("secciones")]
        public List<VelneoSeccion> Secciones { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoSeccionesLookupResponse
    {
        [JsonPropertyName("secciones")]
        public List<VelneoSeccionLookup> Secciones { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoSeccion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("icono")]
        public string Icono { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("fechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        // Campos adicionales que puede tener Velneo
        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("orden")]
        public int? Orden { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }
    }

    public class VelneoSeccionLookup
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("icono")]
        public string Icono { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }
    }
}