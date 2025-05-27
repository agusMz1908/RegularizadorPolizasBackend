using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company> GetByRucAsync(string comruc);
        Task<Company> GetByAliasAsync(string comalias);
        Task<IEnumerable<Company>> GetActiveCompaniesAsync();
        Task<bool> ExistsByRucAsync(string comruc);
        Task<bool> ExistsByAliasAsync(string comalias);
        Task<IEnumerable<Company>> GetInsuranceCompaniesAsync();
        Task<IEnumerable<Company>> GetBrokerCompaniesAsync();
    }
}