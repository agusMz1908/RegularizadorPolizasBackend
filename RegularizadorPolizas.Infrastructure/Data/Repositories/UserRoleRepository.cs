using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories 
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

        public async Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds, int? assignedByUserId = null)
        {
            var currentRoles = _context.UserRoles.Where(ur => ur.UserId == userId && ur.IsActive);
            foreach (var ur in currentRoles)
            {
                ur.IsActive = false;
            }

            foreach (var roleId in roleIds.Distinct())
            {
                var existing = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);

                if (existing == null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        IsActive = true,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = assignedByUserId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserRoleAsync(int userId, int roleId, int? removedByUserId = null)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);

            if (userRole != null)
            {
                userRole.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}