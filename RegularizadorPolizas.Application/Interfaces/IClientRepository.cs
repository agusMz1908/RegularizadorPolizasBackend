using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<IEnumerable<Client>> GetClientsWithPoliciesAsync();
        Task<Client> GetClientByEmailAsync(string email);
        Task<Client> GetClientByDocumentAsync(string documento);
        Task<Client> GetClientWithPoliciesAsync(int id);
    }
}