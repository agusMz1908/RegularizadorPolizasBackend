using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Security.Claims;

namespace RegularizadorPolizas.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public TenantService(
            IApiKeyRepository apiKeyRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _apiKeyRepository = apiKeyRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public string GetCurrentTenantId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var tenantId = httpContext.User.FindFirst("tenant")?.Value;
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("No se encontró información de tenant en el token");
            }

            return tenantId;
        }

        public int GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("No se encontró información válida de usuario en el token");
            }

            return userId;
        }

        public async Task<ApiKey> GetTenantConfigurationAsync(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentException("TenantId no puede ser nulo o vacío", nameof(tenantId));
            }

            var config = await _apiKeyRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
            {
                throw new InvalidOperationException($"No se encontró configuración para el tenant: {tenantId}");
            }

            return config;
        }

        public async Task<ApiKey> GetCurrentTenantConfigurationAsync()
        {
            var tenantId = GetCurrentTenantId();
            return await GetTenantConfigurationAsync(tenantId);
        }

        public async Task<bool> ValidateTenantAccessAsync(string tenantId, int userId)
        {
            if (string.IsNullOrEmpty(tenantId))
                return false;

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.Activo)
                    return false;

                if (user.TenantId != tenantId)
                    return false;

                var tenantExists = await TenantExistsAsync(tenantId);
                return tenantExists;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateCurrentUserTenantAccessAsync(string tenantId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                return await ValidateTenantAccessAsync(tenantId, currentUserId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> BuildVelneoUrlAsync(string tenantId, string endpoint)
        {
            var config = await GetTenantConfigurationAsync(tenantId);

            var baseUrl = config.BaseUrl.TrimEnd('/');

            var cleanEndpoint = endpoint.TrimStart('/');

            return $"{baseUrl}/{cleanEndpoint}";
        }

        public async Task<string> BuildCurrentTenantVelneoUrlAsync(string endpoint)
        {
            var tenantId = GetCurrentTenantId();
            return await BuildVelneoUrlAsync(tenantId, endpoint);
        }

        public async Task<IEnumerable<ApiKey>> GetAllTenantsAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.IsInRole("SuperAdmin") != true)
            {
                throw new UnauthorizedAccessException("Solo los SuperAdmins pueden acceder a todos los tenants");
            }

            return await _apiKeyRepository.GetActiveApiKeysAsync();
        }

        public async Task<bool> TenantExistsAsync(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
                return false;

            var config = await _apiKeyRepository.GetByTenantIdAsync(tenantId);
            return config != null;
        }
    }
}