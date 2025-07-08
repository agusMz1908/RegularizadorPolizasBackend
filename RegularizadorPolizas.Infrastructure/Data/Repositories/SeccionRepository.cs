using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Infrastructure.Repositories;

namespace RegularizadorPolizas.Infrastructure.Data.Repositories
{
    public class SeccionRepository : GenericRepository<Seccion>, ISeccionRepository
    {
        public SeccionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Seccion>> GetActiveSeccionesAsync()
        {
            return await _dbSet
                .Where(s => s.Activo)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Seccion>> SearchSeccionesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveSeccionesAsync();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(s => s.Activo &&
                           s.Name.ToLower().Contains(lowerSearchTerm))
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Seccion?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var query = _dbSet.Where(s => s.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public override async Task<Seccion> AddAsync(Seccion entity)
        {
            entity.FechaCreacion = DateTime.Now;
            entity.FechaModificacion = DateTime.Now;
            return await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(Seccion entity)
        {
            entity.FechaModificacion = DateTime.Now;
            await base.UpdateAsync(entity);
        }
    }
}