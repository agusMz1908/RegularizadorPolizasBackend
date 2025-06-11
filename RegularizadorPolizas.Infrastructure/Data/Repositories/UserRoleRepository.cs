using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;

namespace RegularizadorPolizas.Infrastructure.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Include(ur => ur.AssignedByUser)
                .Where(ur => ur.UserId == userId)
                .OrderByDescending(ur => ur.AssignedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Include(ur => ur.AssignedByUser)
                .Where(ur => ur.RoleId == roleId)
                .OrderByDescending(ur => ur.AssignedAt)
                .ToListAsync();
        }

        public async Task<UserRole?> GetByUserAndRoleAsync(int userId, int roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Include(ur => ur.AssignedByUser)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        public async Task<IEnumerable<UserRole>> GetActiveUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Include(ur => ur.AssignedByUser)
                .Where(ur => ur.UserId == userId && ur.IsActive && ur.Role.IsActive)
                .OrderBy(ur => ur.Role.Name)
                .ToListAsync();
        }
    }
}