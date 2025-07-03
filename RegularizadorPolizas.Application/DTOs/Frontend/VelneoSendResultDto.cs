namespace RegularizadorPolizas.Application.DTOs.Frontend
{
    public class VelneoSendResultDto
    {
        public bool Success { get; set; }
        public string VelneoPolizaId { get; set; }
        public int? LocalPolizaId { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public DateTime ProcessedAt { get; set; } = DateTime.Now;
        public string TransactionId { get; set; } // Para tracking
        public bool RequiereVerificacionManual { get; set; }
    }
}