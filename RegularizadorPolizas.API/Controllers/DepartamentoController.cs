using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartamentoController : ControllerBase
    {
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<DepartamentoController> _logger;

        public DepartamentoController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<DepartamentoController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartamentoDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DepartamentoDto>>> GetDepartamentos()
        {
            try
            {
                _logger.LogInformation("🏛️ DepartamentosController: Obteniendo departamentos...");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var departamentos = await _velneoMaestrosService.GetAllDepartamentosAsync();

                _logger.LogInformation("✅ Departamentos obtenidos: {Count}", departamentos.Count());

                var departamentosOrdenados = departamentos.OrderBy(d => d.Nombre).ToList();

                return Ok(new
                {
                    success = true,
                    data = departamentosOrdenados,
                    total = departamentosOrdenados.Count,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo departamentos");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Error interno del servidor",
                    details = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene departamentos para lookup/select (zona de circulación)
        /// </summary>
        /// <returns>Lista simplificada de departamentos</returns>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<ActionResult> GetDepartamentosLookup()
        {
            try
            {
                _logger.LogInformation("🏛️ DepartamentosController: Obteniendo departamentos lookup...");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var departamentos = await _velneoMaestrosService.GetAllDepartamentosAsync();

                var lookup = departamentos
                    .Where(d => d.Activo)
                    .Select(d => new {
                        id = d.Id,
                        nombre = d.Nombre,
                        bonificacion = d.BonificacionInterior
                    })
                    .OrderBy(d => d.nombre)
                    .ToList();

                _logger.LogInformation("✅ Departamentos lookup obtenidos: {Count}", lookup.Count());

                return Ok(new
                {
                    success = true,
                    data = lookup,
                    total = lookup.Count,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo departamentos lookup");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene un departamento específico por ID
        /// </summary>
        /// <param name="id">ID del departamento</param>
        /// <returns>Departamento encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartamentoDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DepartamentoDto>> GetDepartamento(int id)
        {
            try
            {
                _logger.LogInformation("🏛️ DepartamentosController: Obteniendo departamento ID: {Id}", id);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var departamentos = await _velneoMaestrosService.GetAllDepartamentosAsync();
                var departamento = departamentos.FirstOrDefault(d => d.Id == id);

                if (departamento == null)
                {
                    _logger.LogWarning("⚠️ Departamento no encontrado: {Id}", id);
                    return NotFound(new
                    {
                        success = false,
                        error = "Departamento no encontrado",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = departamento,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo departamento {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}