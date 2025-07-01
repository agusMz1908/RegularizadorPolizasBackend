using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IHybridApiService
    {
        Task<CompanyDto?> GetCompanyAsync(int id); 
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);
        Task<CompanyDto?> GetCompanyByCodeAsync(string code);
        Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo); 
        Task<CompanyDto?> GetCompanyByAliasAsync(string alias);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync();
        Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync();
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);

        Task<BrokerDto?> GetBrokerAsync(int id);
        Task<BrokerDto?> GetBrokerByIdAsync(int id);
        Task<BrokerDto?> GetBrokerByEmailAsync(string email);
        Task<BrokerDto?> GetBrokerByCodigoAsync(string codigo);
        Task<IEnumerable<BrokerDto>> GetAllBrokersAsync();
        Task<IEnumerable<BrokerDto>> GetActiveBrokersAsync();
        Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync();
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task UpdateBrokerAsync(BrokerDto brokerDto);
        Task DeleteBrokerAsync(int id);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
        Task<bool> ExistsBrokerByCodigoAsync(string codigo, int? excludeId = null);
        Task<bool> ExistsBrokerByEmailAsync(string email, int? excludeId = null);
    }
}