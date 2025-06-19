
using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;

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
            .FirstOrDefaultAsync(a => a.Key == key && a.Activo && (a.FechaExpiracion == null || a.FechaExpiracion > DateTime.UtcNow));
    }

    public async Task AddAsync(ApiKey apiKey)
    {
        await _context.ApiKeys.AddAsync(apiKey);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var apiKey = await _context.ApiKeys.FindAsync(id);
        if (apiKey != null)
        {
            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ApiKey>> GetAllAsync()
    {
        return await _context.ApiKeys.ToListAsync();
    }

    Task<RegularizadorPolizas.Domain.Entities.ApiKey?> IApiKeyRepository.GetByKeyAsync(string key)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<RegularizadorPolizas.Domain.Entities.ApiKey>> IApiKeyRepository.GetAllAsync()
    {
        throw new NotImplementedException();
    }
}