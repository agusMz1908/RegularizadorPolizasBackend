// RegularizadorPolizas.API/Controllers/VerificationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Frontend;
using RegularizadorPolizas.Application.DTOs.Verification;
using RegularizadorPolizas.Application.Interfaces;
using System.Security.Claims;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/verification")]
    [Authorize]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;
        private readonly ILogger<VerificationController> _logger;

        public VerificationController(
            IVerificationService verificationService,
            ILogger<VerificationController> logger)
        {
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("save-status")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> SaveVerificationStatus([FromBody] VerificationStatusDto status)
        {
            try
            {
                if (status == null)
                    return BadRequest("Status de verificación es requerido");

                if (string.IsNullOrEmpty(status.PolizaId))
                    return BadRequest("ID de póliza es requerido");

                if (status.UserId <= 0)
                {
                    status.UserId = GetCurrentUserId();
                }

                if (status.FechaVerificacion == default)
                {
                    status.FechaVerificacion = DateTime.Now;
                }

                _logger.LogInformation("Saving verification status for poliza {PolizaId} by user {UserId}",
                    status.PolizaId, status.UserId);

                await _verificationService.SaveVerificationStatusAsync(status);

                return Ok(new
                {
                    message = "Estado de verificación guardado exitosamente",
                    polizaId = status.PolizaId,
                    estado = status.EstadoGeneral,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving verification status for poliza {PolizaId}", status?.PolizaId);
                return StatusCode(500, $"Error guardando verificación: {ex.Message}");
            }
        }

        [HttpGet("poliza/{polizaId}/status")]
        [ProducesResponseType(typeof(VerificationStatusDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VerificationStatusDto>> GetVerificationStatus(string polizaId)
        {
            try
            {
                if (string.IsNullOrEmpty(polizaId))
                    return BadRequest("ID de póliza es requerido");

                _logger.LogInformation("Getting verification status for poliza {PolizaId}", polizaId);

                var status = await _verificationService.GetVerificationStatusAsync(polizaId);

                if (status == null)
                    return NotFound($"No se encontró verificación para la póliza {polizaId}");

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving verification status for poliza {PolizaId}", polizaId);
                return StatusCode(500, $"Error obteniendo estado de verificación: {ex.Message}");
            }
        }

        [HttpPost("apply-corrections")]
        [ProducesResponseType(typeof(VelneoSendResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VelneoSendResultDto>> ApplyCorrections([FromBody] PolizaCorrectionDto corrections)
        {
            try
            {
                if (corrections == null)
                    return BadRequest("Datos de corrección son requeridos");

                if (string.IsNullOrEmpty(corrections.PolizaId))
                    return BadRequest("ID de póliza es requerido");

                if (corrections.CamposCorregidos == null || corrections.CamposCorregidos.Count == 0)
                    return BadRequest("Al menos un campo corregido es requerido");

                if (corrections.UserId <= 0)
                {
                    corrections.UserId = GetCurrentUserId();
                }

                _logger.LogInformation("Applying corrections for poliza {PolizaId} by user {UserId}. Fields: {FieldCount}",
                    corrections.PolizaId, corrections.UserId, corrections.CamposCorregidos.Count);

                var result = await _verificationService.ApplyCorrectionsAsync(corrections);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying corrections for poliza {PolizaId}", corrections?.PolizaId);
                return StatusCode(500, new VelneoSendResultDto
                {
                    Success = false,
                    ErrorMessage = $"Error aplicando correcciones: {ex.Message}"
                });
            }
        }

        [HttpGet("pending")]
        [ProducesResponseType(typeof(IEnumerable<VerificationStatusDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<VerificationStatusDto>>> GetPendingVerifications()
        {
            try
            {
                _logger.LogInformation("Getting pending verifications list");

                var pendingVerifications = await _verificationService.GetPendingVerificationsAsync();
                return Ok(pendingVerifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending verifications");
                return StatusCode(500, $"Error obteniendo verificaciones pendientes: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<VerificationStatusDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<VerificationStatusDto>>> GetUserVerifications(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("ID de usuario inválido");

                _logger.LogInformation("Getting verifications for user {UserId}", userId);

                var userVerifications = await _verificationService.GetUserVerificationsAsync(userId);
                return Ok(userVerifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verifications for user {UserId}", userId);
                return StatusCode(500, $"Error obteniendo verificaciones del usuario: {ex.Message}");
            }
        }

        [HttpGet("my-verifications")]
        [ProducesResponseType(typeof(IEnumerable<VerificationStatusDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<VerificationStatusDto>>> GetMyVerifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation("Getting verifications for current user {UserId}", userId);

                var userVerifications = await _verificationService.GetUserVerificationsAsync(userId);
                return Ok(userVerifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verifications for current user");
                return StatusCode(500, $"Error obteniendo mis verificaciones: {ex.Message}");
            }
        }

        [HttpGet("pending/count")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<object>> GetPendingCount()
        {
            try
            {
                var count = await _verificationService.GetPendingCountAsync();

                return Ok(new
                {
                    pendingCount = count,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending verifications count");
                return StatusCode(500, $"Error obteniendo contador de pendientes: {ex.Message}");
            }
        }

        [HttpGet("poliza/{polizaId}/has-pending")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<object>> HasPendingVerification(string polizaId)
        {
            try
            {
                if (string.IsNullOrEmpty(polizaId))
                    return BadRequest("ID de póliza es requerido");

                var hasPending = await _verificationService.HasPendingVerificationAsync(polizaId);

                return Ok(new
                {
                    polizaId = polizaId,
                    hasPendingVerification = hasPending,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking pending verification for poliza {PolizaId}", polizaId);
                return StatusCode(500, $"Error verificando estado pendiente: {ex.Message}");
            }
        }

        [HttpPost("poliza/{polizaId}/mark-verified")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> MarkAsVerified(string polizaId, [FromBody] MarkVerifiedDto markVerifiedDto)
        {
            try
            {
                if (string.IsNullOrEmpty(polizaId))
                    return BadRequest("ID de póliza es requerido");

                var userId = GetCurrentUserId();
                var observaciones = markVerifiedDto?.Observaciones ?? "Verificado por el usuario";

                _logger.LogInformation("Marking poliza {PolizaId} as verified by user {UserId}", polizaId, userId);

                await _verificationService.MarkAsVerifiedAsync(polizaId, userId, observaciones);

                return Ok(new
                {
                    message = "Póliza marcada como verificada exitosamente",
                    polizaId = polizaId,
                    userId = userId,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking poliza {PolizaId} as verified", polizaId);
                return StatusCode(500, $"Error marcando como verificada: {ex.Message}");
            }
        }

        [HttpPost("poliza/{polizaId}/mark-correction-required")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> MarkAsRequiringCorrection(string polizaId, [FromBody] MarkCorrectionRequiredDto markCorrectionDto)
        {
            try
            {
                if (string.IsNullOrEmpty(polizaId))
                    return BadRequest("ID de póliza es requerido");

                if (markCorrectionDto == null || string.IsNullOrEmpty(markCorrectionDto.Motivo))
                    return BadRequest("Motivo de corrección es requerido");

                var userId = GetCurrentUserId();

                _logger.LogInformation("Marking poliza {PolizaId} as requiring correction by user {UserId}. Reason: {Reason}",
                    polizaId, userId, markCorrectionDto.Motivo);

                await _verificationService.MarkAsRequiringCorrectionAsync(polizaId, userId, markCorrectionDto.Motivo);

                return Ok(new
                {
                    message = "Póliza marcada como requiere corrección",
                    polizaId = polizaId,
                    userId = userId,
                    motivo = markCorrectionDto.Motivo,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking poliza {PolizaId} as requiring correction", polizaId);
                return StatusCode(500, $"Error marcando como requiere corrección: {ex.Message}");
            }
        }

        #region DTOs auxiliares

        public class MarkVerifiedDto
        {
            public string Observaciones { get; set; }
        }

        public class MarkCorrectionRequiredDto
        {
            public string Motivo { get; set; }
        }

        #endregion

        #region Métodos auxiliares

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;

            _logger.LogWarning("Could not parse user ID from claims");
            return 0;
        }

        #endregion
    }
}