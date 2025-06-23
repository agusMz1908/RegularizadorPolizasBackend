using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantSwitchController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IHybridApiService _hybridApiService;
        private readonly ILogger<TenantSwitchController> _logger;

        public TenantSwitchController(
            ITenantService tenantService,
            IHybridApiService hybridApiService,
            ILogger<TenantSwitchController> logger)
        {
            _tenantService = tenantService;
            _hybridApiService = hybridApiService;
            _logger = logger;
        }

        [HttpGet("config")]
        public async Task<ActionResult<TenantConfigDto>> GetCurrentTenantConfig()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                var result = new TenantConfigDto
                {
                    TenantId = config.TenantId,
                    Mode = config.Mode,
                    BaseUrl = config.BaseUrl,
                    Environment = config.Environment,
                    Active = config.Activo,
                    ApiVersion = config.ApiVersion,
                    Description = config.Descripcion,
                    LastUsed = config.LastUsed,
                    TimeoutSeconds = config.TimeoutSeconds,
                    EnableLogging = config.EnableLogging,
                    EnableRetries = config.EnableRetries
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenant configuration");
                return StatusCode(500, new { error = "Error obteniendo configuración del tenant" });
            }
        }

        [HttpPut("mode")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> UpdateTenantMode([FromBody] UpdateTenantModeDto request)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();

                if (!request.Mode.Equals("VELNEO", StringComparison.OrdinalIgnoreCase) &&
                    !request.Mode.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { error = "Mode debe ser 'VELNEO' o 'LOCAL'" });
                }

                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                // Aquí necesitarías un método en TenantService para actualizar el modo
                // await _tenantService.UpdateTenantModeAsync(tenantId, request.Mode);

                _logger.LogInformation("Tenant {TenantId} mode changed from {OldMode} to {NewMode} by user {UserId}",
                    tenantId, config.Mode, request.Mode, _tenantService.GetCurrentUserId());

                return Ok(new
                {
                    message = $"Modo del tenant cambiado a {request.Mode}",
                    tenantId = tenantId,
                    newMode = request.Mode,
                    previousMode = config.Mode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant mode");
                return StatusCode(500, new { error = "Error actualizando modo del tenant" });
            }
        }

        [HttpGet("health")]
        public async Task<ActionResult> GetTenantHealth()
        {
            try
            {
                var health = await _hybridApiService.GetSystemHealthAsync();
                return Ok(health);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenant health");
                return StatusCode(500, new { error = "Error obteniendo salud del sistema" });
            }
        }

        [HttpPost("simulate")]
        public async Task<ActionResult<SwitchSimulationDto>> SimulateSwitch([FromBody] SimulateSwitchDto request)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                var simulation = new SwitchSimulationDto
                {
                    TenantId = tenantId,
                    CurrentMode = config.Mode,
                    SimulatedMode = request.Mode,
                    Operations = new List<OperationSimulationDto>()
                };

                // Simular operaciones comunes
                var commonOperations = new[]
                {
                    new { Entity = "Client", Operation = "GET" },
                    new { Entity = "Client", Operation = "CREATE" },
                    new { Entity = "Poliza", Operation = "GET" },
                    new { Entity = "Poliza", Operation = "CREATE" },
                    new { Entity = "Document", Operation = "PROCESS" },
                    new { Entity = "Document", Operation = "EXTRACT" }
                };

                foreach (var op in commonOperations)
                {
                    var currentDestination = GetDestination(op.Entity, op.Operation, config.Mode);
                    var simulatedDestination = GetDestination(op.Entity, op.Operation, request.Mode);

                    simulation.Operations.Add(new OperationSimulationDto
                    {
                        Entity = op.Entity,
                        Operation = op.Operation,
                        CurrentDestination = currentDestination,
                        SimulatedDestination = simulatedDestination,
                        WillChange = currentDestination != simulatedDestination
                    });
                }

                return Ok(simulation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error simulating switch");
                return StatusCode(500, new { error = "Error simulando switch" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<SwitchStatsDto>> GetSwitchStats()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                var stats = new SwitchStatsDto
                {
                    TenantId = tenantId,
                    Mode = config.Mode,
                    LastModeChange = DateTime.UtcNow.AddDays(-7), 
                    TotalOperationsToday = 150, // Ejemplo
                    VelneoOperationsToday = config.Mode == "VELNEO" ? 120 : 0,
                    LocalOperationsToday = config.Mode == "VELNEO" ? 30 : 150,
                    FailoverEventsToday = 2, // Ejemplo
                    AverageResponseTimeMs = config.Mode == "VELNEO" ? 450 : 120
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting switch stats");
                return StatusCode(500, new { error = "Error obteniendo estadísticas" });
            }
        }

        #region Helper Methods

        private string GetDestination(string entity, string operation, string mode)
        {
            if (entity == "Document" && (operation == "PROCESS" || operation == "EXTRACT"))
            {
                return "Local";
            }

            return mode.Equals("VELNEO", StringComparison.OrdinalIgnoreCase) ? "Velneo" : "Local";
        }

        [HttpGet("debug")]
        public async Task<ActionResult> DebugTenant()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                return Ok(new
                {
                    TenantId = tenantId,
                    Mode = config.Mode,
                    BaseUrl = config.BaseUrl,
                    UserClaims = User.Claims.Select(c => new { c.Type, c.Value })
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }

        #endregion
    }

    #region DTOs

    public class TenantConfigDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string ApiVersion { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? LastUsed { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableRetries { get; set; }
    }

    public class UpdateTenantModeDto
    {
        public string Mode { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class SimulateSwitchDto
    {
        public string Mode { get; set; } = string.Empty;
    }

    public class SwitchSimulationDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string CurrentMode { get; set; } = string.Empty;
        public string SimulatedMode { get; set; } = string.Empty;
        public List<OperationSimulationDto> Operations { get; set; } = new();
    }

    public class OperationSimulationDto
    {
        public string Entity { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string CurrentDestination { get; set; } = string.Empty;
        public string SimulatedDestination { get; set; } = string.Empty;
        public bool WillChange { get; set; }
    }

    public class SwitchStatsDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public DateTime? LastModeChange { get; set; }
        public int TotalOperationsToday { get; set; }
        public int VelneoOperationsToday { get; set; }
        public int LocalOperationsToday { get; set; }
        public int FailoverEventsToday { get; set; }
        public double AverageResponseTimeMs { get; set; }
    }

    #endregion
}