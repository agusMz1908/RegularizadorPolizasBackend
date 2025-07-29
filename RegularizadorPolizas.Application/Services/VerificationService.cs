using System.Text.Json;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs.Verification;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IVerificationRepository _verificationRepository;
        private readonly IVelneoApiService _velneoApiService;
        private readonly IPolizaService _polizaService;
        private readonly IUserService _userService;
        private readonly ILogger<VerificationService> _logger;

        public VerificationService(
            IVerificationRepository verificationRepository,
            IVelneoApiService velneoApiService,
            IPolizaService polizaService,
            IUserService userService,
            ILogger<VerificationService> logger)
        {
            _verificationRepository = verificationRepository ?? throw new ArgumentNullException(nameof(verificationRepository));
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveVerificationStatusAsync(VerificationStatusDto status)
        {
            try
            {
                _logger.LogInformation("Saving verification status for poliza {PolizaId} by user {UserId}",
                    status.PolizaId, status.UserId);

                var existingVerification = await _verificationRepository.GetByPolizaIdAsync(status.PolizaId);

                if (existingVerification != null)
                {
                    existingVerification.EstadoGeneral = status.EstadoGeneral;
                    existingVerification.CamposVerificados = JsonSerializer.Serialize(status.CamposVerificados);
                    existingVerification.FechaVerificacion = status.FechaVerificacion;
                    existingVerification.Observaciones = status.Observaciones;
                    existingVerification.FechaActualizacion = DateTime.Now;

                    await _verificationRepository.UpdateAsync(existingVerification);
                }
                else
                {
                    var verification = new PolizaVerification
                    {
                        PolizaId = status.PolizaId,
                        UserId = status.UserId,
                        EstadoGeneral = status.EstadoGeneral,
                        CamposVerificados = JsonSerializer.Serialize(status.CamposVerificados),
                        FechaVerificacion = status.FechaVerificacion,
                        Observaciones = status.Observaciones,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    };

                    await _verificationRepository.AddAsync(verification);
                }

                _logger.LogInformation("Verification status saved successfully for poliza {PolizaId}", status.PolizaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving verification status for poliza {PolizaId}", status.PolizaId);
                throw;
            }
        }

        public async Task<VerificationStatusDto> GetVerificationStatusAsync(string polizaId)
        {
            try
            {
                var verification = await _verificationRepository.GetByPolizaIdAsync(polizaId);

                if (verification == null)
                    return null;

                var camposVerificados = string.IsNullOrEmpty(verification.CamposVerificados)
                    ? new Dictionary<string, VerificationFieldDto>()
                    : JsonSerializer.Deserialize<Dictionary<string, VerificationFieldDto>>(verification.CamposVerificados);

                return new VerificationStatusDto
                {
                    PolizaId = verification.PolizaId,
                    UserId = verification.UserId,
                    NombreUsuario = verification.User?.Nombre ?? "Usuario no encontrado",
                    CamposVerificados = camposVerificados,
                    EstadoGeneral = verification.EstadoGeneral,
                    FechaVerificacion = verification.FechaVerificacion,
                    Observaciones = verification.Observaciones,
                    CamposVerificadosCount = camposVerificados.Count(kv => kv.Value.EsVerificado),
                    CamposConErroresCount = camposVerificados.Count(kv => kv.Value.TieneDiscrepancia),
                    PorcentajeCompletado = camposVerificados.Count > 0
                        ? (decimal)camposVerificados.Count(kv => kv.Value.EsVerificado) / camposVerificados.Count * 100
                        : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verification status for poliza {PolizaId}", polizaId);
                throw;
            }
        }

        public async Task<List<VerificationStatusDto>> GetPendingVerificationsAsync()
        {
            try
            {
                var pendingVerifications = await _verificationRepository.GetPendingVerificationsAsync();

                return pendingVerifications.Select(v => new VerificationStatusDto
                {
                    PolizaId = v.PolizaId,
                    UserId = v.UserId,
                    NombreUsuario = v.User?.Nombre ?? "Usuario no encontrado",
                    EstadoGeneral = v.EstadoGeneral,
                    FechaVerificacion = v.FechaVerificacion,
                    Observaciones = v.Observaciones,
                    CamposVerificados = string.IsNullOrEmpty(v.CamposVerificados)
                        ? new Dictionary<string, VerificationFieldDto>()
                        : JsonSerializer.Deserialize<Dictionary<string, VerificationFieldDto>>(v.CamposVerificados)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending verifications");
                throw;
            }
        }

        public async Task<List<VerificationStatusDto>> GetUserVerificationsAsync(int userId)
        {
            try
            {
                var userVerifications = await _verificationRepository.GetByUserIdAsync(userId);

                return userVerifications.Select(v => new VerificationStatusDto
                {
                    PolizaId = v.PolizaId,
                    UserId = v.UserId,
                    NombreUsuario = v.User?.Nombre ?? "Usuario no encontrado",
                    EstadoGeneral = v.EstadoGeneral,
                    FechaVerificacion = v.FechaVerificacion,
                    Observaciones = v.Observaciones,
                    CamposVerificados = string.IsNullOrEmpty(v.CamposVerificados)
                        ? new Dictionary<string, VerificationFieldDto>()
                        : JsonSerializer.Deserialize<Dictionary<string, VerificationFieldDto>>(v.CamposVerificados)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await _verificationRepository.GetPendingCountAsync();
        }

        public async Task<bool> HasPendingVerificationAsync(string polizaId)
        {
            return await _verificationRepository.HasPendingVerificationAsync(polizaId);
        }

        public async Task MarkAsVerifiedAsync(string polizaId, int userId, string observaciones = null)
        {
            var status = new VerificationStatusDto
            {
                PolizaId = polizaId,
                UserId = userId,
                EstadoGeneral = "verificado",
                FechaVerificacion = DateTime.Now,
                Observaciones = observaciones ?? "Verificado por el usuario",
                CamposVerificados = new Dictionary<string, VerificationFieldDto>()
            };

            await SaveVerificationStatusAsync(status);
        }

        public async Task MarkAsRequiringCorrectionAsync(string polizaId, int userId, string motivo)
        {
            var status = new VerificationStatusDto
            {
                PolizaId = polizaId,
                UserId = userId,
                EstadoGeneral = "requiere_correccion",
                FechaVerificacion = DateTime.Now,
                Observaciones = $"Requiere corrección: {motivo}",
                CamposVerificados = new Dictionary<string, VerificationFieldDto>()
            };

            await SaveVerificationStatusAsync(status);
        }
    }
}