using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        // ✅ CORREGIDO: Usar IVelneoMaestrosService
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            IVelneoMaestrosService velneoMaestrosService, // ✅ CAMBIO CRÍTICO
            ILogger<CompaniesController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService ?? throw new ArgumentNullException(nameof(velneoMaestrosService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las compañías desde VelneoMaestrosService
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
        {
            try
            {
                _logger.LogInformation("🏢 Getting all companies from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var companies = await _velneoMaestrosService.GetAllCompaniesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} companies from VelneoMaestrosService", companies.Count());

                return Ok(new
                {
                    success = true,
                    data = companies,
                    total = companies.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all companies from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo compañías desde VelneoMaestrosService",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene las compañías para lookup/dropdown desde VelneoMaestrosService (usado por el frontend)
        /// </summary>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<CompanyLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyLookupDto>>> GetCompaniesForLookup()
        {
            try
            {
                _logger.LogInformation("🏢 Getting companies for lookup from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var companies = await _velneoMaestrosService.GetCompaniesForLookupAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} companies for lookup from VelneoMaestrosService", companies.Count());

                return Ok(new
                {
                    success = true,
                    data = companies,
                    total = companies.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting companies for lookup from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al cargar compañías desde VelneoMaestrosService",
                    error = ex.Message,
                    details = "Verifica la conexión con VelneoMaestrosService",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene solo las compañías activas desde VelneoMaestrosService
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetActiveCompanies()
        {
            try
            {
                _logger.LogInformation("🏢 Getting active companies from VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var companies = await _velneoMaestrosService.GetActiveCompaniesAsync();

                _logger.LogInformation("✅ Successfully retrieved {Count} active companies from VelneoMaestrosService", companies.Count());

                return Ok(new
                {
                    success = true,
                    data = companies,
                    total = companies.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting active companies from VelneoMaestrosService");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo compañías activas desde VelneoMaestrosService",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene una compañía por ID desde VelneoMaestrosService
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
        {
            try
            {
                _logger.LogInformation("🏢 Getting company {CompanyId} from VelneoMaestrosService", id);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var company = await _velneoMaestrosService.GetCompanyByIdAsync(id);

                if (company == null)
                {
                    _logger.LogWarning("⚠️ Company {CompanyId} not found in VelneoMaestrosService", id);
                    return NotFound(new
                    {
                        success = false,
                        message = $"Compañía con ID {id} no encontrada en VelneoMaestrosService",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ Successfully retrieved company {CompanyId} from VelneoMaestrosService", id);

                return Ok(new
                {
                    success = true,
                    data = company,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCompanyById not implemented in VelneoMaestrosService: {Message}", ex.Message);
                return StatusCode(501, new
                {
                    success = false,
                    message = "GetCompanyById no está implementado en VelneoMaestrosService aún",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting company {CompanyId} from VelneoMaestrosService", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo compañía desde VelneoMaestrosService",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene una compañía por código desde VelneoMaestrosService
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByCodigo(string codigo)
        {
            try
            {
                _logger.LogInformation("🏢 Getting company by codigo {Codigo} from VelneoMaestrosService", codigo);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var company = await _velneoMaestrosService.GetCompanyByCodigoAsync(codigo);

                if (company == null)
                {
                    _logger.LogWarning("⚠️ Company with codigo {Codigo} not found in VelneoMaestrosService", codigo);
                    return NotFound(new
                    {
                        success = false,
                        message = $"Compañía con código {codigo} no encontrada en VelneoMaestrosService",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ Successfully retrieved company by codigo {Codigo} from VelneoMaestrosService", codigo);

                return Ok(new
                {
                    success = true,
                    data = company,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCompanyByCodigo not implemented in VelneoMaestrosService: {Message}", ex.Message);
                return StatusCode(501, new
                {
                    success = false,
                    message = "GetCompanyByCodigo no está implementado en VelneoMaestrosService aún",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting company by codigo {Codigo} from VelneoMaestrosService", codigo);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo compañía por código desde VelneoMaestrosService",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene una compañía por alias desde VelneoMaestrosService
        /// </summary>
        [HttpGet("alias/{alias}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByAlias(string alias)
        {
            try
            {
                _logger.LogInformation("🏢 Getting company by alias {Alias} from VelneoMaestrosService", alias);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var company = await _velneoMaestrosService.GetCompanyByAliasAsync(alias);

                if (company == null)
                {
                    _logger.LogWarning("⚠️ Company with alias {Alias} not found in VelneoMaestrosService", alias);
                    return NotFound(new
                    {
                        success = false,
                        message = $"Compañía con alias {alias} no encontrada en VelneoMaestrosService",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ Successfully retrieved company by alias {Alias} from VelneoMaestrosService", alias);

                return Ok(new
                {
                    success = true,
                    data = company,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCompanyByAlias not implemented in VelneoMaestrosService: {Message}", ex.Message);
                return StatusCode(501, new
                {
                    success = false,
                    message = "GetCompanyByAlias no está implementado en VelneoMaestrosService aún",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting company by alias {Alias} from VelneoMaestrosService", alias);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error obteniendo compañía por alias desde VelneoMaestrosService",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Busca compañías por término desde VelneoMaestrosService
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
                    return BadRequest(new
                    {
                        success = false,
                        message = "Término de búsqueda requerido",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("🔍 Searching companies with term '{SearchTerm}' in VelneoMaestrosService", searchTerm);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var companies = await _velneoMaestrosService.SearchCompaniesAsync(searchTerm);

                _logger.LogInformation("✅ Successfully found {Count} companies matching '{SearchTerm}' in VelneoMaestrosService", companies.Count(), searchTerm);

                return Ok(new
                {
                    success = true,
                    data = companies,
                    total = companies.Count(),
                    searchTerm = searchTerm,
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("SearchCompanies not implemented in VelneoMaestrosService: {Message}", ex.Message);
                return StatusCode(501, new
                {
                    success = false,
                    message = "SearchCompanies no está implementado en VelneoMaestrosService aún",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error searching companies with term '{SearchTerm}' in VelneoMaestrosService", searchTerm);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error buscando compañías en VelneoMaestrosService",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// CRUD operations would need to be implemented in VelneoMaestrosService first
        /// For now, these endpoints return NotImplemented status
        /// </summary>

        [HttpPost]
        [ProducesResponseType(501)]
        public async Task<ActionResult> CreateCompany([FromBody] CompanyDto companyDto)
        {
            _logger.LogWarning("CreateCompany not implemented - VelneoMaestrosService doesn't support company creation");
            return StatusCode(501, new
            {
                success = false,
                message = "Creación de compañías no está implementada",
                details = "Esta funcionalidad requiere implementación en VelneoMaestrosService",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(501)]
        public async Task<ActionResult> UpdateCompany(int id, [FromBody] CompanyDto companyDto)
        {
            _logger.LogWarning("UpdateCompany not implemented - VelneoMaestrosService doesn't support company updates");
            return StatusCode(501, new
            {
                success = false,
                message = "Actualización de compañías no está implementada",
                details = "Esta funcionalidad requiere implementación en VelneoMaestrosService",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(501)]
        public async Task<ActionResult> DeleteCompany(int id)
        {
            _logger.LogWarning("DeleteCompany not implemented - VelneoMaestrosService doesn't support company deletion");
            return StatusCode(501, new
            {
                success = false,
                message = "Eliminación de compañías no está implementada",
                details = "Esta funcionalidad requiere implementación en VelneoMaestrosService",
                timestamp = DateTime.UtcNow
            });
        }
    }
}