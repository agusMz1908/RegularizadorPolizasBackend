using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;

namespace RegularizadorPolizas.Infrastructure.Repositories
{
    public class ProcessDocumentRepository : GenericRepository<ProcessDocument>, IProcessDocumentRepository
    {
        public ProcessDocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProcessDocument>> GetProcessDocumentsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(pd => pd.EstadoProcesamiento == status)
                .OrderByDescending(pd => pd.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessDocument>> GetProcessDocumentsByPolicyAsync(int polizaId)
        {
            return await _dbSet
                .Where(pd => pd.PolizaId == polizaId)
                .OrderByDescending(pd => pd.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ProcessDocument> GetProcessDocumentWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(pd => pd.Poliza)
                .Include(pd => pd.User)
                .Include(pd => pd.Company)
                .FirstOrDefaultAsync(pd => pd.Id == id);
        }

        public async Task<DashboardOverviewStatsDto> GetOverviewStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var startDate = fromDate ?? DateTime.MinValue;
            var endDate = toDate ?? DateTime.MaxValue;

            var query = _dbSet.Where(d => d.FechaCreacion >= startDate && d.FechaCreacion <= endDate);

            return new DashboardOverviewStatsDto
            {
                DocumentsToday = await query.CountAsync(d => d.FechaCreacion.Date == today),
                DocumentsMonth = await query.CountAsync(d => d.FechaCreacion >= monthStart),
                DocumentsTotal = await query.CountAsync(),

                CostToday = await query
                    .Where(d => d.FechaCreacion.Date == today && d.CostoProcessamiento.HasValue)
                    .SumAsync(d => d.CostoProcessamiento.Value),

                CostMonth = await query
                    .Where(d => d.FechaCreacion >= monthStart && d.CostoProcessamiento.HasValue)
                    .SumAsync(d => d.CostoProcessamiento.Value),

                CostTotal = await query
                    .Where(d => d.CostoProcessamiento.HasValue)
                    .SumAsync(d => d.CostoProcessamiento.Value),

                AvgSuccessRate = await CalculateSuccessRateAsync(query),
                AvgProcessingTime = await CalculateAvgProcessingTimeAsync(query),

                LastUpdate = DateTime.UtcNow
            };
        }

        public async Task<List<CompanyStatsDto>> GetCompanyStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var startDate = fromDate ?? DateTime.MinValue;
            var endDate = toDate ?? DateTime.MaxValue;

            var query = _dbSet
                .Include(d => d.Company)
                .Where(d => d.FechaCreacion >= startDate && d.FechaCreacion <= endDate && d.CompaniaId.HasValue);

            var companiesWithStats = await query
                .GroupBy(d => new { d.CompaniaId, d.CodigoCompania, d.Company.Comalias })
                .Select(g => new CompanyStatsDto
                {
                    CompanyId = g.Key.CompaniaId.ToString(),
                    CompanyCode = g.Key.CodigoCompania ?? "",
                    CompanyName = g.Key.Comalias ?? "",

                    DocumentsToday = g.Count(d => d.FechaCreacion.Date == today),
                    DocumentsMonth = g.Count(d => d.FechaCreacion >= monthStart),

                    CostToday = g.Where(d => d.FechaCreacion.Date == today && d.CostoProcessamiento.HasValue)
                              .Sum(d => d.CostoProcessamiento.Value),

                    CostMonth = g.Where(d => d.FechaCreacion >= monthStart && d.CostoProcessamiento.HasValue)
                              .Sum(d => d.CostoProcessamiento.Value),

                    LastProcessed = g.Max(d => d.FechaCreacion),
                    IsActive = true
                })
                .OrderByDescending(c => c.DocumentsMonth)
                .ToListAsync();

            foreach (var company in companiesWithStats)
            {
                if (int.TryParse(company.CompanyId, out var companyId))
                {
                    company.SuccessRate = await CalculateCompanySuccessRateAsync(companyId);
                    company.AvgProcessingTime = await CalculateCompanyAvgProcessingTimeAsync(companyId);
                }
            }

            return companiesWithStats;
        }

        public async Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit = 10, string status = null)
        {
            var query = _dbSet
                .Include(d => d.Company)
                .OrderByDescending(d => d.FechaCreacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.EstadoProcesamiento == status);
            }

