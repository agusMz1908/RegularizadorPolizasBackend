using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVerificationRepository : IGenericRepository<PolizaVerification>
    {
        Task<PolizaVerification> GetByPolizaIdAsync(string polizaId);
        Task<List<PolizaVerification>> GetPendingVerificationsAsync();
        Task<List<PolizaVerification>> GetByUserIdAsync(int userId);
        Task<List<PolizaVerification>> GetVerificationsByDateRangeAsync(DateTime from, DateTime to);
        Task<int> GetPendingCountAsync();
        Task<bool> HasPendingVerificationAsync(string polizaId);
    }
}