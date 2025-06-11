using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories 
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Permission?> GetByNameAsync(string name)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Resource)
                .ThenBy(p => p.Action)
                .ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetByResourceAsync(string resource)
        {
            return await _context.Permissions
                .Where(p => p.Resource == resource && p.IsActive)
                .OrderBy(p => p.Action)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Permissions.Where(p => p.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}