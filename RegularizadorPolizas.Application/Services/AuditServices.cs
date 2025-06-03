using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using RegularizadorPolizas.Application.Interfaces.RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;
        private readonly IMapper _mapper;

        public AuditService(
            IAuditRepository auditRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger,
            IMapper mapper)
        {
            _auditRepository = auditRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task LogAsync(AuditEventType eventType, string description, object? additionalData = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = eventType,
                    Category = GetCategoryFromEventType(eventType),
                    EntityName = "System",
                    Description = description,
                    AdditionalData = additionalData != null ? JsonConvert.SerializeObject(additionalData) : null,
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit event: {EventType}", eventType);
            }
        }

        public async Task LogEntityChangeAsync<T>(AuditEventType eventType, T? oldEntity, T? newEntity, int? entityId = null)
        {
            try
            {
                var entityName = typeof(T).Name;
                var action = GetActionFromEventType(eventType);

                var auditLog = new AuditLog
                {
                    EventType = eventType,
                    Category = AuditCategory.Business,
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = action,
                    Description = $"{action} {entityName}",
                    OldValues = oldEntity != null ? JsonConvert.SerializeObject(oldEntity, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat
                    }) : null,
                    NewValues = newEntity != null ? JsonConvert.SerializeObject(newEntity, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat
                    }) : null,
                    TableName = entityName,
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging entity change for {EntityType}", typeof(T).Name);
            }
        }

        public async Task LogErrorAsync(Exception exception, string description, object? additionalData = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = AuditEventType.SystemError,
                    Category = AuditCategory.System,
                    EntityName = "System",
                    Description = description,
                    ErrorMessage = exception.Message,
                    StackTrace = exception.StackTrace?.Length > 2000 ? exception.StackTrace.Substring(0, 2000) : exception.StackTrace,
                    AdditionalData = additionalData != null ? JsonConvert.SerializeObject(additionalData) : null,
                    Success = false,
                    Severity = "Error",
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging error audit: {OriginalError}", exception.Message);
            }
        }

        public async Task LogLoginAsync(string userName, bool success, string? reason = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = success ? AuditEventType.Login : AuditEventType.AuthenticationError,
                    Category = AuditCategory.Technical,
                    EntityName = "User",
                    UserName = userName,
                    Description = success ? $"Usuario {userName} inició sesión" : $"Error de autenticación para {userName}",
                    Success = success,
                    ErrorMessage = success ? null : reason,
                    Severity = success ? "Info" : "Warning",
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging login attempt for user: {UserName}", userName);
            }
        }

        public async Task LogLogoutAsync(string userName)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = AuditEventType.Logout,
                    Category = AuditCategory.Technical,
                    EntityName = "User",
                    UserName = userName,
                    Description = $"Usuario {userName} cerró sesión",
                    Severity = "Info",
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging logout for user: {UserName}", userName);
            }
        }

        public async Task LogClientActivityAsync(AuditEventType eventType, int clientId, object? oldData = null, object? newData = null)
        {
            await LogEntityActivityAsync(eventType, "Client", clientId, oldData, newData);
        }

        public async Task LogPolicyActivityAsync(AuditEventType eventType, int policyId, object? oldData = null, object? newData = null)
        {
            await LogEntityActivityAsync(eventType, "Poliza", policyId, oldData, newData);
        }

        public async Task LogBrokerActivityAsync(AuditEventType eventType, int brokerId, object? oldData = null, object? newData = null)
        {
            await LogEntityActivityAsync(eventType, "Broker", brokerId, oldData, newData);
        }

        public async Task LogCompanyActivityAsync(AuditEventType eventType, int companyId, object? oldData = null, object? newData = null)
        {
            await LogEntityActivityAsync(eventType, "Company", companyId, oldData, newData);
        }

        public async Task LogCurrencyActivityAsync(AuditEventType eventType, int currencyId, object? oldData = null, object? newData = null)
        {
            await LogEntityActivityAsync(eventType, "Currency", currencyId, oldData, newData);
        }

        public async Task LogRenovationActivityAsync(AuditEventType eventType, int renovationId, object? oldData = null, object? newData = null)
        {
            await LogEntityActivityAsync(eventType, "Renovation", renovationId, oldData, newData);
        }

        public async Task LogDocumentActivityAsync(AuditEventType eventType, int documentId, string fileName, object? additionalData = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = eventType,
                    Category = AuditCategory.Application,
                    EntityName = "ProcessDocument",
                    EntityId = documentId,
                    Action = GetActionFromEventType(eventType),
                    Description = $"Documento {fileName} - {GetEventTypeDescription(eventType)}",
                    AdditionalData = additionalData != null ? JsonConvert.SerializeObject(additionalData) : null,
                    TableName = "ProcessDocuments",
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging document activity for document: {DocumentId}", documentId);
            }
        }

        public async Task LogDocumentProcessingAsync(int documentId, string fileName, bool success, string? errorMessage = null, long? durationMs = null)
        {
            try
            {
                var eventType = success ? AuditEventType.DocumentProcessed : AuditEventType.DocumentProcessingError;

                var auditLog = new AuditLog
                {
                    EventType = eventType,
                    Category = AuditCategory.Application,
                    EntityName = "ProcessDocument",
                    EntityId = documentId,
                    Action = "PROCESS",
                    Description = $"Procesamiento de documento {fileName} - {(success ? "Exitoso" : "Error")}",
                    Success = success,
                    ErrorMessage = errorMessage,
                    DurationMs = durationMs,
                    Severity = success ? "Info" : "Error",
                    TableName = "ProcessDocuments",
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging document processing for document: {DocumentId}", documentId);
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(AuditFilter filter)
        {
            var auditLogs = await _auditRepository.GetFilteredAsync(filter);
            return _mapper.Map<IEnumerable<AuditLogDto>>(auditLogs);
        }

        public async Task<AuditLogDto?> GetAuditLogByIdAsync(long id)
        {
            var auditLog = await _auditRepository.GetByIdAsync(id);
            return auditLog != null ? _mapper.Map<AuditLogDto>(auditLog) : null;
        }

        public async Task<IEnumerable<AuditLogDto>> GetEntityAuditHistoryAsync(string entityName, int entityId)
        {
            var auditLogs = await _auditRepository.GetEntityHistoryAsync(entityName, entityId);
            return _mapper.Map<IEnumerable<AuditLogDto>>(auditLogs);
        }

        public async Task<IEnumerable<AuditLogDto>> GetUserActivityAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var auditLogs = await _auditRepository.GetUserActivityAsync(userId, fromDate, toDate);
            return _mapper.Map<IEnumerable<AuditLogDto>>(auditLogs);
        }

        private async Task LogEntityActivityAsync(AuditEventType eventType, string entityName, int entityId, object? oldData, object? newData)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = eventType,
                    Category = AuditCategory.Business,
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = GetActionFromEventType(eventType),
                    Description = $"{GetActionFromEventType(eventType)} {entityName} con ID {entityId}",
                    OldValues = oldData != null ? JsonConvert.SerializeObject(oldData, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }) : null,
                    NewValues = newData != null ? JsonConvert.SerializeObject(newData, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }) : null,
                    TableName = GetTableNameFromEntity(entityName),
                    Timestamp = DateTime.UtcNow
                };

                PopulateUserAndRequestInfo(auditLog);
                await _auditRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging {EntityName} activity: {EventType}", entityName, eventType);
            }
        }

        private void PopulateUserAndRequestInfo(AuditLog auditLog)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Usuario
                var userIdClaim = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    auditLog.UserId = userId;
                }

                var userNameClaim = httpContext.User?.FindFirst(ClaimTypes.Name);
                if (userNameClaim != null)
                {
                    auditLog.UserName = userNameClaim.Value;
                }

                auditLog.IpAddress = GetClientIpAddress(httpContext);
                auditLog.UserAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault()?.Substring(0, Math.Min(500, httpContext.Request.Headers["User-Agent"].FirstOrDefault()?.Length ?? 0)) ?? "";
                auditLog.SessionId = httpContext.Session?.Id;
                auditLog.RequestId = httpContext.TraceIdentifier;
            }
        }

        private string GetClientIpAddress(HttpContext httpContext)
        {
            var xForwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                return xForwardedFor.Split(',').FirstOrDefault()?.Trim() ?? "";
            }

            var xRealIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        }

        private AuditCategory GetCategoryFromEventType(AuditEventType eventType)
        {
            return (int)eventType switch
            {
                >= 1 and < 100 => AuditCategory.Technical,
                >= 100 and < 1000 => AuditCategory.Business,
                >= 1000 and < 9000 => AuditCategory.Application,
                >= 9000 => AuditCategory.System,
                _ => AuditCategory.System
            };
        }

        private string GetActionFromEventType(AuditEventType eventType)
        {
            return eventType.ToString() switch
            {
                var s when s.Contains("Created") => "CREATE",
                var s when s.Contains("Updated") => "UPDATE",
                var s when s.Contains("Deleted") => "DELETE",
                var s when s.Contains("Processed") => "PROCESS",
                var s when s.Contains("Cancelled") => "CANCEL",
                var s when s.Contains("Linked") => "LINK",
                var s when s.Contains("Uploaded") => "UPLOAD",
                var s when s.Contains("Login") => "LOGIN",
                var s when s.Contains("Logout") => "LOGOUT",
                _ => "OTHER"
            };
        }

        private string GetEventTypeDescription(AuditEventType eventType)
        {
            var field = eventType.GetType().GetField(eventType.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? eventType.ToString();
        }

        private string GetTableNameFromEntity(string entityName)
        {
            return entityName switch
            {
                "Client" => "Clients",
                "Poliza" => "Polizas",
                "Broker" => "Brokers",
                "Company" => "Companies",
                "Currency" => "Currencies",
                "Renovation" => "Renovations",
                "ProcessDocument" => "ProcessDocuments",
                "User" => "Users",
                _ => entityName
            };
        }
    }
}