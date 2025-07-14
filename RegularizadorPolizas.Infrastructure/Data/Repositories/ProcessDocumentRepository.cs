using Microsoft.EntityFrameworkCore;
using AutoMapper;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;
using RegularizadorPolizas.Application.DTOs.Dashboard;
using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class ProcessDocumentRepository : GenericRepository<ProcessDocument>, IProcessDocumentRepository
    {
        private readonly IMapper _mapper;

        public ProcessDocumentRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        // ================================
        // MÉTODOS ORIGINALES - MANTENER IGUAL
        // ================================
        public async Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status)
        {
            return await _context.ProcessDocuments
                .Where(d => d.EstadoProcesamiento == status)
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .Include(d => d.Company) // Agregar Company
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId)
        {
            return await _context.ProcessDocuments
                .Where(d => d.PolizaId == polizaId)
                .Include(d => d.User)
                .Include(d => d.Company) // Agregar Company
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ProcessDocument> GetDocumentWithDetailsAsync(int documentId)
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                    .ThenInclude(p => p != null ? p.Client : null)
                .Include(d => d.User)
                .Include(d => d.Company) // Agregar Company
                .FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public override async Task<IEnumerable<ProcessDocument>> GetAllAsync()
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .Include(d => d.Company) // Agregar Company
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }

        public override async Task<ProcessDocument> GetByIdAsync(int id)
        {
            return await _context.ProcessDocuments
                .Include(d => d.Poliza)
                .Include(d => d.User)
                .Include(d => d.Company) // Agregar Company
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // ================================
        // MÉTODOS NUEVOS PARA LA INTERFACE (nombres exactos)
        // ================================

        // Contadores básicos
        public async Task<int> CountAllDocumentsInRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.ProcessDocuments
                .CountAsync(d => d.FechaCreacion >= fromDate && d.FechaCreacion <= toDate);
        }

        public async Task<int> CountDocumentsByStatusInRangeAsync(string status, DateTime fromDate, DateTime toDate)
        {
            return await _context.ProcessDocuments
                .CountAsync(d => d.EstadoProcesamiento == status &&
                               d.FechaCreacion >= fromDate &&
                               d.FechaCreacion <= toDate);
        }

        // Costos
        public async Task<decimal> GetTotalCostInRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.ProcessDocuments
                .Where(d => d.FechaCreacion >= fromDate &&
                           d.FechaCreacion <= toDate &&
                           d.CostoProcessamiento.HasValue)
                .SumAsync(d => d.CostoProcessamiento.Value);
        }

        // Documentos recientes - CORREGIDO: Retorna ProcessingDocumentDto
        public async Task<List<ProcessingDocumentDto>> GetRecentDocumentsWithLimitAsync(int limit)
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .OrderByDescending(d => d.FechaCreacion)
                .Take(limit)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        public async Task<List<ProcessingDocumentDto>> GetRecentDocumentsByStatusWithLimitAsync(string status, int limit)
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.EstadoProcesamiento == status)
                .OrderByDescending(d => d.FechaCreacion)
                .Take(limit)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        // Por compañía - CORREGIDO: Retorna ProcessingDocumentDto
        public async Task<List<ProcessingDocumentDto>> GetAllDocumentsByCompanyAsync(int companyId)
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.CompaniaId == companyId)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        public async Task<List<ProcessingDocumentDto>> GetDocumentsByCompanyInRangeAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.CompaniaId == companyId &&
                           d.FechaCreacion >= fromDate &&
                           d.FechaCreacion <= toDate)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        // Tiempos de procesamiento
        public async Task<double> GetAverageProcessingTimeForAllAsync()
        {
            var times = await _context.ProcessDocuments
                .Where(d => d.TiempoProcessamiento.HasValue)
                .Select(d => (double)d.TiempoProcessamiento.Value)
                .ToListAsync();

            return times.Any() ? times.Average() : 0;
        }

        public async Task<double> GetAverageProcessingTimeInRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var times = await _context.ProcessDocuments
                .Where(d => d.TiempoProcessamiento.HasValue &&
                           d.FechaCreacion >= fromDate &&
                           d.FechaCreacion <= toDate)
                .Select(d => (double)d.TiempoProcessamiento.Value)
                .ToListAsync();

            return times.Any() ? times.Average() : 0;
        }

        public async Task<double> GetAverageProcessingTimeForCompanyAsync(int companyId)
        {
            var times = await _context.ProcessDocuments
                .Where(d => d.CompaniaId == companyId && d.TiempoProcessamiento.HasValue)
                .Select(d => (double)d.TiempoProcessamiento.Value)
                .ToListAsync();

            return times.Any() ? times.Average() : 0;
        }

        // Estados específicos - CORREGIDO: Retorna ProcessingDocumentDto
        public async Task<List<ProcessingDocumentDto>> GetProcessingDocumentsAsync()
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.EstadoProcesamiento == "PROCESANDO")
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        public async Task<List<ProcessingDocumentDto>> GetPendingDocumentsAsync()
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.EstadoProcesamiento == "PENDIENTE")
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        public async Task<List<ProcessingDocumentDto>> GetCompletedDocumentsAsync()
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.EstadoProcesamiento == "COMPLETADO")
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }

        public async Task<List<ProcessingDocumentDto>> GetErrorDocumentsAsync()
        {
            var documents = await _context.ProcessDocuments
                .Include(d => d.Company)
                .Where(d => d.EstadoProcesamiento == "ERROR")
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();

            return _mapper.Map<List<ProcessingDocumentDto>>(documents);
        }
    }
}