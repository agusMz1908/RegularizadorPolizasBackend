using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace RegularizadorPolizas.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<TenantService> _logger;

        public TenantService(
            IApiKeyRepository apiKeyRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IAuditService auditService,
            ILogger<TenantService> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _auditService = auditService;
            _logger = logger;
        }

        #region Métodos Existentes

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

        #endregion

        #region Nuevos Métodos para el Switch

        public async Task<bool> UpdateTenantModeAsync(string tenantId, string mode, string? reason = null)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                    throw new ArgumentException("TenantId no puede ser nulo o vacío", nameof(tenantId));

                if (!mode.Equals("VELNEO", StringComparison.OrdinalIgnoreCase) &&
                    !mode.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Mode debe ser 'VELNEO' o 'LOCAL'", nameof(mode));
                }

                var config = await GetTenantConfigurationAsync(tenantId);
                var oldMode = config.Mode;

                config.Mode = mode.ToUpper();
                config.FechaModificacion = DateTime.UtcNow;

                await _apiKeyRepository.UpdateAsync(config);

                int userId = GetCurrentUserId();
                await _auditService.LogAsync(
                    RegularizadorPolizas.Domain.Enums.AuditEventType.ConfigurationChanged,
                    $"Tenant {tenantId} mode changed from {oldMode} to {mode}",
                    new
                    {
                        tenantId,
                        oldMode,
                        newMode = mode,
                        reason,
                        changedBy = userId
                    });

                _logger.LogInformation("Tenant {TenantId} mode updated from {OldMode} to {NewMode} by user {UserId}",
                    tenantId, oldMode, mode, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant {TenantId} mode to {Mode}", tenantId, mode);
                return false;
            }
        }

        public async Task<bool> UpdateCurrentTenantModeAsync(string mode, string? reason = null)
        {
            var tenantId = GetCurrentTenantId();
            return await UpdateTenantModeAsync(tenantId, mode, reason);
        }

        public async Task<ApiKey> CreateTenantConfigurationAsync(CreateTenantConfigDto config)
        {
            try
            {
                if (await TenantExistsAsync(config.TenantId))
                {
                    throw new InvalidOperationException($"Ya existe un tenant con ID: {config.TenantId}");
                }

                var apiKey = new ApiKey
                {
                    TenantId = config.TenantId,
                    Key = config.Key,
                    BaseUrl = config.BaseUrl,
                    Mode = config.Mode.ToUpper(),
                    Environment = config.Environment,
                    Descripcion = config.Description,
                    FechaExpiracion = config.FechaExpiracion,
                    MaxRequestsPerMinute = config.MaxRequestsPerMinute,
                    ContactEmail = config.ContactEmail,
                    EnableLogging = config.EnableLogging,
                    EnableRetries = config.EnableRetries,
                    TimeoutSeconds = config.TimeoutSeconds,
                    ApiVersion = config.ApiVersion,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow
                };

                var created = await _apiKeyRepository.CreateAsync(apiKey);

                int userId = GetCurrentUserId();
                await _auditService.LogAsync(
                    RegularizadorPolizas.Domain.Enums.AuditEventType.ConfigurationChanged,
                    $"New tenant configuration created: {config.TenantId}",
                    new { tenantId = config.TenantId, mode = config.Mode, createdBy = userId });

                _logger.LogInformation("New tenant {TenantId} created with mode {Mode} by user {UserId}",
                    config.TenantId, config.Mode, userId);

                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant configuration for {TenantId}", config.TenantId);
                throw;
            }
        }

        public async Task<bool> DeleteTenantConfigurationAsync(string tenantId)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.IsInRole("SuperAdmin") != true)
                {
                    throw new UnauthorizedAccessException("Solo los SuperAdmins pueden eliminar configuraciones de tenants");
                }

                var config = await GetTenantConfigurationAsync(tenantId);
                await _apiKeyRepository.DeleteAsync(config.Id);

                int userId = GetCurrentUserId();
                await _auditService.LogAsync(
                    RegularizadorPolizas.Domain.Enums.AuditEventType.ConfigurationChanged,
                    $"Tenant configuration deleted: {tenantId}",
                    new { tenantId, deletedBy = userId });

                _logger.LogWarning("Tenant {TenantId} configuration deleted by user {UserId}", tenantId, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tenant configuration for {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<ApiKey> UpdateTenantConfigurationAsync(string tenantId, UpdateTenantConfigDto updateConfig)
        {
            try
            {
                var config = await GetTenantConfigurationAsync(tenantId);

                // Actualizar solo los campos que no son nulos
                if (!string.IsNullOrEmpty(updateConfig.Key))
                    config.Key = updateConfig.Key;
                if (!string.IsNullOrEmpty(updateConfig.BaseUrl))
                    config.BaseUrl = updateConfig.BaseUrl;
                if (!string.IsNullOrEmpty(updateConfig.Mode))
                    config.Mode = updateConfig.Mode.ToUpper();
                if (!string.IsNullOrEmpty(updateConfig.Environment))
                    config.Environment = updateConfig.Environment;
                if (updateConfig.Description != null)
                    config.Descripcion = updateConfig.Description;
                if (updateConfig.FechaExpiracion.HasValue)
                    config.FechaExpiracion = updateConfig.FechaExpiracion;
                if (updateConfig.MaxRequestsPerMinute.HasValue)
                    config.MaxRequestsPerMinute = updateConfig.MaxRequestsPerMinute;
                if (!string.IsNullOrEmpty(updateConfig.ContactEmail))
                    config.ContactEmail = updateConfig.ContactEmail;
                if (updateConfig.EnableLogging.HasValue)
                    config.EnableLogging = updateConfig.EnableLogging.Value;
                if (updateConfig.EnableRetries.HasValue)
                    config.EnableRetries = updateConfig.EnableRetries.Value;
                if (updateConfig.TimeoutSeconds.HasValue)
                    config.TimeoutSeconds = updateConfig.TimeoutSeconds.Value;
                if (!string.IsNullOrEmpty(updateConfig.ApiVersion))
                    config.ApiVersion = updateConfig.ApiVersion;
                if (updateConfig.Activo.HasValue)
                    config.Activo = updateConfig.Activo.Value;

                config.FechaModificacion = DateTime.UtcNow;

                await _apiKeyRepository.UpdateAsync(config);

                int userId = GetCurrentUserId();
                await _auditService.LogAsync(
                    RegularizadorPolizas.Domain.Enums.AuditEventType.ConfigurationChanged,
                    $"Tenant configuration updated: {tenantId}",
                    new { tenantId, updatedFields = updateConfig, updatedBy = userId });

                _logger.LogInformation("Tenant {TenantId} configuration updated by user {UserId}", tenantId, userId);

                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant configuration for {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<bool> TestTenantConnectivityAsync(string tenantId)
        {
            try
            {
                var config = await GetTenantConfigurationAsync(tenantId);

                if (config.Mode.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
                {
                    // Para modo LOCAL, verificamos conectividad local (siempre debería ser true)
                    return true;
                }

                // Para modo VELNEO, probamos conectividad con la API externa
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);

                if (!string.IsNullOrEmpty(config.Key))
                {
                    httpClient.DefaultRequestHeaders.Add("X-API-Key", config.Key);
                }

                var testUrl = $"{config.BaseUrl.TrimEnd('/')}/health";
                var response = await httpClient.GetAsync(testUrl);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Connectivity test failed for tenant {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<bool> TestCurrentTenantConnectivityAsync()
        {
            var tenantId = GetCurrentTenantId();
            return await TestTenantConnectivityAsync(tenantId);
        }

        public async Task<IEnumerable<ApiKey>> GetActiveTenantsByModeAsync(string mode)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.IsInRole("SuperAdmin") != true)
            {
                throw new UnauthorizedAccessException("Solo los SuperAdmins pueden acceder a esta información");
            }

            var allTenants = await GetAllTenantsAsync();
            return allTenants.Where(t => t.Mode.Equals(mode, StringComparison.OrdinalIgnoreCase) && t.Activo);
        }

        public async Task<TenantUsageStatsDto> GetTenantUsageStatsAsync(string tenantId, DateTime? fromDate = null)
        {
            try
            {
                var config = await GetTenantConfigurationAsync(tenantId);
                var startDate = fromDate ?? DateTime.UtcNow.Date;

                // Aquí implementarías la lógica real para obtener estadísticas de audit logs
                // Por ahora devolvemos datos de ejemplo

                return new TenantUsageStatsDto
                {
                    TenantId = tenantId,
                    Mode = config.Mode,
                    FromDate = startDate,
                    ToDate = DateTime.UtcNow,
                    TotalOperations = 150, // Ejemplo
                    VelneoOperations = config.Mode == "VELNEO" ? 120 : 0,
                    LocalOperations = config.Mode == "VELNEO" ? 30 : 150,
                    FailoverEvents = 2,
                    AverageResponseTimeMs = config.Mode == "VELNEO" ? 450 : 120,
                    ErrorCount = 3,
                    OperationsByEntity = new Dictionary<string, int>
                    {
                        ["Client"] = 45,
                        ["Poliza"] = 60,
                        ["Document"] = 30,
                        ["Broker"] = 15
                    },
                    OperationsByType = new Dictionary<string, int>
                    {
                        ["GET"] = 90,
                        ["CREATE"] = 35,
                        ["UPDATE"] = 20,
                        ["DELETE"] = 5
                    },
                    LastModeChange = DateTime.UtcNow.AddDays(-7) // Ejemplo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage stats for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<TenantUsageStatsDto> GetCurrentTenantUsageStatsAsync(DateTime? fromDate = null)
        {
            var tenantId = GetCurrentTenantId();
            return await GetTenantUsageStatsAsync(tenantId, fromDate);
        }

        #endregion
    }
}