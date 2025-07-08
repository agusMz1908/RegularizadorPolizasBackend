using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoCliente
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("clinro")]
        public int Clinro { get; set; }

        [JsonPropertyName("clinom")]
        public string Clinom { get; set; } = string.Empty;

        [JsonPropertyName("cliape")]
        public string? Cliape { get; set; }

        [JsonPropertyName("cliruc")]
        public string? Cliruc { get; set; }

        [JsonPropertyName("cliced")]
        public string? Cliced { get; set; }

        [JsonPropertyName("clitel")]
        public string? Clitel { get; set; }

        [JsonPropertyName("cliemail")]
        public string? Cliemail { get; set; }

        [JsonPropertyName("clidir")]
        public string? Clidir { get; set; }

        [JsonPropertyName("cliciu")]
        public string? Cliciu { get; set; }

        [JsonPropertyName("clidep")]
        public string? Clidep { get; set; }

        [JsonPropertyName("clipai")]
        public string? Clipai { get; set; }

        [JsonPropertyName("cliobse")]
        public string? Cliobse { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("fechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        [JsonPropertyName("tipoDocumento")]
        public string? TipoDocumento { get; set; }
    }

    public class VelneoClientesResponse
    {
        [JsonPropertyName("clientes")]
        public List<VelneoCliente> Clientes { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoClienteResponse
    {
        [JsonPropertyName("cliente")]
        public VelneoCliente Cliente { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}