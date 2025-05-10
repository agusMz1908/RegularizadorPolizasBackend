using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IRenovacionRepository : IGenericRepository<Renovation>
    {
        Task<IEnumerable<Renovation>> GetRenovacionesPorEstadoAsync(string estado);
        Task<IEnumerable<Renovation>> GetRenovacionesPorPolizaAsync(int polizaId);
        Task<Renovation> GetRenovacionDetalladaAsync(int id);
    }
}