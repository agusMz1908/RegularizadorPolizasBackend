using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRespository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }
        public Task<IEnumerable<Client>> GetClientesConPolizasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClienteByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClienteByDocumentoAsync(string documento)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClienteConPolizasAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
