using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IPolizaRepository : IGenericRepository<Poliza>
    {
        Task<IEnumerable<Poliza>> GetPolizasByClienteAsync(int clienteId);
        Task<Poliza> GetPolizaDetalladaAsync(int id);
        Task<IEnumerable<Poliza>> GetPolizasProximasAVencerAsync(int dias);
        Task<IEnumerable<Poliza>> GetPolizasVencidasAsync();
        Task<Poliza> GetPolizaByNumeroAsync(string numeroPoliza);
        new Task<IEnumerable<Poliza>> FindAsync(Expression<Func<Poliza, bool>> predicate);
    }
}