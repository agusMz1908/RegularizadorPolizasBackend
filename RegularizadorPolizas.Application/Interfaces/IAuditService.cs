using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(AuditEventType eventType, string description, object? additionalData = null);
        Task LogWithUserAsync(AuditEventType eventType, string description, object? oldData, object? newData, int userId);
        Task LogEntityChangeAsync<T>(AuditEventType eventType, T? oldEntity, T? newEntity, int? entityId = null);
        Task LogErrorAsync(Exception ex, string description, object? additionalData = null);
        Task LogLoginAsync(string userName, bool success, string? reason = null);
        Task LogLogoutAsync(string userName);
        Task LogClientActivityAsync(AuditEventType eventType, int clientId, object? oldData = null, object? newData = null);
        Task LogPolicyActivityAsync(AuditEventType eventType, int policyId, object? oldData = null, object? newData = null);
        Task LogBrokerActivityAsync(AuditEventType eventType, int brokerId, object? oldData = null, object? newData = null);
        Task LogCompanyActivityAsync(AuditEventType eventType, int companyId, object? oldData = null, object? newData = null);
        Task LogCurrencyActivityAsync(AuditEventType eventType, int currencyId, object? oldData = null, object? newData = null);
        Task LogRenovationActivityAsync(AuditEventType eventType, int renovationId, object? oldData = null, object? newData = null);
        Task LogDocumentActivityAsync(AuditEventType eventType, int documentId, string fileName, object? additionalData = null);
        Task LogDocumentProcessingAsync(int documentId, string fileName, bool success, string? errorMessage = null, long? durationMs = null);

        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(AuditFilter filter);
        Task<AuditLogDto?> GetAuditLogByIdAsync(long id);
        Task<IEnumerable<AuditLogDto>> GetEntityAuditHistoryAsync(string entityName, int entityId);
        Task<IEnumerable<AuditLogDto>> GetUserActivityAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
