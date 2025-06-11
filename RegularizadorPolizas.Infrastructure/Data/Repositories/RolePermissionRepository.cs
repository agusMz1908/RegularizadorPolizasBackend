using Microsoft.EntityFrameworkCore;
using Polly;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;

namespace RegularizadorPolizas.Infrastructure.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Include(rp => rp.GrantedByUser)
                .Where(rp => rp.RoleId == roleId)
                .OrderBy(rp => rp.Permission.Resource)
                .ThenBy(rp => rp.Permission.Action)
                .ToListAsync();
        }

        public async Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(int permissionId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Include(rp => rp.GrantedByUser)
                .Where(rp => rp.PermissionId == permissionId)
                .OrderBy(rp => rp.Role.Name)
                .ToListAsync();
        }

        public async Task<RolePermission?> GetByRoleAndPermissionAsync(int roleId, int permissionId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Include(rp => rp.GrantedByUser)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public async Task<IEnumerable<RolePermission>> GetActiveRolePermissionsAsync(int roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Include(rp => rp.GrantedByUser)
                .Where(rp => rp.RoleId == roleId && rp.IsActive && rp.Permission.IsActive)
                .OrderBy(rp => rp.Permission.Resource)
                .ThenBy(rp => rp.Permission.Action)
                .ToListAsync();
        }
    }
}