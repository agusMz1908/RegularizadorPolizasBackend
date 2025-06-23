using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto?> GetCompanyByCodeAsync(string code); 
        Task<CompanyDto?> GetCompanyByAliasAsync(string alias);
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);
        Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
    }
}