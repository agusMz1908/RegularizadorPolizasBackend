using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models
{
    public class VelneoPoliza
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; } = string.Empty;

        [JsonPropertyName("clienteId")]
        public int ClienteId { get; set; }

        [JsonPropertyName("companiaId")]
        public int CompaniaId { get; set; }

        [JsonPropertyName("seccionId")]
        public int SeccionId { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("vigenciaDesde")]
        public DateTime VigenciaDesde { get; set; }

        [JsonPropertyName("vigenciaHasta")]
        public DateTime VigenciaHasta { get; set; }

        [JsonPropertyName("suma")]
        public decimal Suma { get; set; }

        [JsonPropertyName("premio")]
        public decimal Premio { get; set; }

        [JsonPropertyName("comision")]
        public decimal Comision { get; set; }

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "UYU";

        [JsonPropertyName("observaciones")]
        public string? Observaciones { get; set; }

        [JsonPropertyName("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("fechaModificacion")]
        public DateTime? FechaModificacion { get; set; }

        [JsonPropertyName("usuarioCreacion")]
        public string? UsuarioCreacion { get; set; }

        [JsonPropertyName("usuarioModificacion")]
        public string? UsuarioModificacion { get; set; }

        [JsonPropertyName("brokerId")]
        public int? BrokerId { get; set; }

        [JsonPropertyName("endoso")]
        public int? Endoso { get; set; }

        [JsonPropertyName("certificado")]
        public string? Certificado { get; set; }

        [JsonPropertyName("deducible")]
        public decimal? Deducible { get; set; }

        [JsonPropertyName("descuento")]
        public decimal? Descuento { get; set; }

        [JsonPropertyName("impuestos")]
        public decimal? Impuestos { get; set; }

        [JsonPropertyName("activa")]
        public bool Activa { get; set; } = true;

        [JsonPropertyName("cliente")]
        public VelneoClienteInfo? Cliente { get; set; }

        [JsonPropertyName("compania")]
        public VelneoCompaniaInfo? Compania { get; set; }

        [JsonPropertyName("seccion")]
        public VelneoSeccionInfo? Seccion { get; set; }
    }

    public class VelneoClienteInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("documento")]
        public string? Documento { get; set; }

        [JsonPropertyName("tipoDocumento")]
        public string? TipoDocumento { get; set; }
    }

    public class VelneoCompaniaInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }
    }

    public class VelneoSeccionInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("icono")]
        public string? Icono { get; set; }
    }
}