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
                    currencyDto.EsMonedaPorDefecto = currencyDto.Codigo == "UYU";
                }

                return currenciesDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error obteniendo monedas: {ex.Message}", ex);
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
                currencyDto.EsMonedaPorDefecto = currencyDto.Codigo == "UYU";

                return currencyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error obteniendo monedas por ID {id}: {ex.Message}", ex);
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
                currencyDto.EsMonedaPorDefecto = currencyDto.Codigo == "UYU";

                return currencyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error obteniendo moneda por codigo {codigo}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync()
        {
            try
            {
                var currencies = await _currencyRepository.GetActiveCurrenciesAsync();
                var currenciesDto = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);

                foreach (var currencyDto in currenciesDto)
                {
                    currencyDto.EsMonedaPorDefecto = currencyDto.Codigo == "UYU";
                }

                return currenciesDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error obteniendo monedas activas: {ex.Message}", ex);
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
                throw new ApplicationException($"Error obteniendo monedas por lookup: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyDto> GetDefaultCurrencyAsync()
        {
            try
            {
                var currency = await _currencyRepository.GetDefaultCurrencyAsync();
                if (currency == null)
                    return null;

                var currencyDto = _mapper.Map<CurrencyDto>(currency);
                currencyDto.EsMonedaPorDefecto = true;

                return currencyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error obteniendo moneda default: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currencyDto.Nombre))
                    throw new ArgumentException("El nombre de la moneda es obligatorio.");

                if (string.IsNullOrWhiteSpace(currencyDto.Codigo))
                    throw new ArgumentException("El codigo de la moneda es obligatorio.");

                if (string.IsNullOrWhiteSpace(currencyDto.Simbolo))
                    throw new ArgumentException("El simbolo de la moneda es obligatorio.");

                if (await _currencyRepository.ExistsByCodigoAsync(currencyDto.Codigo))
                    throw new ArgumentException($"Moneda con codigo: '{currencyDto.Codigo}' ya existe");

                if (await ExistsBySimboloAsync(currencyDto.Simbolo))
                    throw new ArgumentException($"Moneda con simbolo: '{currencyDto.Simbolo}' ya existe");

                var currency = _mapper.Map<Currency>(currencyDto);
                currency.Activo = true;

                var createdCurrency = await _currencyRepository.AddAsync(currency);
                var result = _mapper.Map<CurrencyDto>(createdCurrency);
                result.EsMonedaPorDefecto = result.Codigo == "UYU";

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creando currency: {ex.Message}", ex);
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
                    throw new ApplicationException($"Moneda con ID: {currencyDto.Id} no existe");

                if (await ExistsByCodigoAsync(currencyDto.Codigo, currencyDto.Id))
                    throw new ArgumentException($"Moneda con codigo: '{currencyDto.Codigo}' ya existe");

                if (await ExistsBySimboloAsync(currencyDto.Simbolo, currencyDto.Id))
                    throw new ArgumentException($"Moneda con simbolo '{currencyDto.Simbolo}' ya existe");

                _mapper.Map(currencyDto, existingCurrency);
                await _currencyRepository.UpdateAsync(existingCurrency);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error actualizando moneda: {ex.Message}", ex);
            }
        }

        public async Task DeleteCurrencyAsync(int id)
        {
            try
            {
                var currency = await _currencyRepository.GetByIdAsync(id);
                if (currency == null)
                    throw new ApplicationException($"Moneda con ID {id} no existe");

                if (currency.Codigo == "UYU")
                    throw new ApplicationException("No se puede eliminar la moneda por defecto (UYU)");

                var polizasCount = await GetPolizasCountAsync(id);
                if (polizasCount > 0)
                    throw new ApplicationException($"No se puede eliminar la moneda. Tiene {polizasCount} polizas asociadas.");

                currency.Activo = false;
                await _currencyRepository.UpdateAsync(currency);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error eliminando la moneda: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            try
            {
                var currencies = await _currencyRepository.FindAsync(c => c.Codigo == codigo);
                return currencies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error chequeando el codigo de la moneda: {ex.Message}", ex);
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
                throw new ApplicationException($"Error chequeando el simbolo de la moneda: {ex.Message}", ex);
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

                var currenciesDto = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);

                foreach (var currencyDto in currenciesDto)
                {
                    currencyDto.EsMonedaPorDefecto = currencyDto.Codigo == "UYU";
                }

                return currenciesDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error buscando monedas: {ex.Message}", ex);
            }
        }

        private async Task<int> GetPolizasCountAsync(int currencyId)
        {
            try
            {
                var polizas = await _polizaRepository.FindAsync(p => p.CurrencyId == currencyId && p.Activo);
                return polizas.Count();
            }
            catch
            {
                return 0;
            }
        }
    }
}