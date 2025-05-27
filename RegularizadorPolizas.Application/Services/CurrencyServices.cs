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

        public async Task<CurrencyDto> GetCurrencyByCodigoAsync(string codigo)
        {
            try
            {
                var currency = await _currencyRepository.GetByCodigoAsync(codigo);
                if (currency == null)
                    return null;

                var currencyDto = _mapper.Map<CurrencyDto>(currency);
                currencyDto.TotalPolizas = await GetPolizasCountAsync(currency.Id);

                return currencyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving currency with codigo {codigo}: {ex.Message}", ex);
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
                var currency = await _currencyRepository.GetDefaultCurrencyAsync();
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
                if (string.IsNullOrWhiteSpace(currencyDto.Nombre))
                    throw new ArgumentException("Currency name is required");

                if (string.IsNullOrWhiteSpace(currencyDto.Codigo))
                    throw new ArgumentException("Currency code is required");

                if (await ExistsByCodigoAsync(currencyDto.Codigo))
                    throw new ArgumentException($"Currency with code '{currencyDto.Codigo}' already exists");

                if (await ExistsBySimboloAsync(currencyDto.Simbolo))
                    throw new ArgumentException($"Currency with symbol '{currencyDto.Simbolo}' already exists");

                var currency = _mapper.Map<Currency>(currencyDto);
                currency.Activo = true;

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

                if (await ExistsByCodigoAsync(currencyDto.Codigo, currencyDto.Id))
                    throw new ArgumentException($"Currency with code '{currencyDto.Codigo}' already exists");

                if (await ExistsBySimboloAsync(currencyDto.Simbolo, currencyDto.Id))
                    throw new ArgumentException($"Currency with symbol '{currencyDto.Simbolo}' already exists");

                _mapper.Map(currencyDto, existingCurrency);
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
                await _currencyRepository.UpdateAsync(currency);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting currency: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                    return false;

                var currencies = await _currencyRepository.FindAsync(c => c.Codigo == codigo);
                return currencies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking currency code existence: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsBySimboloAsync(string simbolo, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(simbolo))
                    return false;

                var currencies = await _currencyRepository.FindAsync(c => c.Simbolo == simbolo);
                return currencies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking currency symbol existence: {ex.Message}", ex);
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
                    c.Activo && (
                        c.Nombre.ToLower().Contains(normalizedSearchTerm) ||
                        c.Codigo.ToLower().Contains(normalizedSearchTerm) ||
                        c.Simbolo.ToLower().Contains(normalizedSearchTerm)
                    )
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