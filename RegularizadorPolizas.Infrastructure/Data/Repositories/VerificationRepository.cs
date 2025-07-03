using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class VerificationRepository : GenericRepository<PolizaVerification>, IVerificationRepository
    {
        public VerificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PolizaVerification> GetByPolizaIdAsync(string polizaId)
        {
            return await _context.PolizaVerifications
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.PolizaId == polizaId && v.Activo);
        }

        public async Task<List<PolizaVerification>> GetPendingVerificationsAsync()
        {
            return await _context.PolizaVerifications
                .Include(v => v.User)
                .Where(v => v.EstadoGeneral == "pendiente" && v.Activo)
                .OrderBy(v => v.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<PolizaVerification>> GetByUserIdAsync(int userId)
        {
            return await _context.PolizaVerifications
                .Include(v => v.User)
                .Where(v => v.UserId == userId && v.Activo)
                .OrderByDescending(v => v.FechaVerificacion)
                .ToListAsync();
        }

        public async Task<List<PolizaVerification>> GetVerificationsByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.PolizaVerifications
                .Include(v => v.User)
                .Where(v => v.FechaVerificacion >= from &&
                           v.FechaVerificacion <= to &&
                           v.Activo)
                .OrderByDescending(v => v.FechaVerificacion)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await _context.PolizaVerifications
                .CountAsync(v => v.EstadoGeneral == "pendiente" && v.Activo);
        }

        public async Task<bool> HasPendingVerificationAsync(string polizaId)
        {
            return await _context.PolizaVerifications
                .AnyAsync(v => v.PolizaId == polizaId &&
                              v.EstadoGeneral == "pendiente" &&
                              v.Activo);
        }
    }
}