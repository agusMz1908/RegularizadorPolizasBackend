using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class ApiKeysController : ControllerBase
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ITenantService _tenantService;
        private readonly IMapper _mapper;

        public ApiKeysController(
            IApiKeyRepository apiKeyRepository,
            ITenantService tenantService,
            IMapper mapper)
        {
            _apiKeyRepository = apiKeyRepository;
            _tenantService = tenantService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApiKeyDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetAll()
        {
            var apiKeys = await _apiKeyRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys);
            return Ok(dtos);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ApiKeyDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetActive()
        {
            var apiKeys = await _apiKeyRepository.GetActiveApiKeysAsync();
            var dtos = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiKeyDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiKeyDto>> GetById(int id)
        {
            var apiKey = await _apiKeyRepository.GetByIdAsync(id);
            if (apiKey == null)
                return NotFound($"ApiKey con ID {id} no encontrado");

            var dto = _mapper.Map<ApiKeyDto>(apiKey);
            return Ok(dto);
        }

        [HttpGet("tenant/{tenantId}")]
        [ProducesResponseType(typeof(ApiKeyDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiKeyDto>> GetByTenantId(string tenantId)
        {
            var apiKey = await _apiKeyRepository.GetByTenantIdAsync(tenantId);
            if (apiKey == null)
                return NotFound($"ApiKey para tenant {tenantId} no encontrado");

            var dto = _mapper.Map<ApiKeyDto>(apiKey);
            return Ok(dto);
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(IEnumerable<ApiKeyDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> Search([FromBody] ApiKeySearchDto searchDto)
        {
            IEnumerable<ApiKey> apiKeys;

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                apiKeys = await _apiKeyRepository.SearchAsync(searchDto.SearchTerm);
            }
            else
            {
                apiKeys = await _apiKeyRepository.GetAllAsync();
            }

            if (!string.IsNullOrEmpty(searchDto.Environment))
            {
                apiKeys = apiKeys.Where(a => a.Environment == searchDto.Environment);
            }

            if (searchDto.Activo.HasValue)
            {
                apiKeys = apiKeys.Where(a => a.Activo == searchDto.Activo.Value);
            }

            if (searchDto.CreatedAfter.HasValue)
            {
                apiKeys = apiKeys.Where(a => a.FechaCreacion >= searchDto.CreatedAfter.Value);
            }

            if (searchDto.CreatedBefore.HasValue)
            {
                apiKeys = apiKeys.Where(a => a.FechaCreacion <= searchDto.CreatedBefore.Value);
            }

            if (searchDto.ShowExpired == false)
            {
                apiKeys = apiKeys.Where(a => !a.FechaExpiracion.HasValue || a.FechaExpiracion > DateTime.UtcNow);
            }

            var dtos = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys);
            return Ok(dtos);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiKeyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ApiKeyDto>> Create([FromBody] ApiKeyCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _apiKeyRepository.ExistsByTenantIdAsync(createDto.TenantId))
            {
                return Conflict($"Ya existe una configuración para el tenant {createDto.TenantId}");
            }

            if (await _apiKeyRepository.ExistsByKeyAsync(createDto.Key))
            {
                return Conflict($"La API Key ya está en uso");
            }

            var apiKey = _mapper.Map<ApiKey>(createDto);
            await _apiKeyRepository.AddAsync(apiKey);

            var dto = _mapper.Map<ApiKeyDto>(apiKey);
            return CreatedAtAction(nameof(GetById), new { id = apiKey.Id }, dto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiKeyDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ApiKeyDto>> Update(int id, [FromBody] ApiKeyUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID en la URL no coincide con el ID del objeto");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingApiKey = await _apiKeyRepository.GetByIdAsync(id);
            if (existingApiKey == null)
                return NotFound($"ApiKey con ID {id} no encontrado");

            if (!string.IsNullOrEmpty(updateDto.Key) &&
                updateDto.Key != existingApiKey.Key &&
                await _apiKeyRepository.ExistsByKeyAsync(updateDto.Key, id))
            {
                return Conflict("La nueva API Key ya está en uso");
            }

            if (!string.IsNullOrEmpty(updateDto.Key))
                existingApiKey.Key = updateDto.Key;

            if (!string.IsNullOrEmpty(updateDto.BaseUrl))
                existingApiKey.BaseUrl = updateDto.BaseUrl;

            if (updateDto.Activo.HasValue)
                existingApiKey.Activo = updateDto.Activo.Value;

            if (updateDto.FechaExpiracion.HasValue)
                existingApiKey.FechaExpiracion = updateDto.FechaExpiracion;

            if (updateDto.Descripcion != null)
                existingApiKey.Descripcion = updateDto.Descripcion;

            if (!string.IsNullOrEmpty(updateDto.Environment))
                existingApiKey.Environment = updateDto.Environment;

            if (updateDto.MaxRequestsPerMinute.HasValue)
                existingApiKey.MaxRequestsPerMinute = updateDto.MaxRequestsPerMinute;

            if (updateDto.ContactEmail != null)
                existingApiKey.ContactEmail = updateDto.ContactEmail;

            if (updateDto.EnableLogging.HasValue)
                existingApiKey.EnableLogging = updateDto.EnableLogging.Value;

            if (updateDto.EnableRetries.HasValue)
                existingApiKey.EnableRetries = updateDto.EnableRetries.Value;

            if (updateDto.TimeoutSeconds.HasValue)
                existingApiKey.TimeoutSeconds = updateDto.TimeoutSeconds.Value;

            if (!string.IsNullOrEmpty(updateDto.ApiVersion))
                existingApiKey.ApiVersion = updateDto.ApiVersion;

            await _apiKeyRepository.UpdateAsync(existingApiKey);

            var dto = _mapper.Map<ApiKeyDto>(existingApiKey);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var apiKey = await _apiKeyRepository.GetByIdAsync(id);
            if (apiKey == null)
                return NotFound($"ApiKey con ID {id} no encontrado");

            await _apiKeyRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("environment/{environment}")]
        [ProducesResponseType(typeof(IEnumerable<ApiKeyDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetByEnvironment(string environment)
        {
            var apiKeys = await _apiKeyRepository.GetByEnvironmentAsync(environment);
            var dtos = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys);
            return Ok(dtos);
        }

        [HttpGet("expired")]
        [ProducesResponseType(typeof(IEnumerable<ApiKeyDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetExpired()
        {
            var apiKeys = await _apiKeyRepository.GetExpiredApiKeysAsync();
            var dtos = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys);
            return Ok(dtos);
        }

        [HttpGet("unused/{days}")]
        [ProducesResponseType(typeof(IEnumerable<ApiKeyDto>), 200)]
        public async Task<ActionResult<IEnumerable<ApiKeyDto>>> GetUnused([Range(1, 365)] int days)
        {
            var apiKeys = await _apiKeyRepository.GetUnusedApiKeysAsync(days);
            var dtos = _mapper.Map<IEnumerable<ApiKeyDto>>(apiKeys);
            return Ok(dtos);
        }

        [HttpPost("validate/{tenantId}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> ValidateTenantConfiguration(string tenantId)
        {
            try
            {
                var config = await _tenantService.GetTenantConfigurationAsync(tenantId);
                return Ok(true);
            }
            catch (InvalidOperationException)
            {
                return Ok(false);
            }
        }
    }
}