using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        public ApiKeyService(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task<ApiKey?> GetApiKeyAsync(string key)
        {
            return await _apiKeyRepository.GetByKeyAsync(key);
        }
    }
} 