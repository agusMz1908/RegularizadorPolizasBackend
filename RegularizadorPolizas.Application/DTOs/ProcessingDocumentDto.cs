namespace RegularizadorPolizas.Application.DTOs
{
    public class ProcessingDocumentDto
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; } = "PENDIENTE";
        public string CurrentStage { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public decimal? ProcessingTime { get; set; }
        public decimal? Cost { get; set; }
        public int? PageCount { get; set; }
        public decimal? ConfidenceLevel { get; set; }
        public string ErrorMessage { get; set; }
        public bool? SentToVelneo { get; set; }
        public DateTime? VelneoSentDate { get; set; }
        public string VelneoResponse { get; set; }
        public long? FileSize { get; set; }
        public string MimeType { get; set; }
        public string FileHash { get; set; }
        public int Priority { get; set; } = 2;
        public int ProcessingAttempts { get; set; } = 0;
        public int MaxAttempts { get; set; } = 3;
        public int? PolicyId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string PolicyNumber { get; set; }
        public bool IsSuccessful => Status == "COMPLETADO" && string.IsNullOrEmpty(ErrorMessage);
        public bool IsPending => Status == "PENDIENTE";
        public bool IsProcessing => Status == "PROCESANDO";
        public bool HasError => Status == "ERROR" || !string.IsNullOrEmpty(ErrorMessage);
        public ProcessingDocumentDto()
        {
            Id = string.Empty;
            FileName = string.Empty;
            FilePath = string.Empty;
            DocumentType = string.Empty;
            Status = "PENDIENTE";
            CompanyCode = string.Empty;
            CompanyName = string.Empty;
            CurrentStage = "pending";
            ErrorMessage = string.Empty;
            VelneoResponse = string.Empty;
            MimeType = string.Empty;
            FileHash = string.Empty;
            UserName = string.Empty;
            PolicyNumber = string.Empty;

            CreationTime = DateTime.Now;
        }
    }
}