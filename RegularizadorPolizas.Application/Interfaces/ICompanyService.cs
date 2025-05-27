using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto> GetCompanyByRucAsync(string comruc);
        Task<CompanyDto> GetCompanyByAliasAsync(string comalias);
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyDto>> GetInsuranceCompaniesAsync(); 
        Task<IEnumerable<CompanyDto>> GetBrokerCompaniesAsync(); 
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(CompanyDto companyDto);
        Task DeleteCompanyAsync(int id); 
        Task<bool> ExistsByRucAsync(string comruc, int? excludeId = null);
        Task<bool> ExistsByAliasAsync(string comalias, int? excludeId = null);
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
    }
}