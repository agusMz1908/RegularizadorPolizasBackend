using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Company> GetByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Codigo == codigo && c.Activo);
        }

        public async Task<Company> GetByAliasAsync(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                return null;

            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Alias == alias && c.Activo);
        }

        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
        {
            return await _context.Companies
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            return await _context.Companies
                .AnyAsync(c => c.Codigo == codigo);
        }

        public override async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public override async Task<Company> GetByIdAsync(int id)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }
    }
}