namespace RegularizadorPolizas.Application.DTOs.Velneo
{
    public class VelneoSubmissionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string VelneoPolizaId { get; set; }
        public int? LocalTrackingId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}