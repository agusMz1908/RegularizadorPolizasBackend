using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAuditRepository
    {
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task<AuditLog?> GetByIdAsync(long id);
        Task<IEnumerable<AuditLog>> GetFilteredAsync(AuditFilter filter);
        Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityName, int entityId);
        Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<AuditLog>> GetByEventTypeAsync(AuditEventType eventType, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<AuditLog>> GetByCategoryAsync(AuditCategory category, DateTime? fromDate = null, DateTime? toDate = null);
        Task<int> GetTotalCountAsync(AuditFilter filter);
        Task<IEnumerable<AuditLog>> GetRecentErrorsAsync(int count = 50);
        Task<IEnumerable<AuditLog>> GetLoginAttemptsAsync(DateTime? fromDate = null, int? take = null);
        Task CleanupOldRecordsAsync(DateTime olderThan);
    }
}