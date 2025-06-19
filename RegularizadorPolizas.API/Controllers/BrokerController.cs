using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpGet("summary")]
        [ProducesResponseType(typeof(IEnumerable<BrokerSummaryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<BrokerSummaryDto>>> GetBrokersSummary()
        {
            try
            {
                var brokers = await _brokerService.GetActiveBrokersAsync();
                var summary = brokers.Select(b => new BrokerSummaryDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Codigo = b.Codigo,
                    Telefono = b.Telefono,
                    Activo = b.Activo,
                    TotalPolizas = b.TotalPolizas
                });

                return Ok(summary);
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
        public async Task<ActionResult<BrokerDto>> CreateBroker([FromBody] BrokerCreateDto brokerCreateDto)
        {
            try
            {
                if (brokerCreateDto == null)
                    return BadRequest("Broker data is null");

                var brokerDto = new BrokerDto
                {
                    Name = brokerCreateDto.Name,
                    Telefono = brokerCreateDto.Telefono,
                    Direccion = brokerCreateDto.Direccion,
                    Observaciones = brokerCreateDto.Observaciones,
                    Foto = brokerCreateDto.Foto,
                    Codigo = brokerCreateDto.Codigo,
                    Email = brokerCreateDto.Email,
                    Activo = true
                };

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

        [HttpPost("full")]
        [ProducesResponseType(typeof(BrokerDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<BrokerDto>> CreateBrokerFull([FromBody] BrokerDto brokerDto)
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
        public async Task<IActionResult> UpdateBroker(int id, [FromBody] BrokerDto brokerDto)
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

        [HttpGet("legacy")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<object>>> GetBrokersLegacyFormat()
        {
            try
            {
                var brokers = await _brokerService.GetActiveBrokersAsync();
                var legacyFormat = brokers.Select(b => new
                {
                    id = b.Id,
                    nombre = b.Nombre, // Campo de compatibilidad
                    codigo = b.Codigo,
                    domicilio = b.Domicilio, // Campo de compatibilidad
                    telefono = b.Telefono,
                    email = b.Email,
                    activo = b.Activo,
                    totalPolizas = b.TotalPolizas,
                    puedeEliminar = b.PuedeEliminar
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
        public async Task<ActionResult<object>> GetBrokerStats(int id)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByIdAsync(id);
                if (broker == null)
                    return NotFound($"Broker with ID {id} not found");

                var stats = new
                {
                    broker.Id,
                    broker.Name,
                    broker.Codigo,
                    broker.TotalPolizas,
                    broker.Telefono,
                    broker.Email,
                    HasPhoto = !string.IsNullOrEmpty(broker.Foto),
                    IsActive = broker.Activo
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("with-photos")]
        [ProducesResponseType(typeof(IEnumerable<BrokerDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<BrokerDto>>> GetBrokersWithPhotos()
        {
            try
            {
                var brokers = await _brokerService.GetActiveBrokersAsync();
                var brokersWithPhotos = brokers.Where(b => !string.IsNullOrEmpty(b.Foto));
                return Ok(brokersWithPhotos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id}/photo")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateBrokerPhoto(int id, [FromBody] string photoUrl)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByIdAsync(id);
                if (broker == null)
                    return NotFound($"Broker with ID {id} not found");

                broker.Foto = photoUrl ?? string.Empty;
                await _brokerService.UpdateBrokerAsync(broker);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}