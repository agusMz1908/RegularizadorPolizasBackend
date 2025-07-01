using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;

namespace RegularizadorPolizas.Infrastructure.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Currency> GetByCodigoAsync(string codigo)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Codigo == codigo || c.Moneda == codigo);
        }

        public async Task<Currency> GetBySimboloAsync(string simbolo)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Simbolo == simbolo);
        }

        public async Task<IEnumerable<Currency>> GetActiveCurrenciesAsync()
        {
            return await _context.Currencies
                .Where(c => c.Activo)
                .OrderBy(c => c.Moneda)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo)
        {
            return await _context.Currencies
                .AnyAsync(c => c.Codigo == codigo || c.Moneda == codigo);
        }

        public async Task<Currency> GetDefaultCurrencyAsync()
        {
            var defaultCurrency = await _context.Currencies
                .Where(c => c.Activo && (c.Codigo == "UYU" || c.Moneda == "UYU"))
                .FirstOrDefaultAsync();

            if (defaultCurrency == null)
            {
                defaultCurrency = await _context.Currencies
                    .Where(c => c.Activo)
                    .OrderBy(c => c.Moneda)
                    .FirstOrDefaultAsync();
            }

            return defaultCurrency;
        }
    }
}