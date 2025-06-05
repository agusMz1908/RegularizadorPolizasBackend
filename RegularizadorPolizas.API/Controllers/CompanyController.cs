using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
        {
            try
            {
                var companies = await _companyService.GetAllCompaniesAsync();
                if (companies == null || !companies.Any())
                    return NotFound("No companies found");

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetActiveCompanies()
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync();
                if (companies == null || !companies.Any())
                    return NotFound("No active companies found");

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<CompanyLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyLookupDto>>> GetCompaniesForLookup()
        {
            try
            {
                var companies = await _companyService.GetCompaniesForLookupAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(IEnumerable<CompanySummaryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanySummaryDto>>> GetCompaniesSummary()
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync();
                var summary = companies.Select(c => new CompanySummaryDto
                {
                    Id = c.Id,
                    Comnom = c.Comnom,
                    Comalias = c.Comalias,
                    Cod_srvcompanias = c.Cod_srvcompanias,
                    Broker = c.Broker,
                    Activo = c.Activo,
                    TotalPolizas = c.TotalPolizas
                });

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                    return NotFound($"Company with ID {id} not found");

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("code/{codigo}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByCode(string codigo)
        {
            try
            {
                var company = await _companyService.GetCompanyByCodigoAsync(codigo);
                if (company == null)
                    return NotFound($"Company with code '{codigo}' not found");

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("alias/{alias}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByAlias(string alias)
        {
            try
            {
                var company = await _companyService.GetCompanyByAliasAsync(alias);
                if (company == null)
                    return NotFound($"Company with alias '{alias}' not found");

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> SearchCompanies([FromQuery] string searchTerm)
        {
            try
            {
                var companies = await _companyService.SearchCompaniesAsync(searchTerm);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CompanyCreateDto companyCreateDto)
        {
            try
            {
                if (companyCreateDto == null)
                    return BadRequest("Company data is null");

                var companyDto = new CompanyDto
                {
                    Comnom = companyCreateDto.Comnom,
                    Comrazsoc = companyCreateDto.Comrazsoc,
                    Comruc = companyCreateDto.Comruc,
                    Comdom = companyCreateDto.Comdom,
                    Comtel = companyCreateDto.Comtel,
                    Comfax = companyCreateDto.Comfax,
                    Comalias = companyCreateDto.Comalias,
                    Broker = companyCreateDto.Broker,
                    Cod_srvcompanias = companyCreateDto.Cod_srvcompanias,
                    No_utiles = companyCreateDto.No_utiles,
                    Paq_dias = companyCreateDto.Paq_dias,
                    Activo = true
                };

                var createdCompany = await _companyService.CreateCompanyAsync(companyDto);
                return CreatedAtAction(nameof(GetCompanyById), new { id = createdCompany.Id }, createdCompany);
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
        [ProducesResponseType(typeof(CompanyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CompanyDto>> CreateCompanyFull([FromBody] CompanyDto companyDto)
        {
            try
            {
                if (companyDto == null)
                    return BadRequest("Company data is null");

                var createdCompany = await _companyService.CreateCompanyAsync(companyDto);
                return CreatedAtAction(nameof(GetCompanyById), new { id = createdCompany.Id }, createdCompany);
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
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyDto companyDto)
        {
            try
            {
                if (companyDto == null)
                    return BadRequest("Company data is null");

                if (id != companyDto.Id)
                    return BadRequest("Company ID mismatch");

                await _companyService.UpdateCompanyAsync(companyDto);
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
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                await _companyService.DeleteCompanyAsync(id);
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
                var exists = await _companyService.ExistsByCodigoAsync(codigo, excludeId);
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
        public async Task<ActionResult<IEnumerable<object>>> GetCompaniesLegacyFormat()
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync();
                var legacyFormat = companies.Select(c => new
                {
                    id = c.Id,
                    nombre = c.Nombre, // Campo de compatibilidad
                    alias = c.Alias,   // Campo de compatibilidad
                    codigo = c.Codigo, // Campo de compatibilidad
                    activo = c.Activo,
                    totalPolizas = c.TotalPolizas,
                    puedeEliminar = c.PuedeEliminar
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
        public async Task<ActionResult<object>> GetCompanyStats(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                    return NotFound($"Company with ID {id} not found");

                var stats = new
                {
                    company.Id,
                    company.Comnom,
                    company.Comalias,
                    company.TotalPolizas,
                    TotalPremiums = company.Comtotpre,
                    TotalCommissions = company.Comtotcomi,
                    ClientCount = company.Comcntcli,
                    ContractCount = company.Comcntcon,
                    IsBroker = company.Broker,
                    IsActive = company.Activo
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("brokers")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetBrokers()
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync();
                var brokers = companies.Where(c => c.Broker);
                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("insurers")]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetInsurers()
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync();
                var insurers = companies.Where(c => !c.Broker);
                return Ok(insurers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}