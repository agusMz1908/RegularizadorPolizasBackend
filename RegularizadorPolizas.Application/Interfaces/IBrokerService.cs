using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IBrokerService
    {
        Task<IEnumerable<BrokerDto>> GetAllBrokersAsync();
        Task<BrokerDto> GetBrokerByIdAsync(int id);
        Task<BrokerDto> GetBrokerByNameAsync(string name);
        Task<BrokerDto> GetBrokerByTelefonoAsync(string telefono);
        Task<IEnumerable<BrokerDto>> GetActiveBrokersAsync();
        Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync();
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id); // Soft delete
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> ExistsByTelefonoAsync(string telefono, int? excludeId = null);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
    }
}