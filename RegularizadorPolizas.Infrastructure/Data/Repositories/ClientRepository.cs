﻿using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Client>> GetClientsWithPoliciesAsync()
        {
            return await _context.Clients
                .Where(c => c.Activo)
                .Include(c => c.Polizas.Where(p => p.Activo))
                .ToListAsync();
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Cliemail.ToLower() == email.ToLower() && c.Activo);
        }

        public async Task<Client> GetClientByDocumentAsync(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return null;
            documento = documento.Trim().Replace(".", "").Replace("-", "");

            return await _context.Clients
                .FirstOrDefaultAsync(c =>
                    (c.Cliruc != null && c.Cliruc.Replace(".", "").Replace("-", "") == documento) ||
                    (c.Cliced != null && c.Cliced.Replace(".", "").Replace("-", "") == documento) &&
                    c.Activo);
        }

        public async Task<Client> GetClientWithPoliciesAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Polizas.Where(p => p.Activo))
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }

        public override async Task<IEnumerable<Client>> FindAsync(System.Linq.Expressions.Expression<System.Func<Client, bool>> predicate)
        {
            return await _context.Clients
                .Where(predicate)
                .ToListAsync();
        }

        public override async Task<Client> GetByIdAsync(int id)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }
    }
}