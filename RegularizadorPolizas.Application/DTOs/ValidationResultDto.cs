namespace RegularizadorPolizas.Application.DTOs
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasWarnings => Warnings.Count > 0;
    }
}