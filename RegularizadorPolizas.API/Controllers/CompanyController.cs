using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(IVelneoApiService velneoApiService, ILogger<CompaniesController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las compañías desde Velneo
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
        {
            try
            {
                _logger.LogInformation("🏢 Getting all companies from Velneo API");
                var companies = await _velneoApiService.GetAllCompaniesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} companies from Velneo", companies.Count());
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all companies from Velneo API");
                return StatusCode(500, new { message = "Error obteniendo compañías desde Velneo", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las compañías para lookup/dropdown desde Velneo (usado por el frontend)
        /// </summary>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<CompanyLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyLookupDto>>> GetCompaniesForLookup()
        {
            try
            {
                _logger.LogInformation("🏢 Getting companies for lookup from Velneo API");
                var companies = await _velneoApiService.GetCompaniesForLookupAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} companies for lookup from Velneo", companies.Count());
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting companies for lookup from Velneo API");
                return StatusCode(500, new
                {
                    message = "Error al cargar compañías desde Velneo",
                    error = ex.Message,
                    details = "Verifica la conexión con Velneo API"
                });
            }
        }

        /// <summary>
        /// Obtiene solo las compañías activas desde Velneo
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetActiveCompanies()
        {
            try
            {
                _logger.LogInformation("🏢 Getting active companies from Velneo API");
                var companies = await _velneoApiService.GetActiveCompaniesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} active companies from Velneo", companies.Count());
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting active companies from Velneo API");
                return StatusCode(500, new { message = "Error obteniendo compañías activas desde Velneo", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una compañía por ID desde Velneo
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
        {
            try
            {
                _logger.LogInformation("🏢 Getting company {CompanyId} from Velneo API", id);

                var company = await _velneoApiService.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    _logger.LogWarning("Company {CompanyId} not found in Velneo API", id);
                    return NotFound(new { message = $"Compañía con ID {id} no encontrada en Velneo" });
                }

                _logger.LogInformation("✅ Successfully retrieved company {CompanyId} from Velneo", id);
                return Ok(company);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCompanyById not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetCompanyById no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting company {CompanyId} from Velneo API", id);
                return StatusCode(500, new { message = "Error obteniendo compañía desde Velneo", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una compañía por código desde Velneo
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByCodigo(string codigo)
        {
            try
            {
                _logger.LogInformation("🏢 Getting company by codigo {Codigo} from Velneo API", codigo);

                var company = await _velneoApiService.GetCompanyByCodigoAsync(codigo);
                if (company == null)
                {
                    _logger.LogWarning("Company with codigo {Codigo} not found in Velneo API", codigo);
                    return NotFound(new { message = $"Compañía con código {codigo} no encontrada en Velneo" });
                }

                _logger.LogInformation("✅ Successfully retrieved company by codigo {Codigo} from Velneo", codigo);
                return Ok(company);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCompanyByCodigo not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetCompanyByCodigo no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting company by codigo {Codigo} from Velneo API", codigo);
                return StatusCode(500, new { message = "Error obteniendo compañía por código desde Velneo", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una compañía por alias desde Velneo
        /// </summary>
        [HttpGet("alias/{alias}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByAlias(string alias)
        {
            try
            {
                _logger.LogInformation("🏢 Getting company by alias {Alias} from Velneo API", alias);

                var company = await _velneoApiService.GetCompanyByAliasAsync(alias);
                if (company == null)
                {
                    _logger.LogWarning("Company with alias {Alias} not found in Velneo API", alias);
                    return NotFound(new { message = $"Compañía con alias {alias} no encontrada en Velneo" });
                }

                _logger.LogInformation("✅ Successfully retrieved company by alias {Alias} from Velneo", alias);
                return Ok(company);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCompanyByAlias not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetCompanyByAlias no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting company by alias {Alias} from Velneo API", alias);
                return StatusCode(500, new { message = "Error obteniendo compañía por alias desde Velneo", error = ex.Message });
            }
        }

        /// <summary>
        /// Busca compañías por término desde Velneo
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> SearchCompanies([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Término de búsqueda requerido" });
                }

                _logger.LogInformation("🔍 Searching companies with term '{SearchTerm}' in Velneo API", searchTerm);

                var companies = await _velneoApiService.SearchCompaniesAsync(searchTerm);

                _logger.LogInformation("✅ Successfully found {Count} companies matching '{SearchTerm}' in Velneo", companies.Count(), searchTerm);
                return Ok(companies);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("SearchCompanies not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "SearchCompanies no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error searching companies with term '{SearchTerm}' in Velneo API", searchTerm);
                return StatusCode(500, new { message = "Error buscando compañías en Velneo", error = ex.Message });
            }
        }

        /// <summary>
        /// CRUD operations would need to be implemented in Velneo API first
        /// For now, these endpoints return NotImplemented status
        /// </summary>

        [HttpPost]
        [ProducesResponseType(501)]
        public async Task<ActionResult> CreateCompany([FromBody] CompanyDto companyDto)
        {
            _logger.LogWarning("CreateCompany not implemented - Velneo API doesn't support company creation");
            return StatusCode(501, new
            {
                message = "Creación de compañías no está implementada",
                details = "Esta funcionalidad requiere implementación en Velneo API"
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(501)]
        public async Task<ActionResult> UpdateCompany(int id, [FromBody] CompanyDto companyDto)
        {
            _logger.LogWarning("UpdateCompany not implemented - Velneo API doesn't support company updates");
            return StatusCode(501, new
            {
                message = "Actualización de compañías no está implementada",
                details = "Esta funcionalidad requiere implementación en Velneo API"
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(501)]
        public async Task<ActionResult> DeleteCompany(int id)
        {
            _logger.LogWarning("DeleteCompany not implemented - Velneo API doesn't support company deletion");
            return StatusCode(501, new
            {
                message = "Eliminación de compañías no está implementada",
                details = "Esta funcionalidad requiere implementación en Velneo API"
            });
        }
    }
}