using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class BrokerRepository : GenericRepository<Broker>, IBrokerRepository
    {
        public BrokerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Broker> GetByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return await _context.Brokers
                .FirstOrDefaultAsync(b => b.Codigo == codigo && b.Activo);
        }

        public async Task<Broker> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Brokers
                .FirstOrDefaultAsync(b => b.Email.ToLower() == email.ToLower() && b.Activo);
        }

        public async Task<IEnumerable<Broker>> GetActiveBrokersAsync()
        {
            return await _context.Brokers
                .Where(b => b.Activo)
                .OrderBy(b => b.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Broker>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveBrokersAsync();

            var normalizedSearchTerm = searchTerm.Trim().ToLower();

            return await _context.Brokers
                .Where(b => b.Activo &&
                           (b.Nombre.ToLower().Contains(normalizedSearchTerm) ||
                            b.Codigo.ToLower().Contains(normalizedSearchTerm)))
                .OrderBy(b => b.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            return await _context.Brokers
                .AnyAsync(b => b.Codigo == codigo);
        }

        public async Task<IEnumerable<Broker>> GetBrokersWithPoliciesAsync()
        {
            return await _context.Brokers
                .Where(b => b.Activo)
                .Include(b => b.Polizas.Where(p => p.Activo))
                .OrderBy(b => b.Nombre)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Broker>> GetAllAsync()
        {
            return await _context.Brokers
                .Where(b => b.Activo)
                .OrderBy(b => b.Nombre)
                .ToListAsync();
        }

        public override async Task<Broker> GetByIdAsync(int id)
        {
            return await _context.Brokers
                .FirstOrDefaultAsync(b => b.Id == id && b.Activo);
        }
    }
}