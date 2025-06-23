using AutoMapper;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            ICompanyRepository companyRepository,
            IPolizaRepository polizaRepository,
            IMapper mapper,
            ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _polizaRepository = polizaRepository ?? throw new ArgumentNullException(nameof(polizaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            try
            {
                var companies = await _companyRepository.GetAllAsync();
                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

                foreach (var companyDto in companiesDto)
                {
                    var polizasCount = await GetPolizasCountAsync(companyDto.Id);
                    companyDto.TotalPolizas = polizasCount;
                }

                return companiesDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving companies: {ex.Message}", ex);
            }
        }

        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null)
                    return null;

                var companyDto = _mapper.Map<CompanyDto>(company);
                companyDto.TotalPolizas = await GetPolizasCountAsync(id);

                return companyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving company with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            try
            {
                var company = await _companyRepository.GetByAliasAsync(alias);
                return _mapper.Map<CompanyDto>(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by alias {Alias}", alias);
                throw;
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            try
            {
                var companies = await _companyRepository.FindAsync(c => c.Activo);
                return _mapper.Map<IEnumerable<CompanyDto>>(companies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving active companies: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                var companies = await GetActiveCompaniesAsync();
                return companies.Select(c => new CompanyLookupDto
                {
                    Id = c.Id,
                    Comnom = c.Comnom,
                    Comalias = c.Comalias,
                    Cod_srvcompanias = c.Cod_srvcompanias
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving companies for lookup: {ex.Message}", ex);
            }
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                ValidateCompanyDto(companyDto);

                if (!string.IsNullOrEmpty(companyDto.Cod_srvcompanias))
                {
                    var existingCompany = await GetCompanyByCodigoAsync(companyDto.Cod_srvcompanias);
                    if (existingCompany != null)
                        throw new ArgumentException($"Company with code '{companyDto.Cod_srvcompanias}' already exists");
                }

                var company = _mapper.Map<Company>(companyDto);

                SyncCompatibilityFields(company);

                company.Activo = true;
                company.FechaCreacion = DateTime.Now;
                company.FechaModificacion = DateTime.Now;

                var createdCompany = await _companyRepository.AddAsync(company);
                return _mapper.Map<CompanyDto>(createdCompany);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating company: {ex.Message}", ex);
            }
        }

        public async Task UpdateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                if (companyDto == null)
                    throw new ArgumentNullException(nameof(companyDto));

                ValidateCompanyDto(companyDto);

                var existingCompany = await _companyRepository.GetByIdAsync(companyDto.Id);
                if (existingCompany == null)
                    throw new ApplicationException($"Company with ID {companyDto.Id} not found");

                if (!string.IsNullOrEmpty(companyDto.Cod_srvcompanias))
                {
                    var existingByCode = await GetCompanyByCodigoAsync(companyDto.Cod_srvcompanias);
                    if (existingByCode != null && existingByCode.Id != companyDto.Id)
                        throw new ArgumentException($"Company with code '{companyDto.Cod_srvcompanias}' already exists");
                }

                _mapper.Map(companyDto, existingCompany);

                SyncCompatibilityFields(existingCompany);

                existingCompany.FechaModificacion = DateTime.Now;

                await _companyRepository.UpdateAsync(existingCompany);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating company: {ex.Message}", ex);
            }
        }

        public async Task DeleteCompanyAsync(int id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null)
                    throw new ApplicationException($"Company with ID {id} not found");

                var polizasCount = await GetPolizasCountAsync(id);
                if (polizasCount > 0)
                    throw new ApplicationException($"Cannot delete company. It has {polizasCount} associated policies");

                company.Activo = false;
                company.FechaModificacion = DateTime.Now;

                await _companyRepository.UpdateAsync(company);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting company: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                    return false;

                var companies = await _companyRepository.FindAsync(c =>
                    c.Cod_srvcompanias == codigo || c.Codigo == codigo);

                return companies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking company code existence: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveCompaniesAsync();

                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                var companies = await _companyRepository.FindAsync(c =>
                    c.Activo && (
                        (c.Comnom != null && c.Comnom.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Nombre != null && c.Nombre.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Comalias != null && c.Comalias.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Alias != null && c.Alias.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Cod_srvcompanias != null && c.Cod_srvcompanias.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Codigo != null && c.Codigo.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Comrazsoc != null && c.Comrazsoc.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Comruc != null && c.Comruc.ToLower().Contains(normalizedSearchTerm))
                    )
                );

                return _mapper.Map<IEnumerable<CompanyDto>>(companies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching companies: {ex.Message}", ex);
            }
        }

        private async Task<int> GetPolizasCountAsync(int companyId)
        {
            try
            {
                var polizas = await _polizaRepository.FindAsync(p => p.Comcod == companyId && p.Activo);
                return polizas.Count();
            }
            catch
            {
                return 0;
            }
        }

        private void ValidateCompanyDto(CompanyDto companyDto)
        {
            if (string.IsNullOrWhiteSpace(companyDto.Comnom))
                throw new ArgumentException("Company name (Comnom) is required");

            if (string.IsNullOrWhiteSpace(companyDto.Cod_srvcompanias))
                throw new ArgumentException("Company code (Cod_srvcompanias) is required");
        }

        private void SyncCompatibilityFields(Company company)
        {
            if (!string.IsNullOrEmpty(company.Comnom))
                company.Nombre = company.Comnom;

            if (!string.IsNullOrEmpty(company.Comalias))
                company.Alias = company.Comalias;

            if (!string.IsNullOrEmpty(company.Cod_srvcompanias))
                company.Codigo = company.Cod_srvcompanias;

            if (string.IsNullOrEmpty(company.Comnom) && !string.IsNullOrEmpty(company.Nombre))
                company.Comnom = company.Nombre;

            if (string.IsNullOrEmpty(company.Comalias) && !string.IsNullOrEmpty(company.Alias))
                company.Comalias = company.Alias;

            if (string.IsNullOrEmpty(company.Cod_srvcompanias) && !string.IsNullOrEmpty(company.Codigo))
                company.Cod_srvcompanias = company.Codigo;
        }
        public async Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo)
        {
            try
            {
                var companies = await _companyRepository.FindAsync(c =>
                    c.Cod_srvcompanias == codigo || c.Codigo == codigo);

                var company = companies.FirstOrDefault();
                if (company == null)
                    return null;

                var companyDto = _mapper.Map<CompanyDto>(company);
                companyDto.TotalPolizas = await GetPolizasCountAsync(company.Id);

                return companyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company with codigo {codigo}", codigo);
                throw new ApplicationException($"Error retrieving company with codigo {codigo}: {ex.Message}", ex);
            }
        }
    }
}