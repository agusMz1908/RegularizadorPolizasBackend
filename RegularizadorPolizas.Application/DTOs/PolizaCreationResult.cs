namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaCreationResult
    {
        public bool Success { get; set; }
        public PolizaDto? Poliza { get; set; }
        public string Source { get; set; } = string.Empty; // "Velneo", "Local", "Both"
        public string Message { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long ProcessingTimeMs { get; set; }
        public int? DocumentId { get; set; } // Si fue creada desde un documento
        public DocumentValidationResult? ValidationResult { get; set; }

        // Información específica del proceso híbrido
        public bool UsedFallback { get; set; }
        public string? FallbackReason { get; set; }
        public Dictionary<string, object> AdditionalMetadata { get; set; } = new();

        // Información de sincronización
        public SyncStatus LocalSync { get; set; } = SyncStatus.NotApplicable;
        public SyncStatus VelneoSync { get; set; } = SyncStatus.NotApplicable;
        public string? LocalSyncError { get; set; }
        public string? VelneoSyncError { get; set; }

        public bool HasWarnings => Warnings.Any();
        public bool HasErrors => Errors.Any();
        public bool IsPartialSuccess => Success && (HasWarnings || UsedFallback);

        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning))
            {
                Warnings.Add(warning);
            }
        }

        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                Errors.Add(error);
                Success = false;
            }
        }

        public void SetProcessingTime(DateTime startTime)
        {
            ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
        }

        public string GetStatusSummary()
        {
            if (!Success)
                return $"Error: {string.Join(", ", Errors)}";

            if (IsPartialSuccess)
            {
                var issues = new List<string>();
                if (HasWarnings) issues.Add($"{Warnings.Count} advertencia(s)");
                if (UsedFallback) issues.Add("usó fallback");
                return $"Éxito parcial: {string.Join(", ", issues)}";
            }

            return "Éxito completo";
        }
    }

    public enum SyncStatus
    {
        NotApplicable = 0,  // No se requiere sincronización
        Pending = 1,        // Pendiente de sincronización
        Success = 2,        // Sincronización exitosa
        Failed = 3,         // Falló la sincronización
        Skipped = 4         // Se omitió la sincronización
    }

    public class PolicyCreationContext
    {
        public int? DocumentId { get; set; }
        public string? DocumentFileName { get; set; }
        public bool FromDocumentExtraction { get; set; } = false;
        public DateTime? DocumentProcessedAt { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string CreationMethod { get; set; } = "Manual"; // "Manual", "DocumentExtraction", "API", "Import"
        public Dictionary<string, object> ExtractionMetadata { get; set; } = new();
        public bool RequiresReview { get; set; } = false;
        public decimal? ExtractionConfidence { get; set; }
    }

    public class PolizaCreationRequest
    {
        public PolizaDto Poliza { get; set; } = new();
        public PolicyCreationContext Context { get; set; } = new();
        public bool ForceVelneo { get; set; } = false;
        public bool ForceLocal { get; set; } = false;
        public bool SkipValidation { get; set; } = false;
        public bool EnableSync { get; set; } = true;
        public Dictionary<string, object> AdditionalOptions { get; set; } = new();
    }

    public class BatchPolizaCreationResult
    {
        public int TotalAttempted { get; set; }
        public int SuccessCount { get; set; }
        public int PartialSuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<PolizaCreationResult> Results { get; set; } = new();
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public long TotalProcessingTimeMs { get; set; }
        public List<string> GeneralErrors { get; set; } = new();

        public double SuccessRate => TotalAttempted > 0 ? (double)SuccessCount / TotalAttempted * 100 : 0;
        public bool AllSucceeded => ErrorCount == 0 && PartialSuccessCount == 0;
        public bool HasErrors => ErrorCount > 0 || GeneralErrors.Any();

        public void Complete()
        {
            CompletedAt = DateTime.UtcNow;
            TotalProcessingTimeMs = (long)(CompletedAt.Value - StartedAt).TotalMilliseconds;
        }

        public string GetSummary()
        {
            return $"Procesadas: {TotalAttempted}, Exitosas: {SuccessCount}, Parciales: {PartialSuccessCount}, Errores: {ErrorCount} (Tasa de éxito: {SuccessRate:F1}%)";
        }
    }
}