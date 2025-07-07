using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces; 

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolizasController : ControllerBase
    {
        private readonly IPolizaService _polizaService;

        public PolizasController(IPolizaService polizaService)
        {
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PolizaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PolizaDto>>> GetPolizas()
        {
            try
            {
                var polizas = await _polizaService.GetAllPolizasAsync();
                if (polizas == null || !polizas.Any())
                    return NotFound("No policies found");

                return Ok(polizas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PolizaDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> GetPolizaById(int id)
        {
            try
            {
                var poliza = await _polizaService.GetPolizaByIdAsync(id);
                if (poliza == null)
                    return NotFound($"Policy with ID {id} not found");

                return Ok(poliza);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(IEnumerable<PolizaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PolizaDto>>> GetPolizasByCliente(int clienteId)
        {
            try
            {
                var polizas = await _polizaService.GetPolizasByClienteAsync(clienteId);
                if (polizas == null || !polizas.Any())
                    return NotFound($"No policies found for client with ID {clienteId}");

                return Ok(polizas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<PolizaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PolizaDto>>> SearchPolizas([FromQuery] string searchTerm)
        {
            try
            {
                var polizas = await _polizaService.SearchPolizasAsync(searchTerm);
                if (polizas == null || !polizas.Any())
                    return NotFound($"No policies found matching '{searchTerm}'");

                return Ok(polizas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PolizaDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> CreatePoliza(PolizaDto polizaDto)
        {
            try
            {
                if (polizaDto == null)
                    return BadRequest("Policy data is null");

                var createdPoliza = await _polizaService.CreatePolizaAsync(polizaDto);
                return CreatedAtAction(nameof(GetPolizaById), new { id = createdPoliza.Id }, createdPoliza);
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
        public async Task<IActionResult> UpdatePoliza(int id, PolizaDto polizaDto)
        {
            try
            {
                if (polizaDto == null)
                    return BadRequest("Policy data is null");

                if (id != polizaDto.Id)
                    return BadRequest("Policy ID mismatch");

                var existingPoliza = await _polizaService.GetPolizaByIdAsync(id);
                if (existingPoliza == null)
                    return NotFound($"Policy with ID {id} not found");

                await _polizaService.UpdatePolizaAsync(polizaDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePoliza(int id)
        {
            try
            {
                var existingPoliza = await _polizaService.GetPolizaByIdAsync(id);
                if (existingPoliza == null)
                    return NotFound($"Policy with ID {id} not found");

                await _polizaService.DeletePolizaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/renew")]
        [ProducesResponseType(typeof(PolizaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> RenovarPoliza(int id, RenovationDto renovationDto)
        {
            try
            {
                if (renovationDto == null)
                    return BadRequest("Renewal data is null");

                var polizaRenovada = await _polizaService.RenovarPolizaAsync(id, renovationDto);
                return Ok(polizaRenovada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}