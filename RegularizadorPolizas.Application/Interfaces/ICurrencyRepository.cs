using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        Task<Currency> GetByMonedaAsync(string moneda);
        Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();
        Task<bool> ExistsByMonedaAsync(string moneda);
        Task<Currency> GetDefaultCurrencyAsync();
    }
}