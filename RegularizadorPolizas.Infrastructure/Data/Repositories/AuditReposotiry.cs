using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            try
            {
                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
                return auditLog;
            }
            catch (Exception)
            {
                return auditLog;
            }
        }

        public async Task<AuditLog?> GetByIdAsync(long id)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AuditLog>> GetFilteredAsync(AuditFilter filter)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (filter.FromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= filter.ToDate.Value);
            }

            if (filter.Category.HasValue)
            {
                query = query.Where(a => a.Category == filter.Category.Value);
            }

            if (filter.EventType.HasValue)
            {
                query = query.Where(a => a.EventType == filter.EventType.Value);
            }

            if (!string.IsNullOrEmpty(filter.EntityName))
            {
                query = query.Where(a => a.EntityName == filter.EntityName);
            }

            if (filter.EntityId.HasValue)
            {
                query = query.Where(a => a.EntityId == filter.EntityId.Value);
            }

            if (filter.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == filter.UserId.Value);
            }

            if (!string.IsNullOrEmpty(filter.UserName))
            {
                query = query.Where(a => a.UserName.Contains(filter.UserName));
            }

            if (!string.IsNullOrEmpty(filter.Action))
            {
                query = query.Where(a => a.Action == filter.Action);
            }

            if (filter.Success.HasValue)
            {
                query = query.Where(a => a.Success == filter.Success.Value);
            }

            if (!string.IsNullOrEmpty(filter.Severity))
            {
                query = query.Where(a => a.Severity == filter.Severity);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(a =>
                    a.Description.Contains(filter.SearchTerm) ||
                    a.UserName.Contains(filter.SearchTerm) ||
                    a.EntityName.Contains(filter.SearchTerm) ||
                    (a.ErrorMessage != null && a.ErrorMessage.Contains(filter.SearchTerm)));
            }

            if (filter.SortBy.ToLower() == "timestamp")
            {
                query = filter.SortDirection.ToUpper() == "ASC"
                    ? query.OrderBy(a => a.Timestamp)
                    : query.OrderByDescending(a => a.Timestamp);
            }
            else if (filter.SortBy.ToLower() == "eventtype")
            {
                query = filter.SortDirection.ToUpper() == "ASC"
                    ? query.OrderBy(a => a.EventType)
                    : query.OrderByDescending(a => a.EventType);
            }
            else if (filter.SortBy.ToLower() == "username")
            {
                query = filter.SortDirection.ToUpper() == "ASC"
                    ? query.OrderBy(a => a.UserName)
                    : query.OrderByDescending(a => a.UserName);
            }
            else
            {
                query = query.OrderByDescending(a => a.Timestamp);
            }

            return await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityName, int entityId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.UserId == userId);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= toDate.Value);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByEventTypeAsync(AuditEventType eventType, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.EventType == eventType);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= toDate.Value);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByCategoryAsync(AuditCategory category, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.Category == category);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= toDate.Value);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(AuditFilter filter)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (filter.FromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= filter.ToDate.Value);
            }

            if (filter.Category.HasValue)
            {
                query = query.Where(a => a.Category == filter.Category.Value);
            }

            if (filter.EventType.HasValue)
            {
                query = query.Where(a => a.EventType == filter.EventType.Value);
            }

            if (!string.IsNullOrEmpty(filter.EntityName))
            {
                query = query.Where(a => a.EntityName == filter.EntityName);
            }

            if (filter.EntityId.HasValue)
            {
                query = query.Where(a => a.EntityId == filter.EntityId.Value);
            }

            if (filter.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == filter.UserId.Value);
            }

            if (!string.IsNullOrEmpty(filter.UserName))
            {
                query = query.Where(a => a.UserName.Contains(filter.UserName));
            }

            if (!string.IsNullOrEmpty(filter.Action))
            {
                query = query.Where(a => a.Action == filter.Action);
            }

            if (filter.Success.HasValue)
            {
                query = query.Where(a => a.Success == filter.Success.Value);
            }

            if (!string.IsNullOrEmpty(filter.Severity))
            {
                query = query.Where(a => a.Severity == filter.Severity);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(a =>
                    a.Description.Contains(filter.SearchTerm) ||
                    a.UserName.Contains(filter.SearchTerm) ||
                    a.EntityName.Contains(filter.SearchTerm) ||
                    (a.ErrorMessage != null && a.ErrorMessage.Contains(filter.SearchTerm)));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentErrorsAsync(int count = 50)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => !a.Success || a.Severity == "Error" || a.Severity == "Critical")
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLoginAttemptsAsync(DateTime? fromDate = null, int? take = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.EventType == AuditEventType.Login || a.EventType == AuditEventType.AuthenticationError);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            }

            query = query.OrderByDescending(a => a.Timestamp);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }

        public async Task CleanupOldRecordsAsync(DateTime olderThan)
        {
            var oldRecords = await _context.AuditLogs
                .Where(a => a.Timestamp < olderThan)
                .ToListAsync();

            if (oldRecords.Any())
            {
                _context.AuditLogs.RemoveRange(oldRecords);
                await _context.SaveChangesAsync();
            }
        }
    }
}