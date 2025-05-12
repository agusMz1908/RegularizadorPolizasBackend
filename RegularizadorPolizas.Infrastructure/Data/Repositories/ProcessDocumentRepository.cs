using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ProcessDocumentRepository : GenericRepository<ProcessDocument>, IProcessDocumentRepository
    {
        public ProcessDocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status)
        {
            return await _context.ProcessDocuments
                .Where(d => d.EstadoProcesamiento == status)
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId)
        {
            return await _context.ProcessDocuments
                .Where(d => d.PolizaId == polizaId)
                .Include(d => d.User)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ProcessDocument> GetDocumentWithDetailsAsync(int id)
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                    .ThenInclude(p => p != null ? p.Client : null)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public override async Task<IEnumerable<ProcessDocument>> GetAllAsync()
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public override async Task<ProcessDocument> GetByIdAsync(int id)
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}