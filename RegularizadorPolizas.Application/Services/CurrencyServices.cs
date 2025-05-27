using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IMapper _mapper;

        public CurrencyService(
            ICurrencyRepository currencyRepository,
            IPolizaRepository polizaRepository,
            IMapper mapper)
        {
            _currencyRepository = currencyRepository ?? throw new ArgumentNullException(nameof(currencyRepository));
            _polizaRepository = polizaRepository ?? throw new ArgumentNullException(nameof(polizaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
        {
            try
            {
                var currencies = await _currencyRepository.GetAllAsync();
                var currenciesDto = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);

                foreach (var currencyDto in currenciesDto)
                {
                    var polizasCount = await GetPolizasCountAsync(currencyDto.Id);
                    currencyDto.TotalPolizas = polizasCount;
                }

                return currenciesDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving currencies: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyDto> GetCurrencyByIdAsync(int id)
        {
            try
            {
                var currency = await _currencyRepository.GetByIdAsync(id);
                if (currency == null)
                    return null;

                var currencyDto = _mapper.Map<CurrencyDto>(currency);
                currencyDto.TotalPolizas = await GetPolizasCountAsync(id);

                return currencyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving currency with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyDto> GetCurrencyByMonedaAsync(string moneda)
        {
            try
            {
                var currency = await _currencyRepository.GetByMonedaAsync(moneda);
                if (currency == null)
                    return null;

                var currencyDto = _mapper.Map<CurrencyDto>(currency);
                currencyDto.TotalPolizas = await GetPolizasCountAsync(currency.Id);

                return currencyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving currency with moneda {moneda}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync()
        {
            try
            {
                var currencies = await _currencyRepository.GetActiveCurrenciesAsync();
                return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving active currencies: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync()
        {
            try
            {
                var currencies = await _currencyRepository.GetActiveCurrenciesAsync();
                return _mapper.Map<IEnumerable<CurrencyLookupDto>>(currencies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving currencies for lookup: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyDto> GetDefaultCurrencyAsync()
        {
            try
            {
                // Buscar Peso Uruguayo como moneda por defecto
                var currency = await _currencyRepository.GetByMonedaAsync("Peso Uruguayo");
                if (currency == null)
                {
                    // Si no existe, tomar la primera activa
                    var currencies = await _currencyRepository.GetActiveCurrenciesAsync();
                    currency = currencies.FirstOrDefault();
                }

                return currency != null ? _mapper.Map<CurrencyDto>(currency) : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving default currency: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currencyDto.Moneda))
                    throw new ArgumentException("Currency name (Moneda) is required");

                if (await ExistsByMonedaAsync(currencyDto.Moneda))
                    throw new ArgumentException($"Currency with name '{currencyDto.Moneda}' already exists");

                var currency = _mapper.Map<Currency>(currencyDto);
                currency.Activo = true;
                currency.FechaCreacion = DateTime.Now;
                currency.FechaModificacion = DateTime.Now;

                var createdCurrency = await _currencyRepository.AddAsync(currency);
                return _mapper.Map<CurrencyDto>(createdCurrency);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating currency: {ex.Message}", ex);
            }
        }

        public async Task UpdateCurrencyAsync(CurrencyDto currencyDto)
        {
            try
            {
                if (currencyDto == null)
                    throw new ArgumentNullException(nameof(currencyDto));

                var existingCurrency = await _currencyRepository.GetByIdAsync(currencyDto.Id);
                if (existingCurrency == null)
                    throw new ApplicationException($"Currency with ID {currencyDto.Id} not found");

                if (await ExistsByMonedaAsync(currencyDto.Moneda, currencyDto.Id))
                    throw new ArgumentException($"Currency with name '{currencyDto.Moneda}' already exists");

                existingCurrency.Moneda = currencyDto.Moneda;
                existingCurrency.Activo = currencyDto.Activo;
                existingCurrency.FechaModificacion = DateTime.Now;

                await _currencyRepository.UpdateAsync(existingCurrency);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating currency: {ex.Message}", ex);
            }
        }

        public async Task DeleteCurrencyAsync(int id)
        {
            try
            {
                var currency = await _currencyRepository.GetByIdAsync(id);
                if (currency == null)
                    throw new ApplicationException($"Currency with ID {id} not found");

                var polizasCount = await GetPolizasCountAsync(id);
                if (polizasCount > 0)
                    throw new ApplicationException($"Cannot delete currency. It has {polizasCount} associated policies");

                currency.Activo = false;
                currency.FechaModificacion = DateTime.Now;
                await _currencyRepository.UpdateAsync(currency);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting currency: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByMonedaAsync(string moneda, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(moneda))
                    return false;

                var currencies = await _currencyRepository.FindAsync(c => c.Moneda.ToLower() == moneda.ToLower());
                return currencies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking currency name existence: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveCurrenciesAsync();

                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                var currencies = await _currencyRepository.FindAsync(c =>
                    c.Activo && c.Moneda.ToLower().Contains(normalizedSearchTerm)
                );

                return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching currencies: {ex.Message}", ex);
            }
        }

        private async Task<int> GetPolizasCountAsync(int currencyId)
        {
            try
            {
                var polizas = await _polizaRepository.FindAsync(p => p.Moncod == currencyId && p.Activo);
                return polizas.Count();
            }
            catch
            {
                return 0;
            }
        }
    }
}