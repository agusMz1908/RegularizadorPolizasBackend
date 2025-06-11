using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Activo)
                .OrderBy(u => u.Nombre)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithRolesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> SearchByNameOrEmailAsync(string searchTerm)
        {
            var normalizedSearchTerm = searchTerm.ToLower();
            return await _context.Users
                .Where(u => u.Nombre.ToLower().Contains(normalizedSearchTerm) ||
                           u.Email.ToLower().Contains(normalizedSearchTerm))
                .OrderBy(u => u.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            var query = _context.Users.Where(u => u.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}