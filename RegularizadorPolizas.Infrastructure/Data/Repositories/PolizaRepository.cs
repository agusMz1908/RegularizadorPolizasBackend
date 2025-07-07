using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;
using System.Linq.Expressions;

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
                .Include(p => p.Company)  
                .Include(p => p.Seccion)    
                .Include(p => p.Currency)  
                .Include(p => p.Client)     
                .Where(p => p.Clinro == clienteId && p.Activo)
                .OrderByDescending(p => p.Confchdes)
                .ToListAsync();
        }

        public async Task<Poliza> GetPolizaDetalladaAsync(int id)
        {
            return await _context.Polizas
                .Include(p => p.Client)
                .Include(p => p.Company)     
                .Include(p => p.Seccion)       
                .Include(p => p.Currency)       
                .Include(p => p.PolizaPadre)
                .Include(p => p.PolizasHijas.Where(h => h.Activo))
                .Include(p => p.ProcessDocuments)
                .Include(p => p.RenovacionesOrigen)
                .Include(p => p.RenovacionesDestino)
                .FirstOrDefaultAsync(p => p.Id == id && p.Activo);
        }

        public async Task<IEnumerable<Poliza>> GetPolizasProximasAVencerAsync(int dias)
        {
            var fechaLimite = DateTime.Now.AddDays(dias);
            return await _context.Polizas
                .Where(p => p.Confchhas <= fechaLimite &&
                           p.Confchhas >= DateTime.Now &&
                           p.Activo)
                .Include(p => p.Client)
                .OrderBy(p => p.Confchhas)
                .ToListAsync();
        }

        public async Task<IEnumerable<Poliza>> GetPolizasVencidasAsync()
        {
            return await _context.Polizas
                .Where(p => p.Confchhas < DateTime.Now &&
                           p.Activo &&
                           p.Convig != "0")
                .Include(p => p.Client)
                .OrderByDescending(p => p.Confchhas)
                .ToListAsync();
        }

        public async Task<Poliza> GetPolizaByNumeroAsync(string numeroPoliza)
        {
            if (string.IsNullOrWhiteSpace(numeroPoliza))
                return null;

            return await _context.Polizas
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Conpol == numeroPoliza && p.Activo);
        }

        public new async Task<IEnumerable<Poliza>> FindAsync(Expression<Func<Poliza, bool>> predicate)
        {
            return await _context.Polizas
                .Where(predicate)
                .Include(p => p.Client)
                .ToListAsync();
        }

        public async Task<Poliza> CreatePolizaFromDocumentAsync(Poliza poliza)
        {
            poliza.FechaCreacion = DateTime.Now;
            poliza.FechaModificacion = DateTime.Now;
            poliza.Activo = true;
            poliza.Procesado = true;
            poliza.TipoOperacion = "NUEVA";
            poliza.OrigenDocumento = "DOCUMENT_INTELLIGENCE";

            await _context.Polizas.AddAsync(poliza);
            await _context.SaveChangesAsync();

            return await GetPolizaDetalladaAsync(poliza.Id);
        }


        public async Task<bool> ExistePolizaAsync(string numeroPoliza, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(numeroPoliza))
                return false;

            var query = _context.Polizas.Where(p => p.Conpol == numeroPoliza && p.Activo);

            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}