using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs([FromQuery] AuditFilter filter)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(filter);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AuditLogDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AuditLogDto>> GetAuditLogById(long id)
        {
            try
            {
                var auditLog = await _auditService.GetAuditLogByIdAsync(id);
                if (auditLog == null)
                    return NotFound($"Audit log with ID {id} not found");

                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("entity/{entityName}/{entityId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetEntityHistory(string entityName, int entityId)
        {
            try
            {
                var history = await _auditService.GetEntityAuditHistoryAsync(entityName, entityId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetUserActivity(int userId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var activity = await _auditService.GetUserActivityAsync(userId, fromDate, toDate);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public ActionResult GetAuditCategories()
        {
            var categories = Enum.GetValues<AuditCategory>()
                .Select(c => new { Value = (int)c, Name = c.ToString() });
            return Ok(categories);
        }

        [HttpGet("event-types")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public ActionResult GetEventTypes()
        {
            var eventTypes = Enum.GetValues<AuditEventType>()
                .Select(e => new { Value = (int)e, Name = e.ToString() });
            return Ok(eventTypes);
        }
    }
}
