using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Client>> GetClientesConPolizasAsync()
        {
            return await _context.Clients
                .Include(c => c.Polizas)
                .Where(c => c.Activo)
                .ToListAsync();
        }

        public async Task<Client> GetClienteByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Cliemail.ToLower() == email.ToLower() && c.Activo);
        }

        public async Task<Client> GetClienteByDocumentoAsync(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return null;

            // Normalize the document number (remove spaces, dots, etc.)
            documento = documento.Trim().Replace(".", "").Replace("-", "");

            return await _context.Clients
                .FirstOrDefaultAsync(c =>
                    (c.Cliruc != null && c.Cliruc.Replace(".", "").Replace("-", "") == documento) ||
                    (c.Cliced != null && c.Cliced.Replace(".", "").Replace("-", "") == documento) &&
                    c.Activo);
        }

        public async Task<Client> GetClienteConPolizasAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Polizas)
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }

        public new async Task<IEnumerable<Client>> FindAsync(Expression<Func<Client, bool>> predicate)
        {
            return await _context.Clients
                .Where(predicate)
                .ToListAsync();
        }
    }
}