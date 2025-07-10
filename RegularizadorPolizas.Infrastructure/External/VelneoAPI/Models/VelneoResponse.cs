using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{

    public class VelneoPolizaResponse
    {
        [JsonPropertyName("poliza")]
        public VelneoPoliza Poliza { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoPolizasResponse
    {
        [JsonPropertyName("contratos")]
        public List<VelneoPoliza>? Polizas { get; set; } = new();

        [JsonPropertyName("total_count")]
        public int? TotalCount { get; set; } 

        [JsonPropertyName("total")]
        public int? Total { get; set; }      

        [JsonPropertyName("success")]
        public bool? Success { get; set; }   

        [JsonPropertyName("message")]
        public string? Message { get; set; } = string.Empty;

        // ✅ Agregar esta propiedad que falta
        [JsonPropertyName("has_more_data")]
        public bool? HasMoreData { get; set; }
    }

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

    public class VelneoClientResponse
    {
        [JsonPropertyName("cliente")]
        public VelneoCliente Cliente { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoClientsResponse
    {
        [JsonPropertyName("clientes")]
        public List<VelneoCliente> Clientes { get; set; } = new();

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoCompanyResponse
    {
        [JsonPropertyName("compania")]
        public VelneoCompany Compania { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoHealthResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}