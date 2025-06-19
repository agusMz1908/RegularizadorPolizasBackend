using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ITenantService
    {
        string GetCurrentTenantId();
        Task<ApiKey> GetTenantConfigurationAsync(string tenantId);
        Task<ApiKey> GetCurrentTenantConfigurationAsync();
        Task<bool> ValidateTenantAccessAsync(string tenantId, int userId);
        Task<bool> ValidateCurrentUserTenantAccessAsync(string tenantId);
        int GetCurrentUserId();
        Task<string> BuildVelneoUrlAsync(string tenantId, string endpoint);
        Task<string> BuildCurrentTenantVelneoUrlAsync(string endpoint);
        Task<IEnumerable<ApiKey>> GetAllTenantsAsync();
        Task<bool> TenantExistsAsync(string tenantId);
    }
}