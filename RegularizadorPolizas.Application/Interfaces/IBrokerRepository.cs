using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IBrokerRepository : IGenericRepository<Broker>
    {
        Task<Broker> GetByNameAsync(string name);
        Task<Broker> GetByTelefonoAsync(string telefono);
        Task<IEnumerable<Broker>> GetActiveBrokersAsync();
        Task<IEnumerable<Broker>> SearchByNameAsync(string searchTerm);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByTelefonoAsync(string telefono);
        Task<IEnumerable<Broker>> GetBrokersWithPoliciesAsync();
    }
}