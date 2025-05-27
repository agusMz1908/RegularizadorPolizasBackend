using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<CurrencyDto> GetCurrencyByIdAsync(int id);
        Task<CurrencyDto> GetCurrencyByMonedaAsync(string moneda);
        Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync();
        Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync();
        Task<CurrencyDto> GetDefaultCurrencyAsync();
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task DeleteCurrencyAsync(int id);
        Task<bool> ExistsByMonedaAsync(string moneda, int? excludeId = null);
        Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm);
    }
}