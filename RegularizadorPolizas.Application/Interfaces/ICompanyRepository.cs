using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company> GetByCodigoAsync(string codigo);
        Task<Company> GetByAliasAsync(string alias);
        Task<IEnumerable<Company>> GetActiveCompaniesAsync();
        Task<bool> ExistsByCodigoAsync(string codigo);
    }
}