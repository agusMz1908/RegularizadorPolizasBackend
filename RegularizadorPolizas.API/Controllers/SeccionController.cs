using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/secciones")]
    [Authorize]
    public class SeccionesController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<SeccionesController> _logger;

        public SeccionesController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<SeccionesController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetSecciones()
        {
            try
            {
                _logger.LogInformation("📋 Getting all secciones from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var secciones = await _velneoMaestrosService.GetActiveSeccionesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} secciones from VelneoMaestrosService", secciones.Count());

                return Ok(new
                {
                    success = true,
                    data = secciones,
                    total = secciones.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all secciones from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetActiveSecciones()
        {
            try
            {
                _logger.LogInformation("📋 Getting active secciones from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var secciones = await _velneoMaestrosService.GetActiveSeccionesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} active secciones from VelneoMaestrosService", secciones.Count());

                return Ok(new
                {
                    success = true,
                    data = secciones,
                    total = secciones.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting active secciones from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<SeccionLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionLookupDto>>> GetSeccionesForLookup()
        {
            try
            {
                _logger.LogInformation("📋 Getting secciones for lookup from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var secciones = await _velneoMaestrosService.GetSeccionesForLookupAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} secciones for lookup from VelneoMaestrosService", secciones.Count());

                return Ok(new
                {
                    success = true,
                    data = secciones,
                    total = secciones.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting secciones for lookup from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
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
                _logger.LogInformation("📋 Getting seccion {SeccionId} from VelneoMaestrosService", id);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var seccion = await _velneoMaestrosService.GetSeccionAsync(id);

                if (seccion == null)
                {
                    _logger.LogWarning("⚠️ Seccion {SeccionId} not found in VelneoMaestrosService", id);
                    return NotFound(new
                    {
                        success = false,
                        message = $"Sección con ID {id} no encontrada",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ Successfully retrieved seccion {SeccionId} from VelneoMaestrosService", id);

                return Ok(new
                {
                    success = true,
                    data = seccion,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("⚠️ Seccion {SeccionId} not found in VelneoMaestrosService", id);
                return NotFound(new
                {
                    success = false,
                    message = $"Sección con ID {id} no encontrada",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting seccion {SeccionId} from VelneoMaestrosService", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
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
                    return BadRequest(new
                    {
                        success = false,
                        message = "Término de búsqueda requerido",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("🔍 Searching secciones with term '{SearchTerm}' in VelneoMaestrosService", searchTerm);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var secciones = await _velneoMaestrosService.SearchSeccionesAsync(searchTerm);

                _logger.LogInformation("✅ Found {Count} secciones matching '{SearchTerm}' in VelneoMaestrosService", secciones.Count(), searchTerm);

                return Ok(new
                {
                    success = true,
                    data = secciones,
                    total = secciones.Count(),
                    searchTerm = searchTerm,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error searching secciones with term '{SearchTerm}' in VelneoMaestrosService", searchTerm);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("company/{companyId}")]
        [ProducesResponseType(typeof(IEnumerable<SeccionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SeccionDto>>> GetSeccionesByCompany(int companyId)
        {
            try
            {
                _logger.LogInformation("📋 Getting secciones for company {CompanyId} from VelneoMaestrosService", companyId);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var secciones = await _velneoMaestrosService.GetSeccionesByCompanyAsync(companyId);

                _logger.LogInformation("✅ Successfully retrieved {Count} secciones for company {CompanyId} from VelneoMaestrosService", secciones.Count(), companyId);

                return Ok(new
                {
                    success = true,
                    data = secciones,
                    total = secciones.Count(),
                    companyId = companyId,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting secciones for company {CompanyId} from VelneoMaestrosService", companyId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
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
                    return BadRequest(new
                    {
                        success = false,
                        message = "El nombre es requerido",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("🔍 Checking if seccion exists with name '{Name}' in VelneoMaestrosService", name);

                // ✅ CORREGIDO: Implementación usando VelneoMaestrosService
                var secciones = await _velneoMaestrosService.SearchSeccionesAsync(name);
                var exists = secciones.Any(s =>
                    string.Equals(s.Seccion, name, StringComparison.OrdinalIgnoreCase) &&
                    (excludeId == null || s.Id != excludeId));

                _logger.LogInformation("✅ Seccion exists check for '{Name}': {Exists}", name, exists);

                return Ok(new
                {
                    success = true,
                    exists = exists,
                    searchName = name,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking if seccion exists with name '{Name}' in VelneoMaestrosService", name);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}