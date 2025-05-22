namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaExtractionResult
    {
        public PolizaDto PolizaData { get; set; }
        public int DocumentId { get; set; }
        public decimal? ExtractionConfidence { get; set; }
        public DocumentValidationResult ValidationResult { get; set; }
        public bool RequiresReview { get; set; }
        public string ProcessingStatus { get; set; } = "SUCCESS";
        public string Message { get; set; }
    }
}