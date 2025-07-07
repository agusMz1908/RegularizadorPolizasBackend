namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaProcessResultDto
    {
        public PolizaDto PolizaData { get; set; }
        public List<string> ValidationWarnings { get; set; } = new List<string>();
        public bool RequiresUserReview { get; set; }
        public bool ReadyForSubmission { get; set; }
    }
}