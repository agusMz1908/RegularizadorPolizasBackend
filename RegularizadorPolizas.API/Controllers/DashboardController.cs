using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Dashboard;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    // ================================
    // ESTADÍSTICAS GENERALES
    // ================================
    [HttpGet("stats/overview")]
    public async Task<ActionResult<DashboardOverviewDto>> GetOverviewStats(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var stats = await _dashboardService.GetOverviewStatsAsync(fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overview stats");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ================================
    // ESTADÍSTICAS POR COMPAÑÍA
    // ================================
    [HttpGet("stats/companies")]
    public async Task<ActionResult<List<CompanyStatsDto>>> GetCompanyStats(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var stats = await _dashboardService.GetCompanyStatsAsync(fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company stats");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ================================
    // ACTIVIDAD RECIENTE
    // ================================
    [HttpGet("activity/recent")]
    public async Task<ActionResult<List<RecentActivityDto>>> GetRecentActivity(
        [FromQuery] int limit = 10,
        [FromQuery] string status = null)
    {
        try
        {
            var activity = await _dashboardService.GetRecentActivityAsync(limit, status);
            return Ok(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activity");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ================================
    // MÉTRICAS DE PERFORMANCE
    // ================================
    [HttpGet("metrics/performance")]
    public async Task<ActionResult<PerformanceMetricsDto>> GetPerformanceMetrics(
        [FromQuery] int days = 30)
    {
        try
        {
            var metrics = await _dashboardService.GetPerformanceMetricsAsync(days);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ================================
    // ESTADÍSTICAS EN TIEMPO REAL
    // ================================
    [HttpGet("realtime/processing")]
    public async Task<ActionResult<RealTimeStatsDto>> GetRealTimeProcessingStats()
    {
        try
        {
            var stats = await _dashboardService.GetRealTimeStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting real-time stats");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // ================================
    // HEALTH CHECK PARA SERVICIOS
    // ================================
    [HttpGet("health/services")]
    public async Task<ActionResult<ServiceHealthDto>> GetServicesHealth()
    {
        try
        {
            var health = await _dashboardService.GetServicesHealthAsync();
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services health");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}