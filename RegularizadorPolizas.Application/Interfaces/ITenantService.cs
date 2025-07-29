using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ITenantService
    {
        // Métodos existentes
        string GetCurrentTenantId();
        int GetCurrentUserId();
        Task<ApiKey> GetTenantConfigurationAsync(string tenantId);
        Task<ApiKey> GetCurrentTenantConfigurationAsync();
        Task<bool> ValidateTenantAccessAsync(string tenantId, int userId);
        Task<bool> ValidateCurrentUserTenantAccessAsync(string tenantId);
        Task<string> BuildVelneoUrlAsync(string tenantId, string endpoint);
        Task<string> BuildCurrentTenantVelneoUrlAsync(string endpoint);
        Task<IEnumerable<ApiKey>> GetAllTenantsAsync();
        Task<bool> TenantExistsAsync(string tenantId);

        // Nuevos métodos para el switch
        Task<bool> UpdateTenantModeAsync(string tenantId, string mode, string? reason = null);
        Task<bool> UpdateCurrentTenantModeAsync(string mode, string? reason = null);
        Task<ApiKey> CreateTenantConfigurationAsync(CreateTenantConfigDto config);
        Task<bool> DeleteTenantConfigurationAsync(string tenantId);
        Task<ApiKey> UpdateTenantConfigurationAsync(string tenantId, UpdateTenantConfigDto config);
        Task<bool> TestTenantConnectivityAsync(string tenantId);
        Task<bool> TestCurrentTenantConnectivityAsync();
        Task<IEnumerable<ApiKey>> GetActiveTenantsByModeAsync(string mode);
        Task<TenantUsageStatsDto> GetTenantUsageStatsAsync(string tenantId, DateTime? fromDate = null);
        Task<TenantUsageStatsDto> GetCurrentTenantUsageStatsAsync(DateTime? fromDate = null);
        Task<TenantConfigurationDto> GetCurrentTenantConfigurationDtoAsync();
    }

    // DTOs para las nuevas funcionalidades
    public class CreateTenantConfigDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Mode { get; set; } = "VELNEO";
        public string Environment { get; set; } = "Production";
        public string? Description { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public int? MaxRequestsPerMinute { get; set; }
        public string? ContactEmail { get; set; }
        public bool EnableLogging { get; set; } = true;
        public bool EnableRetries { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
        public string ApiVersion { get; set; } = "v1";
    }

    public class UpdateTenantConfigDto
    {
        public string? Key { get; set; }
        public string? BaseUrl { get; set; }
        public string? Mode { get; set; }
        public string? Environment { get; set; }
        public string? Description { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public int? MaxRequestsPerMinute { get; set; }
        public string? ContactEmail { get; set; }
        public bool? EnableLogging { get; set; }
        public bool? EnableRetries { get; set; }
        public int? TimeoutSeconds { get; set; }
        public string? ApiVersion { get; set; }
        public bool? Activo { get; set; }
    }

    public class TenantUsageStatsDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOperations { get; set; }
        public int VelneoOperations { get; set; }
        public int LocalOperations { get; set; }
        public int FailoverEvents { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public int ErrorCount { get; set; }
        public Dictionary<string, int> OperationsByEntity { get; set; } = new();
        public Dictionary<string, int> OperationsByType { get; set; } = new();
        public DateTime? LastModeChange { get; set; }
    }
}