using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RegularizadorPolizas.Application.Models;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CorredorController : ControllerBase
    {
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<CorredorController> _logger;

        public CorredorController(
            IVelneoMaestrosService velneoMaestrosService,
            ILogger<CorredorController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los corredores
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VelneoCorredor>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetCorredores()
        {
            try
            {
                var corredores = await _velneoMaestrosService.GetAllCorredoresAsync();
                return Ok(new
                {
                    success = true,
                    data = corredores,
                    count = corredores.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener corredores");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener corredores",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un corredor por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VelneoCorredor), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetCorredor(int id)
        {
            try
            {
                var corredor = await _velneoMaestrosService.GetCorredorByIdAsync(id);

                if (corredor == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Corredor {id} no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = corredor
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener corredor {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener corredor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea un nuevo corredor
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(VelneoCorredor), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateCorredor([FromBody] VelneoCorredor corredor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(corredor.Corrnom))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El nombre del corredor es requerido"
                    });
                }

                var corredorCreado = await _velneoMaestrosService.CreateCorredorAsync(corredor);

                return CreatedAtAction(nameof(GetCorredor), new { id = corredorCreado.Id }, new
                {
                    success = true,
                    message = "Corredor creado exitosamente",
                    data = corredorCreado
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear corredor");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear corredor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Busca corredores por término
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<VelneoCorredor>), 200)]
        public async Task<ActionResult> SearchCorredores([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El término de búsqueda es requerido"
                    });
                }

                var corredores = await _velneoMaestrosService.SearchCorredoresAsync(term);

                return Ok(new
                {
                    success = true,
                    data = corredores,
                    count = corredores.Count(),
                    searchTerm = term
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar corredores con término '{Term}'", term);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al buscar corredores",
                    error = ex.Message
                });
            }
        }
    }
}