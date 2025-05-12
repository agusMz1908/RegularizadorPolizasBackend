using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class RenovationRepository : GenericRepository<Renovation>, IRenovationRepository
    {
        public RenovationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Renovation>> GetRenovationsByStatusAsync(string status)
        {
            return await _context.Renovations
                .Where(r => r.Estado == status)
                .Include(r => r.PolizaOriginal)
                .Include(r => r.PolizaNueva)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Renovation>> GetRenovationsByPolicyAsync(int polizaId)
        {
            return await _context.Renovations
                .Where(r => r.PolizaId == polizaId || r.PolizaNuevaId == polizaId)
                .Include(r => r.PolizaOriginal)
                .Include(r => r.PolizaNueva)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Renovation> GetRenovationWithDetailsAsync(int id)
        {
            return await _context.Renovations
                .Include(r => r.PolizaOriginal)
                    .ThenInclude(p => p.Client)
                .Include(r => r.PolizaNueva)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}