using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs.Dashboard;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProcessDocumentRepository _processDocumentRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAzureDocumentIntelligenceService _azureService;
        private readonly IVelneoApiService _velneoService;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IProcessDocumentRepository processDocumentRepository,
            ICompanyRepository companyRepository,
            IAzureDocumentIntelligenceService azureService,
            IVelneoApiService velneoService,
            ILogger<DashboardService> logger)
        {
            _processDocumentRepository = processDocumentRepository;
            _companyRepository = companyRepository;
            _azureService = azureService;
            _velneoService = velneoService;
            _logger = logger;
        }

        public async Task<DashboardOverviewDto> GetOverviewStatsAsync(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);

                var documentsToday = await _processDocumentRepository.CountDocumentsByDateRangeAsync(today, today.AddDays(1));
                var documentsMonth = await _processDocumentRepository.CountDocumentsByDateRangeAsync(monthStart, DateTime.Now);
                var documentsTotal = await _processDocumentRepository.CountDocumentsByDateRangeAsync(DateTime.MinValue, DateTime.MaxValue);

                var costToday = await _processDocumentRepository.GetTotalCostByDateRangeAsync(today, today.AddDays(1));
                var costMonth = await _processDocumentRepository.GetTotalCostByDateRangeAsync(monthStart, DateTime.Now);
                var costTotal = await _processDocumentRepository.GetTotalCostByDateRangeAsync(DateTime.MinValue, DateTime.MaxValue);

                var avgProcessingTime = await _processDocumentRepository.GetAverageProcessingTimeAsync();

                // Calcular tasa de éxito (básico)
                var totalDocs = await _processDocumentRepository.CountDocumentsByDateRangeAsync(monthStart, DateTime.Now);
                var successDocs = await _processDocumentRepository.CountDocumentsByStatusAndDateAsync("COMPLETADO", monthStart, DateTime.Now);
                var successRate = totalDocs > 0 ? (double)successDocs / totalDocs * 100 : 0;

                // Contar compañías activas
                var allCompanies = await _companyRepository.GetAllAsync();
                var activeCompanies = allCompanies.Count(); // Ajustar según tu lógica de "activa"

                return new DashboardOverviewDto
                {
                    DocumentsToday = documentsToday,
                    DocumentsMonth = documentsMonth,
                    DocumentsTotal = documentsTotal,
                    CostToday = costToday,
                    CostMonth = costMonth,
                    CostTotal = costTotal,
                    AvgSuccessRate = successRate,
                    AvgProcessingTime = avgProcessingTime,
                    ActiveCompanies = activeCompanies,
                    LastUpdate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overview stats");
                throw;
            }
        }

        public async Task<List<CompanyStatsDto>> GetCompanyStatsAsync(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);

                var companies = await _companyRepository.GetAllAsync();
                var result = new List<CompanyStatsDto>();

                foreach (var company in companies)
                {
                    var docsToday = await _processDocumentRepository.CountDocumentsByDateRangeAsync(today, today.AddDays(1));
                    var docsMonth = (await _processDocumentRepository.GetDocumentsByCompanyAsync(company.Id, monthStart)).Count;

                    var costToday = await _processDocumentRepository.GetTotalCostByDateRangeAsync(today, today.AddDays(1));
                    var costMonth = await _processDocumentRepository.GetTotalCostByDateRangeAsync(monthStart, DateTime.Now);

                    var avgTime = await _processDocumentRepository.GetAverageProcessingTimeAsync(monthStart);

                    // Calcular tasa de éxito para esta compañía (simplificado)
                    var companyDocs = await _processDocumentRepository.GetDocumentsByCompanyAsync(company.Id, monthStart);
                    var successfulDocs = companyDocs.Count(d => d.EsExitoso);
                    var successRate = companyDocs.Count > 0 ? (double)successfulDocs / companyDocs.Count * 100 : 0;

                    result.Add(new CompanyStatsDto
                    {
                        CompanyId = company.Id.ToString(),
                        CompanyCode = company.Codigo ?? "",
                        CompanyName = company.Nombre ?? "",
                        DocumentsToday = docsToday,
                        DocumentsMonth = docsMonth,
                        CostToday = costToday,
                        CostMonth = costMonth,
                        SuccessRate = successRate,
                        AvgProcessingTime = avgTime,
                        LastProcessed = companyDocs.OrderByDescending(d => d.FechaCreacion).FirstOrDefault()?.FechaCreacion,
                        IsActive = true // Ajustar según tu lógica
                    });
                }

                return result.OrderByDescending(c => c.DocumentsMonth).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company stats");
                throw;
            }
        }

        public async Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit, string status)
        {
            try
            {
                var documents = await _processDocumentRepository.GetRecentDocumentsAsync(limit, status);

                return documents.Select(d => new RecentActivityDto
                {
                    Id = d.Id.ToString(),
                    DocumentName = d.NombreArchivo,
                    CompanyCode = d.CodigoCompania ?? "",
                    CompanyName = d.Company?.Nombre ?? "",
                    Status = d.EstadoActual, // Usar la propiedad calculada
                    Timestamp = d.FechaCreacion,
                    ProcessingTime = d.TiempoProcessamiento,
                    ErrorMessage = d.MensajeError,
                    Cost = d.CostoProcessamiento,
                    PageCount = d.NumeroPaginas ?? 0
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activity");
                throw;
            }
        }

        public async Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(int days)
        {
            try
            {
                var cutoffDate = DateTime.Today.AddDays(-days);
                var documents = await _processDocumentRepository.GetDocumentsByCompanyAsync(null, cutoffDate);

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
                    DailyMetrics = await GetDailyMetricsAsync(cutoffDate, documents)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance metrics");
                throw;
            }
        }

        public async Task<RealTimeStatsDto> GetRealTimeStatsAsync()
        {
            try
            {
                var processingDocs = await _processDocumentRepository.GetDocumentsByStatusAsync("PROCESANDO");

                return new RealTimeStatsDto
                {
                    DocumentsProcessingNow = processingDocs.Count(),
                    QueueLength = (await _processDocumentRepository.GetDocumentsByStatusAsync("PENDIENTE")).Count(),
                    CurrentProcessing = processingDocs.Select(d => new ProcessingDocumentDto
                    {
                        Id = d.Id.ToString(),
                        FileName = d.NombreArchivo,
                        CompanyCode = d.CodigoCompania ?? "",
                        StartTime = d.FechaInicioProcesamiento ?? d.FechaCreacion,
                        CurrentStage = DetermineCurrentStage(d)
                    }).ToList(),
                    LastUpdate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time stats");
                throw;
            }
        }

        public async Task<ServiceHealthDto> GetServicesHealthAsync()
        {
            try
            {
                var azureHealthy = await TestAzureHealthAsync();
                var velneoHealthy = await TestVelneoHealthAsync();

                return new ServiceHealthDto
                {
                    AzureDocumentIntelligence = new ServiceStatusDto
                    {
                        IsHealthy = azureHealthy,
                        Status = azureHealthy ? "healthy" : "unhealthy",
                        ResponseTime = 0, // Implementar medición real
                        LastCheck = DateTime.UtcNow
                    },
                    VelneoApi = new ServiceStatusDto
                    {
                        IsHealthy = velneoHealthy,
                        Status = velneoHealthy ? "healthy" : "unhealthy",
                        ResponseTime = 0, // Implementar medición real
                        LastCheck = DateTime.UtcNow
                    },
                    Database = new ServiceStatusDto
                    {
                        IsHealthy = true, // Simplificado - si llegamos aquí, la DB funciona
                        Status = "healthy",
                        ResponseTime = 0,
                        LastCheck = DateTime.UtcNow
                    },
                    OverallHealthy = azureHealthy && velneoHealthy,
                    LastCheck = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting services health");
                throw;
            }
        }

        // ================================
        // MÉTODOS AUXILIARES PRIVADOS
        // ================================

        private async Task<bool> TestAzureHealthAsync()
        {
            try
            {
                return await _azureService.TestConnectionAsync();
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TestVelneoHealthAsync()
        {
            try
            {
                // Implementar test de conexión a Velneo cuando esté disponible
                return true; // Placeholder
            }
            catch
            {
                return false;
            }
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

        private async Task<List<DailyMetricsDto>> GetDailyMetricsAsync(DateTime fromDate, List<ProcessDocument> documents)
        {
            return await Task.FromResult(
                documents
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
                    .ToList()
            );
        }
    }
}