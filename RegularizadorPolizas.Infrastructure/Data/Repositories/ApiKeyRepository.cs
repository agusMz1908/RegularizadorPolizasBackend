using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly ApplicationDbContext _context;

        public ApiKeyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiKey?> GetByKeyAsync(string key)
        {
            return await _context.ApiKeys
                .FirstOrDefaultAsync(a => a.Key == key &&
                                         a.Activo &&
                                         (a.FechaExpiracion == null || a.FechaExpiracion > DateTime.UtcNow));
        }

        public async Task<ApiKey?> GetByTenantIdAsync(string tenantId)
        {
            return await _context.ApiKeys
                .FirstOrDefaultAsync(a => a.TenantId == tenantId &&
                                         a.Activo &&
                                         (a.FechaExpiracion == null || a.FechaExpiracion > DateTime.UtcNow));
        }

        public async Task<ApiKey?> GetByIdAsync(int id)
        {
            return await _context.ApiKeys.FindAsync(id);
        }

        public async Task<IEnumerable<ApiKey>> GetAllAsync()
        {
            return await _context.ApiKeys
                .OrderBy(a => a.TenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApiKey>> GetActiveApiKeysAsync()
        {
            return await _context.ApiKeys
                .Where(a => a.Activo && (a.FechaExpiracion == null || a.FechaExpiracion > DateTime.UtcNow))
                .OrderBy(a => a.TenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApiKey>> GetByEnvironmentAsync(string environment)
        {
            return await _context.ApiKeys
                .Where(a => a.Environment == environment && a.Activo)
                .OrderBy(a => a.TenantId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByTenantIdAsync(string tenantId)
        {
            return await _context.ApiKeys
                .AnyAsync(a => a.TenantId == tenantId);
        }

        public async Task<bool> ExistsByKeyAsync(string key, int? excludeId = null)
        {
            var query = _context.ApiKeys.Where(a => a.Key == key);

            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(ApiKey apiKey)
        {
            apiKey.FechaCreacion = DateTime.UtcNow;
            apiKey.FechaModificacion = DateTime.UtcNow;

            await _context.ApiKeys.AddAsync(apiKey);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApiKey apiKey)
        {
            apiKey.FechaModificacion = DateTime.UtcNow;

            _context.ApiKeys.Update(apiKey);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey != null)
            {
                apiKey.Activo = false;
                apiKey.FechaModificacion = DateTime.UtcNow;

                _context.ApiKeys.Update(apiKey);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLastUsedAsync(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey != null)
            {
                apiKey.LastUsed = DateTime.UtcNow;
                apiKey.FechaModificacion = DateTime.UtcNow;

                _context.ApiKeys.Update(apiKey);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ApiKey>> GetExpiredApiKeysAsync()
        {
            return await _context.ApiKeys
                .Where(a => a.FechaExpiracion.HasValue && a.FechaExpiracion < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApiKey>> GetUnusedApiKeysAsync(int daysUnused)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysUnused);

            return await _context.ApiKeys
                .Where(a => a.Activo &&
                           (a.LastUsed == null || a.LastUsed < cutoffDate))
                .ToListAsync();
        }

        public async Task<IEnumerable<ApiKey>> SearchAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();

            return await _context.ApiKeys
                .Where(a => a.TenantId.ToLower().Contains(term) ||
                           (a.Descripcion != null && a.Descripcion.ToLower().Contains(term)) ||
                           (a.ContactEmail != null && a.ContactEmail.ToLower().Contains(term)))
                .OrderBy(a => a.TenantId)
                .ToListAsync();
        }

        public async Task<ApiKey> CreateAsync(ApiKey apiKey)
        {
            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();
            return apiKey;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}