using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClientRespository : IGenericRepository<Client>
    {
        Task<IEnumerable<Client>> GetClientesConPolizasAsync();
        Task<Client> GetClienteByEmailAsync(string email);
        Task<Client> GetClienteByDocumentoAsync(string documento);
        Task<Client> GetClienteConPolizasAsync(int id);
    }
}