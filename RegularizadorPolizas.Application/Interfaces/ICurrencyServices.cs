using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<CurrencyDto> GetCurrencyByIdAsync(int id);
        Task<CurrencyDto> GetCurrencyByCodigoAsync(string codigo);
        Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync();
        Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync();
        Task<CurrencyDto> GetDefaultCurrencyAsync();
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task DeleteCurrencyAsync(int id); // Soft delete
        Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
        Task<bool> ExistsBySimboloAsync(string simbolo, int? excludeId = null);
        Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm);
    }
}