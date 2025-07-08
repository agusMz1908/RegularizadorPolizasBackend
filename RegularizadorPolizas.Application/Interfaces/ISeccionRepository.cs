using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ISeccionRepository : IGenericRepository<Seccion>
    {
        Task<IEnumerable<Seccion>> GetActiveSeccionesAsync();
        Task<IEnumerable<Seccion>> SearchSeccionesAsync(string searchTerm);
        Task<Seccion?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(string name, int? excludeId = null);
    }
}