using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SeccionesController : ControllerBase
    {
        private readonly ISeccionService _seccionService;
        private readonly ILogger<SeccionesController> _logger;

        public SeccionesController(
            ISeccionService seccionService,
            ILogger<SeccionesController> logger)
        {
            _seccionService = seccionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetSecciones()
        {
            try
            {
                var secciones = await _seccionService.GetAllSeccionesAsync();
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all secciones");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetActiveSecciones()
        {
            try
            {
                var secciones = await _seccionService.GetActiveSeccionesAsync();
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active secciones");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("lookup")]
        public async Task<ActionResult<IEnumerable<SeccionLookupDto>>> GetSeccionesForLookup()
        {
            try
            {
                var secciones = await _seccionService.GetSeccionesForLookupAsync();
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for lookup");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeccionDto>> GetSeccion(int id)
        {
            try
            {
                var seccion = await _seccionService.GetSeccionByIdAsync(id);

                if (seccion == null)
                {
                    return NotFound($"Sección con ID {id} no encontrada");
                }

                return Ok(seccion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seccion {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> SearchSecciones([FromQuery] string searchTerm)
        {
            try
            {
                var secciones = await _seccionService.SearchSeccionesAsync(searchTerm);
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching secciones with term {SearchTerm}", searchTerm);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SeccionDto>> CreateSeccion([FromBody] CreateSeccionDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var seccion = await _seccionService.CreateSeccionAsync(createDto);
                return CreatedAtAction(nameof(GetSeccion), new { id = seccion.Id }, seccion);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation creating seccion");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seccion");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SeccionDto>> UpdateSeccion(int id, [FromBody] UpdateSeccionDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var seccion = await _seccionService.UpdateSeccionAsync(id, updateDto);
                return Ok(seccion);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Seccion not found for update");
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation updating seccion");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating seccion {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSeccion(int id)
        {
            try
            {
                await _seccionService.DeleteSeccionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Seccion not found for deletion");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seccion {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("exists")]
        public async Task<ActionResult<bool>> SeccionExists([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("El nombre es requerido");
                }

                var exists = await _seccionService.SeccionExistsAsync(name, excludeId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if seccion exists");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}