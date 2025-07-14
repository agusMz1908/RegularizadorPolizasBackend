using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;

namespace RegularizadorPolizas.Infrastructure.Repositories
{
    public class ProcessDocumentRepository : GenericRepository<ProcessDocument>, IProcessDocumentRepository
    {
        public ProcessDocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        // ================================
        // MÉTODOS QUE USA ProcessDocumentService
        // ================================

        public async Task<ProcessDocument> GetDocumentWithDetailsAsync(int documentId)
        {
            return await _dbSet
                .Include(pd => pd.Poliza)
                .Include(pd => pd.User)
                .Include(pd => pd.Company)
                .FirstOrDefaultAsync(pd => pd.Id == documentId);
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId)
        {
            return await _dbSet
                .Where(pd => pd.PolizaId == polizaId)
                .OrderByDescending(pd => pd.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(pd => pd.EstadoProcesamiento == status)
                .OrderByDescending(pd => pd.FechaCreacion)
                .ToListAsync();
        }

        // ================================
        // MÉTODOS PARA DASHBOARD - AHORA CON CAMPOS REALES
        // ================================

        public async Task<int> CountDocumentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .CountAsync(d => d.FechaCreacion >= fromDate && d.FechaCreacion <= toDate);
        }

        public async Task<int> CountDocumentsByStatusAndDateAsync(string status, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .CountAsync(d => d.EstadoProcesamiento == status &&
                               d.FechaCreacion >= fromDate &&
                               d.FechaCreacion <= toDate);
        }

        public async Task<decimal> GetTotalCostByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(d => d.FechaCreacion >= fromDate &&
                           d.FechaCreacion <= toDate &&
                           d.CostoProcessamiento.HasValue)
                .SumAsync(d => d.CostoProcessamiento.Value);
        }

        public async Task<List<ProcessDocument>> GetRecentDocumentsAsync(int limit, string status = null)
        {
            var query = _dbSet
                .Include(d => d.Company)
                .OrderByDescending(d => d.FechaCreacion);

            if (!string.IsNullOrEmpty(status))
            {
                query = (IOrderedQueryable<ProcessDocument>)query.Where(d => d.EstadoProcesamiento == status);
            }

            return await query.Take(limit).ToListAsync();
        }

        public async Task<List<ProcessDocument>> GetDocumentsByCompanyAsync(int? companyId, DateTime? fromDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(d => d.CompaniaId == companyId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.FechaCreacion >= fromDate.Value);
            }

            return await query
                .Include(d => d.Company)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public async Task<double> GetAverageProcessingTimeAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet.Where(d => d.TiempoProcessamiento.HasValue);

            if (fromDate.HasValue)
                query = query.Where(d => d.FechaCreacion >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(d => d.FechaCreacion <= toDate.Value);

            var times = await query.Select(d => (double)d.TiempoProcessamiento.Value).ToListAsync();
            return times.Any() ? times.Average() : 0;
        }
    }
}