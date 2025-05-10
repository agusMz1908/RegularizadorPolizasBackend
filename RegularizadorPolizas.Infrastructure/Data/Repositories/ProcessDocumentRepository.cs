using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ProcessDocumentRepository : GenericRepository<ProcessDocument>, IProcessDocumentRepository
    {
        public ProcessDocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentosPorEstadoAsync(string estado)
        {
            return await _context.ProcessDocuments
                .Where(d => d.EstadoProcesamiento == estado)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentosPorPolizaAsync(int polizaId)
        {
            return await _context.ProcessDocuments
                .Where(d => d.PolizaId == polizaId)
                .ToListAsync();
        }

        public async Task<ProcessDocument> GetDocumentoConPolizaAsync(int id)
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // Los métodos GetByIdAsync y AddAsync ya están implementados en la clase base GenericRepository
    }
}