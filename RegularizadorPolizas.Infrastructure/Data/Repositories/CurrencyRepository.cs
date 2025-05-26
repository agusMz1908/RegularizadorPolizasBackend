using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Currency> GetByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Codigo == codigo && c.Activo);
        }

        public async Task<Currency> GetBySimboloAsync(string simbolo)
        {
            if (string.IsNullOrWhiteSpace(simbolo))
                return null;

            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Simbolo == simbolo && c.Activo);
        }

        public async Task<IEnumerable<Currency>> GetActiveCurrenciesAsync()
        {
            return await _context.Currencies
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            return await _context.Currencies
                .AnyAsync(c => c.Codigo == codigo);
        }

        public async Task<Currency> GetDefaultCurrencyAsync()
        {
            // Retorna el Peso Uruguayo como moneda por defecto
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Codigo == "UYU" && c.Activo) ??
                   await _context.Currencies
                .FirstOrDefaultAsync(c => c.Activo);
        }

        public override async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return await _context.Currencies
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public override async Task<Currency> GetByIdAsync(int id)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }
    }
}