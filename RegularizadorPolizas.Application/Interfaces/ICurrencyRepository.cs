namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        Task<Currency> GetByCodigoAsync(string codigo);
        Task<Currency> GetBySimboloAsync(string simbolo);
        Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();
        Task<bool> ExistsByCodigoAsync(string codigo);
        Task<Currency> GetDefaultCurrencyAsync(); 
    }
}