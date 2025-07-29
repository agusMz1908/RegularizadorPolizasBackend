using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/secciones")]
    [Authorize]
    public class SeccionesController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService; 
        private readonly ILogger<SeccionesController> _logger;

        public SeccionesController(
            IVelneoApiService velneoApiService, 
            ILogger<SeccionesController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetSecciones()
        {
            try
            {
                _logger.LogInformation("Getting all secciones from Velneo API");
                var secciones = await _velneoApiService.GetActiveSeccionesAsync(); 
                _logger.LogInformation("Successfully retrieved {Count} secciones from Velneo API", secciones.Count());
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all secciones from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetActiveSecciones()
        {
            try
            {
                _logger.LogInformation("Getting active secciones from Velneo API");
                var secciones = await _velneoApiService.GetActiveSeccionesAsync();
                _logger.LogInformation("Successfully retrieved {Count} active secciones from Velneo API", secciones.Count());
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active secciones from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<SeccionLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionLookupDto>>> GetSeccionesForLookup()
        {
            try
            {
                _logger.LogInformation("Getting secciones for lookup from Velneo API");
                var secciones = await _velneoApiService.GetSeccionesForLookupAsync();
                _logger.LogInformation("Successfully retrieved {Count} secciones for lookup from Velneo API", secciones.Count());
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for lookup from Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SeccionDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SeccionDto>> GetSeccion(int id)
        {
            try
            {
                _logger.LogInformation("Getting seccion {SeccionId} from Velneo API", id);
                var seccion = await _velneoApiService.GetSeccionAsync(id);

                if (seccion == null)
                {
                    _logger.LogWarning("Seccion {SeccionId} not found in Velneo API", id);
                    return NotFound(new { message = $"Sección con ID {id} no encontrada" });
                }

                _logger.LogInformation("Successfully retrieved seccion {SeccionId} from Velneo API", id);
                return Ok(seccion);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Seccion {SeccionId} not found in Velneo API", id);
                return NotFound(new { message = $"Sección con ID {id} no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seccion {SeccionId} from Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> SearchSecciones([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Término de búsqueda requerido" });
                }

                _logger.LogInformation("Searching secciones with term '{SearchTerm}' in Velneo API", searchTerm);
                var secciones = await _velneoApiService.SearchSeccionesAsync(searchTerm);
                _logger.LogInformation("Found {Count} secciones matching '{SearchTerm}' in Velneo API", secciones.Count(), searchTerm);
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching secciones with term '{SearchTerm}' in Velneo API", searchTerm);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("company/{companyId}")]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetSeccionesByCompany(int companyId)
        {
            try
            {
                _logger.LogInformation("Getting secciones for company {CompanyId} from Velneo API", companyId);
                var secciones = await _velneoApiService.GetSeccionesByCompanyAsync(companyId);
                _logger.LogInformation("Successfully retrieved {Count} secciones for company {CompanyId} from Velneo API", secciones.Count(), companyId);
                return Ok(secciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for company {CompanyId} from Velneo API", companyId);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("exists")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> SeccionExists([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { message = "El nombre es requerido" });
                }

                _logger.LogInformation("Checking if seccion exists with name '{Name}' in Velneo API", name);

                // ✅ Implementación usando búsqueda
                var secciones = await _velneoApiService.SearchSeccionesAsync(name);
                var exists = secciones.Any(s =>
                    string.Equals(s.Seccion, name, StringComparison.OrdinalIgnoreCase) &&
                    (excludeId == null || s.Id != excludeId));

                _logger.LogInformation("Seccion exists check for '{Name}': {Exists}", name, exists);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if seccion exists with name '{Name}' in Velneo API", name);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}