using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartamentoController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<DepartamentoController> _logger;

        public DepartamentoController(
            IVelneoApiService velneoApiService,
            ILogger<DepartamentoController> logger)
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
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

                var departamentos = await _velneoApiService.GetAllDepartamentosAsync();

                _logger.LogInformation("✅ Departamentos obtenidos: {Count}", departamentos.Count());

                return Ok(departamentos.OrderBy(d => d.Nombre));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo departamentos");
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
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

                var departamentos = await _velneoApiService.GetAllDepartamentosAsync();

                var lookup = departamentos
                    .Where(d => d.Activo)
                    .Select(d => new {
                        id = d.Id,
                        nombre = d.Nombre,
                        bonificacion = d.BonificacionInterior
                    })
                    .OrderBy(d => d.nombre);

                _logger.LogInformation("✅ Departamentos lookup obtenidos: {Count}", lookup.Count());

                return Ok(lookup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo departamentos lookup");
                return StatusCode(500, new { error = "Error interno del servidor" });
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

                var departamentos = await _velneoApiService.GetAllDepartamentosAsync();
                var departamento = departamentos.FirstOrDefault(d => d.Id == id);

                if (departamento == null)
                {
                    _logger.LogWarning("⚠️ Departamento no encontrado: {Id}", id);
                    return NotFound(new { error = "Departamento no encontrado" });
                }

                return Ok(departamento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo departamento {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}