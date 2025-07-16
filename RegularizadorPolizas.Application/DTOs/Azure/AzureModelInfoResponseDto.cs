namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class AzureModelInfoResponseDto
    {
        public string ModelId { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? WorkingApiUrl { get; set; }
        public string? HttpStatus { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<string>? DocTypes { get; set; }
        public string? ApiVersion { get; set; }
        public string? Warning { get; set; }
        public string? Message { get; set; }
        public DateTime ConsultaTimestamp { get; set; } = DateTime.UtcNow;
        public bool EstaActivo => Status.Contains("activo", StringComparison.OrdinalIgnoreCase) ||
                                 Status.Contains("encontrado", StringComparison.OrdinalIgnoreCase);

        public bool TieneAdvertencias => !string.IsNullOrEmpty(Warning);

        public string EstadoSimplificado => EstaActivo ? "Activo" : "Inactivo";

        public AzureModelHealthDto Health => new()
        {
            EstaOperativo = EstaActivo,
            TieneConexion = !string.IsNullOrEmpty(WorkingApiUrl),
            UltimaVerificacion = ConsultaTimestamp,
            Mensaje = Status,
            Nivel = EstaActivo ? "success" : TieneAdvertencias ? "warning" : "error"
        };
    }

    public class AzureModelHealthDto
    {
        public bool EstaOperativo { get; set; }
        public bool TieneConexion { get; set; }
        public DateTime UltimaVerificacion { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Nivel { get; set; } = "info"; 

        public string IconoEstado => Nivel switch
        {
            "success" => "✅",
            "warning" => "⚠️",
            "error" => "❌",
            _ => "ℹ️"
        };

        public string MensajeCompleto => $"{IconoEstado} {Mensaje}";
    }
}
