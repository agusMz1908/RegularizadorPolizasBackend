using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClientRepository : IGenericRepository<Client>
    {

        Task<IEnumerable<Client>> GetClientesConPolizasAsync();
        Task<Client> GetClienteByEmailAsync(string email);
        Task<Client> GetClienteByDocumentoAsync(string documento);
        Task<Client> GetClienteConPolizasAsync(int id);
        new Task<IEnumerable<Client>> FindAsync(Expression<Func<Client, bool>> predicate);
    }
}