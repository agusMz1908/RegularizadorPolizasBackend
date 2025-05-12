using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IProcessDocumentRepository : IGenericRepository<ProcessDocument>
    {
        Task<IEnumerable<ProcessDocument>> GetDocumentsByStatusAsync(string status);
        Task<IEnumerable<ProcessDocument>> GetDocumentsByPolizaAsync(int polizaId);
        Task<ProcessDocument> GetDocumentWithDetailsAsync(int id);
    }
}