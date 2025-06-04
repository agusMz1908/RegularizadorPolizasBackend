namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaCreationResult
    {
        public bool Success { get; set; }
        public PolizaDto? PolizaDto { get; set; }
        public ProcessDocumentDto? DocumentDto { get; set; }
        public long ProcessingTimeMs { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public object? AdditionalData { get; set; }
    }
}