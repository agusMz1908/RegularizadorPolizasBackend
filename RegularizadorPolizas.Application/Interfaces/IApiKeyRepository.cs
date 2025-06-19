using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IApiKeyRepository
    {
        Task<ApiKey?> GetByKeyAsync(string key);
        Task<IEnumerable<ApiKey>> GetAllAsync();
        Task AddAsync(ApiKey apiKey);
        Task UpdateAsync(ApiKey apiKey);
        Task DeleteAsync(int id);

        // Nuevos métodos para multi-tenant
        Task<ApiKey?> GetByTenantIdAsync(string tenantId);
        Task<ApiKey?> GetByIdAsync(int id);
        Task<IEnumerable<ApiKey>> GetActiveApiKeysAsync();
        Task<IEnumerable<ApiKey>> GetByEnvironmentAsync(string environment);
        Task<bool> ExistsByTenantIdAsync(string tenantId);
        Task<bool> ExistsByKeyAsync(string key, int? excludeId = null);

        Task UpdateLastUsedAsync(int id);
        Task<IEnumerable<ApiKey>> GetExpiredApiKeysAsync();
        Task<IEnumerable<ApiKey>> GetUnusedApiKeysAsync(int daysUnused);

        Task<IEnumerable<ApiKey>> SearchAsync(string searchTerm);
        Task SaveChangesAsync();
    }
}