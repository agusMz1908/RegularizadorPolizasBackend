using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoBroker
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("fechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("ruc")]
        public string? Ruc { get; set; }

        [JsonPropertyName("contactoPrincipal")]
        public string? ContactoPrincipal { get; set; }

        [JsonPropertyName("telefonoContacto")]
        public string? TelefonoContacto { get; set; }

        [JsonPropertyName("emailContacto")]
        public string? EmailContacto { get; set; }

        [JsonPropertyName("porcentajeComision")]
        public decimal? PorcentajeComision { get; set; }

        [JsonPropertyName("tipoComision")]
        public string? TipoComision { get; set; }

        [JsonPropertyName("limiteCredito")]
        public decimal? LimiteCredito { get; set; }

        [JsonPropertyName("diasCredito")]
        public int? DiasCredito { get; set; }
    }

    public class VelneoBrokerResponse
    {
        [JsonPropertyName("broker")]
        public VelneoBroker Broker { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class VelneoBrokersResponse
    {
        [JsonPropertyName("brokers")]
        public List<VelneoBroker> Brokers { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}