using RegularizadorPolizas.Application.DTOs;

public interface IVelneoCompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
    Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(int id);
    Task<CompanyDto?> GetCompanyByAliasAsync(string alias);
    Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo);
    Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
    Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
}