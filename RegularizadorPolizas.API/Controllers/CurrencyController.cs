using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CurrencyDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAllCurrencies()
        {
            try
            {
                var currencies = await _currencyService.GetAllCurrenciesAsync();
                if (currencies == null || !currencies.Any())
                    return NotFound("No currencies found");

                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CurrencyDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetActiveCurrencies()
        {
            try
            {
                var currencies = await _currencyService.GetActiveCurrenciesAsync();
                if (currencies == null || !currencies.Any())
                    return NotFound("No active currencies found");

                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<CurrencyLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CurrencyLookupDto>>> GetCurrenciesForLookup()
        {
            try
            {
                var currencies = await _currencyService.GetCurrenciesForLookupAsync();
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(IEnumerable<CurrencySummaryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CurrencySummaryDto>>> GetCurrenciesSummary()
        {
            try
            {
                var currencies = await _currencyService.GetActiveCurrenciesAsync();
                var summary = currencies.Select(c => new CurrencySummaryDto
                {
                    Id = c.Id,
                    Moneda = c.Moneda,
                    Nombre = c.Nombre,
                    Simbolo = c.Simbolo,
                    Activo = c.Activo,
                    TotalPolizas = c.TotalPolizas
                });

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("default")]
        [ProducesResponseType(typeof(CurrencyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CurrencyDto>> GetDefaultCurrency()
        {
            try
            {
                var currency = await _currencyService.GetDefaultCurrencyAsync();
                if (currency == null)
                    return NotFound("Default currency not found");

                return Ok(currency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CurrencyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CurrencyDto>> GetCurrencyById(int id)
        {
            try
            {
                var currency = await _currencyService.GetCurrencyByIdAsync(id);
                if (currency == null)
                    return NotFound($"Currency with ID {id} not found");

                return Ok(currency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("code/{codigo}")]
        [ProducesResponseType(typeof(CurrencyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CurrencyDto>> GetCurrencyByCode(string codigo)
        {
            try
            {
                var currency = await _currencyService.GetCurrencyByCodigoAsync(codigo);
                if (currency == null)
                    return NotFound($"Currency with code '{codigo}' not found");

                return Ok(currency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CurrencyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> SearchCurrencies([FromQuery] string searchTerm)
        {
            try
            {
                var currencies = await _currencyService.SearchCurrenciesAsync(searchTerm);
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CurrencyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CurrencyDto>> CreateCurrency([FromBody] CurrencyCreateDto currencyCreateDto)
        {
            try
            {
                if (currencyCreateDto == null)
                    return BadRequest("Currency data is null");

                var currencyDto = new CurrencyDto
                {
                    Moneda = currencyCreateDto.Moneda,
                    Nombre = currencyCreateDto.Nombre,
                    Simbolo = currencyCreateDto.Simbolo,
                    Activo = true
                };

                var createdCurrency = await _currencyService.CreateCurrencyAsync(currencyDto);
                return CreatedAtAction(nameof(GetCurrencyById), new { id = createdCurrency.Id }, createdCurrency);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("full")]
        [ProducesResponseType(typeof(CurrencyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CurrencyDto>> CreateCurrencyFull([FromBody] CurrencyDto currencyDto)
        {
            try
            {
                if (currencyDto == null)
                    return BadRequest("Currency data is null");

                var createdCurrency = await _currencyService.CreateCurrencyAsync(currencyDto);
                return CreatedAtAction(nameof(GetCurrencyById), new { id = createdCurrency.Id }, createdCurrency);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCurrency(int id, [FromBody] CurrencyDto currencyDto)
        {
            try
            {
                if (currencyDto == null)
                    return BadRequest("Currency data is null");

                if (id != currencyDto.Id)
                    return BadRequest("Currency ID mismatch");

                await _currencyService.UpdateCurrencyAsync(currencyDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            try
            {
                await _currencyService.DeleteCurrencyAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("Cannot delete"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("exists/code/{codigo}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> ExistsByCode(string codigo, [FromQuery] int? excludeId = null)
        {
            try
            {
                var exists = await _currencyService.ExistsByCodigoAsync(codigo, excludeId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("exists/symbol/{simbolo}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> ExistsBySymbol(string simbolo, [FromQuery] int? excludeId = null)
        {
            try
            {
                var exists = await _currencyService.ExistsBySimboloAsync(simbolo, excludeId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("legacy")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<object>>> GetCurrenciesLegacyFormat()
        {
            try
            {
                var currencies = await _currencyService.GetActiveCurrenciesAsync();
                var legacyFormat = currencies.Select(c => new
                {
                    id = c.Id,
                    moneda = c.Moneda,
                    nombre = c.Nombre,
                    simbolo = c.Simbolo,
                    codigo = c.Codigo, // Campo de compatibilidad
                    activo = c.Activo,
                    totalPolizas = c.TotalPolizas,
                    puedeEliminar = c.PuedeEliminar
                });

                return Ok(legacyFormat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/stats")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<object>> GetCurrencyStats(int id)
        {
            try
            {
                var currency = await _currencyService.GetCurrencyByIdAsync(id);
                if (currency == null)
                    return NotFound($"Currency with ID {id} not found");

                var stats = new
                {
                    currency.Id,
                    currency.Moneda,
                    currency.Nombre,
                    currency.Simbolo,
                    currency.TotalPolizas,
                    IsActive = currency.Activo,
                    CanDelete = currency.PuedeEliminar
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-symbol/{simbolo}")]
        [ProducesResponseType(typeof(CurrencyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CurrencyDto>> GetCurrencyBySymbol(string simbolo)
        {
            try
            {
                var currencies = await _currencyService.SearchCurrenciesAsync(simbolo);
                var currency = currencies.FirstOrDefault(c => c.Simbolo.Equals(simbolo, StringComparison.OrdinalIgnoreCase));

                if (currency == null)
                    return NotFound($"Currency with symbol '{simbolo}' not found");

                return Ok(currency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}