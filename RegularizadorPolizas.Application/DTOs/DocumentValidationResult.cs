using System.Collections.Generic;

namespace RegularizadorPolizas.Application.DTOs
{
    public class DocumentValidationResult
    {
        public bool IsValid { get; set; }
        public bool RequiresReview { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public string Summary => GenerateSummary();

        private string GenerateSummary()
        {
            if (IsValid && !RequiresReview)
                return "Documento procesado correctamente sin problemas";

            if (IsValid && RequiresReview)
                return $"Documento procesado con {Warnings.Count} advertencia(s)";

            return $"Documento con {Errors.Count} error(es) crítico(s) y {Warnings.Count} advertencia(s)";
        }
    }
}