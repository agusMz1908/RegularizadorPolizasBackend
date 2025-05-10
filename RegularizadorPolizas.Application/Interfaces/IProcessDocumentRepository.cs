using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

public interface IProcessDocumentRepository : IGenericRepository<ProcessDocument>
{
    Task<IEnumerable<ProcessDocument>> GetDocumentosPorEstadoAsync(string estado);
    Task<IEnumerable<ProcessDocument>> GetDocumentosPorPolizaAsync(int polizaId);
    Task<ProcessDocument> GetDocumentoConPolizaAsync(int id);
}