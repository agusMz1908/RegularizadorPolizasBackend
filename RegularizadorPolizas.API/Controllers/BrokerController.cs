using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrokersController : ControllerBase
    {
        private readonly IBrokerService _brokerService;

        public BrokersController(IBrokerService brokerService)
        {
            _brokerService = brokerService ?? throw new ArgumentNullException(nameof(brokerService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrokerDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<BrokerDto>>> GetAllBrokers()
        {
            try
            {
                var brokers = await _brokerService.GetAllBrokersAsync();
                if (brokers == null || !brokers.Any())
                    return NotFound("No brokers found");

                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<BrokerDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<BrokerDto>>> GetActiveBrokers()
        {
            try
            {
                var brokers = await _brokerService.GetActiveBrokersAsync();
                if (brokers == null || !brokers.Any())
                    return NotFound("No active brokers found");

                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<BrokerLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<BrokerLookupDto>>> GetBrokersForLookup()
        {
            try
            {
                var brokers = await _brokerService.GetBrokersForLookupAsync();
                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BrokerDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<BrokerDto>> GetBrokerById(int id)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByIdAsync(id);
                if (broker == null)
                    return NotFound($"Broker with ID {id} not found");

                return Ok(broker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("code/{codigo}")]
        [ProducesResponseType(typeof(BrokerDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<BrokerDto>> GetBrokerByCode(string codigo)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByCodigoAsync(codigo);
                if (broker == null)
                    return NotFound($"Broker with code '{codigo}' not found");

                return Ok(broker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(BrokerDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<BrokerDto>> GetBrokerByEmail(string email)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByEmailAsync(email);
                if (broker == null)
                    return NotFound($"Broker with email '{email}' not found");

                return Ok(broker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<BrokerDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<BrokerDto>>> SearchBrokers([FromQuery] string searchTerm)
        {
            try
            {
                var brokers = await _brokerService.SearchBrokersAsync(searchTerm);
                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BrokerDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<BrokerDto>> CreateBroker(BrokerDto brokerDto)
        {
            try
            {
                if (brokerDto == null)
                    return BadRequest("Broker data is null");

                var createdBroker = await _brokerService.CreateBrokerAsync(brokerDto);
                return CreatedAtAction(nameof(GetBrokerById), new { id = createdBroker.Id }, createdBroker);
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
        public async Task<IActionResult> UpdateBroker(int id, BrokerDto brokerDto)
        {
            try
            {
                if (brokerDto == null)
                    return BadRequest("Broker data is null");

                if (id != brokerDto.Id)
                    return BadRequest("Broker ID mismatch");

                await _brokerService.UpdateBrokerAsync(brokerDto);
                return NoContent();
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

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteBroker(int id)
        {
            try
            {
                await _brokerService.DeleteBrokerAsync(id);
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
                var exists = await _brokerService.ExistsByCodigoAsync(codigo, excludeId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("exists/email/{email}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> ExistsByEmail(string email, [FromQuery] int? excludeId = null)
        {
            try
            {
                var exists = await _brokerService.ExistsByEmailAsync(email, excludeId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}