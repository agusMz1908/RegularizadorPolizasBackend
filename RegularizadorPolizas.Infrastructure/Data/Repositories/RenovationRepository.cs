using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class RenovacionRepository : GenericRepository<Renovation>, IRenovacionRepository
    {
        public RenovacionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Renovation>> GetRenovacionesPorEstadoAsync(string estado)
        {
            return await _context.Renovations
                .Where(r => r.Estado == estado)
                .Include(r => r.PolizaOriginal)
                .Include(r => r.PolizaNueva)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Renovation>> GetRenovacionesPorPolizaAsync(int polizaId)
        {
            return await _context.Renovations
                .Where(r => r.PolizaId == polizaId || r.PolizaNuevaId == polizaId)
                .Include(r => r.PolizaOriginal)
                .Include(r => r.PolizaNueva)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Renovation> GetRenovacionDetalladaAsync(int id)
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