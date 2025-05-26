using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IBrokerRepository : IGenericRepository<Broker>
    {
        Task<Broker> GetByCodigoAsync(string codigo);
        Task<Broker> GetByEmailAsync(string email);
        Task<IEnumerable<Broker>> GetActiveBrokersAsync();
        Task<IEnumerable<Broker>> SearchByNameAsync(string searchTerm);
        Task<bool> ExistsByCodigoAsync(string codigo);
        Task<IEnumerable<Broker>> GetBrokersWithPoliciesAsync();
    }
}