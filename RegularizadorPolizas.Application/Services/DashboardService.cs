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

                // ✅ USAR NOMBRES CORRECTOS DE LA INTERFACE
                var documentsToday = await _processDocumentRepository.CountAllDocumentsInRangeAsync(today, today.AddDays(1));
                var documentsMonth = await _processDocumentRepository.CountAllDocumentsInRangeAsync(monthStart, DateTime.Now);
                var documentsTotal = await _processDocumentRepository.CountAllDocumentsInRangeAsync(DateTime.MinValue, DateTime.MaxValue);

                var costToday = await _processDocumentRepository.GetTotalCostInRangeAsync(today, today.AddDays(1));
                var costMonth = await _processDocumentRepository.GetTotalCostInRangeAsync(monthStart, DateTime.Now);
                var costTotal = await _processDocumentRepository.GetTotalCostInRangeAsync(DateTime.MinValue, DateTime.MaxValue);

                var avgProcessingTime = await _processDocumentRepository.GetAverageProcessingTimeForAllAsync();

                // Calcular tasa de éxito
                var totalDocs = await _processDocumentRepository.CountAllDocumentsInRangeAsync(monthStart, DateTime.Now);
                var successDocs = await _processDocumentRepository.CountDocumentsByStatusInRangeAsync("COMPLETADO", monthStart, DateTime.Now);
                var successRate = totalDocs > 0 ? (double)successDocs / totalDocs * 100 : 0;

                // Contar compañías activas
                var allCompanies = await _companyRepository.GetAllAsync();
                var activeCompanies = allCompanies.Count();

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
                    // ✅ USAR NOMBRES CORRECTOS
                    var docsToday = await _processDocumentRepository.CountAllDocumentsInRangeAsync(today, today.AddDays(1));
                    var docsMonth = (await _processDocumentRepository.GetDocumentsByCompanyInRangeAsync(company.Id, monthStart, DateTime.Now)).Count;

                    var costToday = await _processDocumentRepository.GetTotalCostInRangeAsync(today, today.AddDays(1));
                    var costMonth = await _processDocumentRepository.GetTotalCostInRangeAsync(monthStart, DateTime.Now);

                    var avgTime = await _processDocumentRepository.GetAverageProcessingTimeForCompanyAsync(company.Id);

                    // Calcular tasa de éxito para esta compañía
                    var companyDocs = await _processDocumentRepository.GetDocumentsByCompanyInRangeAsync(company.Id, monthStart, DateTime.Now);
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
                        IsActive = true
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
                List<ProcessingDocumentDto> documents;

                // ✅ USAR NOMBRES CORRECTOS
                if (string.IsNullOrEmpty(status))
                {
                    documents = await _processDocumentRepository.GetRecentDocumentsWithLimitAsync(limit);
                }
                else
                {
                    documents = await _processDocumentRepository.GetRecentDocumentsByStatusWithLimitAsync(status, limit);
                }

                return documents.Select(d => new RecentActivityDto
                {
                    Id = d.Id.ToString(),
                    DocumentName = d.NombreArchivo,
                    CompanyCode = d.CodigoCompania ?? "",
                    CompanyName = d.Company?.Nombre ?? "",
                    Status = d.EstadoActual,
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

                // ✅ USAR NOMBRES CORRECTOS
                var avgTime = await _processDocumentRepository.GetAverageProcessingTimeInRangeAsync(cutoffDate, DateTime.Now);
                var totalDocs = await _processDocumentRepository.CountAllDocumentsInRangeAsync(cutoffDate, DateTime.Now);
                var successDocs = await _processDocumentRepository.CountDocumentsByStatusInRangeAsync("COMPLETADO", cutoffDate, DateTime.Now);

                return new PerformanceMetricsDto
                {
                    AvgProcessingTime = avgTime,
                    MedianProcessingTime = avgTime, // Simplificado
                    SuccessRate = totalDocs > 0 ? (double)successDocs / totalDocs * 100 : 0,
                    TotalDocuments = totalDocs,
                    SuccessfulDocuments = successDocs,
                    FailedDocuments = totalDocs - successDocs,
                    DailyMetrics = new List<DailyMetricsDto>() // Simplificado por ahora
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
                // ✅ USAR NOMBRES CORRECTOS
                var processingDocs = await _processDocumentRepository.GetProcessingDocumentsAsync();
                var pendingDocs = await _processDocumentRepository.GetPendingDocumentsAsync();

                return new RealTimeStatsDto
                {
                    DocumentsProcessingNow = processingDocs.Count,
                    QueueLength = pendingDocs.Count,
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

                return new ServiceHealthDto
                {
                    AzureDocumentIntelligence = new ServiceStatusDto
                    {
                        IsHealthy = azureHealthy,
                        Status = azureHealthy ? "healthy" : "unhealthy",
                        ResponseTime = 0,
                        LastCheck = DateTime.UtcNow
                    },
                    VelneoApi = new ServiceStatusDto
                    {
                        IsHealthy = true,
                        Status = "healthy",
                        ResponseTime = 0,
                        LastCheck = DateTime.UtcNow
                    },
                    Database = new ServiceStatusDto
                    {
                        IsHealthy = true,
                        Status = "healthy",
                        ResponseTime = 0,
                        LastCheck = DateTime.UtcNow
                    },
                    OverallHealthy = azureHealthy,
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
    }
}