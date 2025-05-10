using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class PolizaRepository : GenericRepository<Poliza>, IPolizaRepository
    {
        public PolizaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Poliza>> GetPolizasByClienteAsync(int clienteId)
        {
            return await _context.Polizas
                .Where(p => p.Clinro == clienteId)
                .ToListAsync();
        }

        public async Task<Poliza> GetPolizaDetalladaAsync(int id)
        {
            return await _context.Polizas
                .Include(p => p.Client)
                .Include(p => p.PolizaPadre)
                .Include(p => p.PolizasHijas)
                .Include(p => p.ProcessDocuments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Poliza>> GetPolizasProximasAVencerAsync(int dias)
        {
            var fechaLimite = DateTime.Now.AddDays(dias);
            return await _context.Polizas
                .Where(p => p.Confchhas <= fechaLimite && p.Confchhas >= DateTime.Now && p.Activo)
                .Include(p => p.Client)
                .ToListAsync();
        }

        public async Task<IEnumerable<Poliza>> GetPolizasVencidasAsync()
        {
            return await _context.Polizas
                .Where(p => p.Confchhas < DateTime.Now && p.Activo)
                .Include(p => p.Client)
                .ToListAsync();
        }

        public async Task<Poliza> GetPolizaByNumeroAsync(string numeroPoliza)
        {
            return await _context.Polizas
                .FirstOrDefaultAsync(p => p.Conpol == numeroPoliza);
        }
    }
}