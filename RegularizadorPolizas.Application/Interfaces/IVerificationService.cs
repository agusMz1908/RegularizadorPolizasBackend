using RegularizadorPolizas.Application.DTOs.Frontend;
using RegularizadorPolizas.Application.DTOs.Verification;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVerificationService
    {
        Task SaveVerificationStatusAsync(VerificationStatusDto status);
        Task<VerificationStatusDto> GetVerificationStatusAsync(string polizaId);
        Task<VelneoSendResultDto> ApplyCorrectionsAsync(PolizaCorrectionDto corrections);
        Task<List<VerificationStatusDto>> GetPendingVerificationsAsync();
        Task<List<VerificationStatusDto>> GetUserVerificationsAsync(int userId);
        Task<int> GetPendingCountAsync();
        Task<bool> HasPendingVerificationAsync(string polizaId);
        Task MarkAsVerifiedAsync(string polizaId, int userId, string observaciones = null);
        Task MarkAsRequiringCorrectionAsync(string polizaId, int userId, string motivo);
    }
}