using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class ApiKeysController : ControllerBase
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeysController(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ApiKey>> GetAll()
        {
            return await _apiKeyRepository.GetAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApiKey apiKey)
        {
            await _apiKeyRepository.AddAsync(apiKey);
            return CreatedAtAction(nameof(GetAll), new { id = apiKey.Id }, apiKey);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiKeyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
} 