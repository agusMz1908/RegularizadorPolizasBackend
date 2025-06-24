using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.DTOs.External.Velneo
{
    public class VelneoBrokerResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("corredores")]
        public List<VelneoBrokerDto> Corredores { get; set; } = new List<VelneoBrokerDto>();
    }
    public class VelneoBrokerDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; } = string.Empty;

        [JsonPropertyName("foto")]
        public string Foto { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        public bool Activo => true;
    }
}