            return await query
                .Take(limit)
                .Select(d => new RecentActivityDto
                {
                    Id = d.Id.ToString(),
                    DocumentName = d.NombreArchivo,
                    CompanyCode = d.CodigoCompania ?? "",
                    CompanyName = d.Company != null ? d.Company.Comalias : "",
                    Status = d.EstadoActual,
                    Timestamp = d.FechaCreacion,
                    ProcessingTime = d.TiempoProcessamiento,
                    ErrorMessage = d.MensajeError,
                    Cost = d.CostoProcessamiento,
                    PageCount = d.NumeroPaginas ?? 0
                })
                .ToListAsync();
        }

        public async Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(int days = 30)
        {
            var cutoffDate = DateTime.Today.AddDays(-days);

            var documents = await _dbSet
                .Where(d => d.FechaCreacion >= cutoffDate)
                .ToListAsync();

            var processingTimes = documents
                .Where(d => d.TiempoProcessamiento.HasValue)
                .Select(d => (double)d.TiempoProcessamiento.Value)
                .OrderBy(t => t)
                .ToList();

            var totalDocs = documents.Count;
            var successfulDocs = documents.Count(d => d.EsExitoso);

            return new PerformanceMetricsDto
            {
                AvgProcessingTime = processingTimes.Any() ? processingTimes.Average() : 0,
                MedianProcessingTime = processingTimes.Any() ?
                    processingTimes[processingTimes.Count / 2] : 0,
                SuccessRate = totalDocs > 0 ? (double)successfulDocs / totalDocs * 100 : 0,
                TotalDocuments = totalDocs,
                SuccessfulDocuments = successfulDocs,
                FailedDocuments = totalDocs - successfulDocs,
                DailyMetrics = await GetDailyMetricsAsync(cutoffDate)
            };
        }

        public async Task<List<ProcessingDocumentDto>> GetCurrentlyProcessingAsync()
        {
            return await _dbSet
                .Where(d => d.EstadoProcesamiento == "PROCESANDO" ||
                           (d.FechaInicioProcesamiento.HasValue && !d.FechaFinProcesamiento.HasValue))
                .Select(d => new ProcessingDocumentDto
                {
                    Id = d.Id.ToString(),
                    FileName = d.NombreArchivo,
                    CompanyCode = d.CodigoCompania ?? "",
                    StartTime = d.FechaInicioProcesamiento ?? d.FechaCreacion,
                    CurrentStage = DetermineCurrentStage(d)
                })
                .ToListAsync();
        }

        private async Task<double> CalculateSuccessRateAsync(IQueryable<ProcessDocument> query)
        {
            var total = await query.CountAsync();
            if (total == 0) return 0;

            var successful = await query.CountAsync(d =>
                d.EstadoProcesamiento == "COMPLETADO" &&
                string.IsNullOrEmpty(d.MensajeError));

            return (double)successful / total * 100;
        }

        private async Task<double> CalculateAvgProcessingTimeAsync(IQueryable<ProcessDocument> query)
        {
            var times = await query
                .Where(d => d.TiempoProcessamiento.HasValue)
                .Select(d => (double)d.TiempoProcessamiento.Value)
                .ToListAsync();

            return times.Any() ? times.Average() : 0;
        }

        private async Task<double> CalculateCompanySuccessRateAsync(int companyId)
        {
            var total = await _dbSet.CountAsync(d => d.CompaniaId == companyId);
            if (total == 0) return 0;

            var successful = await _dbSet
                .CountAsync(d => d.CompaniaId == companyId && d.EsExitoso);

            return (double)successful / total * 100;
        }

        private async Task<double> CalculateCompanyAvgProcessingTimeAsync(int companyId)
        {
            var times = await _dbSet
                .Where(d => d.CompaniaId == companyId && d.TiempoProcessamiento.HasValue)
                .Select(d => (double)d.TiempoProcessamiento.Value)
                .ToListAsync();

            return times.Any() ? times.Average() : 0;
        }

        private async Task<List<DailyMetricsDto>> GetDailyMetricsAsync(DateTime fromDate)
        {
            return await _dbSet
                .Where(d => d.FechaCreacion >= fromDate)
                .GroupBy(d => d.FechaCreacion.Date)
                .Select(g => new DailyMetricsDto
                {
                    Date = g.Key,
                    DocumentCount = g.Count(),
                    SuccessRate = g.Count() > 0 ?
                        (double)g.Count(d => d.EsExitoso) / g.Count() * 100 : 0,
                    AvgProcessingTime = g.Where(d => d.TiempoProcessamiento.HasValue)
                                      .Average(d => (double?)d.TiempoProcessamiento.Value) ?? 0,
                    TotalCost = g.Where(d => d.CostoProcessamiento.HasValue)
                              .Sum(d => d.CostoProcessamiento.Value)
                })
                .OrderBy(dm => dm.Date)
                .ToListAsync();
        }

        private string DetermineCurrentStage(ProcessDocument document)
        {
            if (document.FechaInicioProcesamiento.HasValue && !document.FechaFinProcesamiento.HasValue)
                return "azure_processing";
            if (document.EstadoProcesamiento == "COMPLETADO" && document.EnviadoVelneo != true)
                return "data_mapping";
            if (document.EnviadoVelneo == true)
                return "velneo_sending";
            return "pending";
        }

        public async Task<int> CountDocumentsByStatusAndDateAsync(string status, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .CountAsync(d => d.EstadoProcesamiento == status &&
                               d.FechaCreacion >= fromDate &&
                               d.FechaCreacion <= toDate);
        }

        public async Task<decimal> GetTotalCostAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(d => d.FechaCreacion >= fromDate &&
                           d.FechaCreacion <= toDate &&
                           d.CostoProcessamiento.HasValue)
                .SumAsync(d => d.CostoProcessamiento.Value);
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

        public async Task<double> GetSuccessRateAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(d => d.FechaCreacion >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(d => d.FechaCreacion <= toDate.Value);

            return await CalculateSuccessRateAsync(query);
        }
    }
}
