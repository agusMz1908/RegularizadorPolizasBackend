using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IBrokerService
    {
        Task<IEnumerable<BrokerDto>> GetAllBrokersAsync();
        Task<BrokerDto> GetBrokerByIdAsync(int id);
        Task<BrokerDto> GetBrokerByCodigoAsync(string codigo);
        Task<BrokerDto> GetBrokerByEmailAsync(string email);
        Task<IEnumerable<BrokerDto>> GetActiveBrokersAsync();
        Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync();
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id); // Soft delete
        Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
    }
}