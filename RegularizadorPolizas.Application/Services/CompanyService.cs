using AutoMapper;
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

        public CompanyService(
            ICompanyRepository companyRepository,
            IPolizaRepository polizaRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _polizaRepository = polizaRepository ?? throw new ArgumentNullException(nameof(polizaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        public async Task<CompanyDto> GetCompanyByRucAsync(string comruc)
        {
            try
            {
                var company = await _companyRepository.GetByRucAsync(comruc);
                if (company == null)
                    return null;

                var companyDto = _mapper.Map<CompanyDto>(company);
                companyDto.TotalPolizas = await GetPolizasCountAsync(company.Id);

                return companyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving company with RUC {comruc}: {ex.Message}", ex);
            }
        }

        public async Task<CompanyDto> GetCompanyByAliasAsync(string comalias)
        {
            try
            {
                var company = await _companyRepository.GetByAliasAsync(comalias);
                if (company == null)
                    return null;

                var companyDto = _mapper.Map<CompanyDto>(company);
                companyDto.TotalPolizas = await GetPolizasCountAsync(company.Id);

                return companyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving company with alias {comalias}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            try
            {
                var companies = await _companyRepository.GetActiveCompaniesAsync();
                return _mapper.Map<IEnumerable<CompanyDto>>(companies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving active companies: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetInsuranceCompaniesAsync()
        {
            try
            {
                // Obtener solo las compañías que NO son brokers (son aseguradoras)
                var companies = await _companyRepository.FindAsync(c => c.Activo && !c.Broker);
                return _mapper.Map<IEnumerable<CompanyDto>>(companies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving insurance companies: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetBrokerCompaniesAsync()
        {
            try
            {
                // Obtener solo las compañías que SÍ son brokers
                var companies = await _companyRepository.FindAsync(c => c.Activo && c.Broker);
                return _mapper.Map<IEnumerable<CompanyDto>>(companies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving broker companies: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                var companies = await _companyRepository.GetActiveCompaniesAsync();
                return _mapper.Map<IEnumerable<CompanyLookupDto>>(companies);
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
                if (string.IsNullOrWhiteSpace(companyDto.Comnom))
                    throw new ArgumentException("Company name (Comnom) is required");

                if (!string.IsNullOrWhiteSpace(companyDto.Comruc))
                {
                    if (await ExistsByRucAsync(companyDto.Comruc))
                        throw new ArgumentException($"Company with RUC '{companyDto.Comruc}' already exists");
                }

                if (!string.IsNullOrWhiteSpace(companyDto.Comalias))
                {
                    if (await ExistsByAliasAsync(companyDto.Comalias))
                        throw new ArgumentException($"Company with alias '{companyDto.Comalias}' already exists");
                }

                var company = _mapper.Map<Company>(companyDto);
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

                var existingCompany = await _companyRepository.GetByIdAsync(companyDto.Id);
                if (existingCompany == null)
                    throw new ApplicationException($"Company with ID {companyDto.Id} not found");

                if (!string.IsNullOrWhiteSpace(companyDto.Comruc) &&
                    await ExistsByRucAsync(companyDto.Comruc, companyDto.Id))
                    throw new ArgumentException($"Company with RUC '{companyDto.Comruc}' already exists");

                if (!string.IsNullOrWhiteSpace(companyDto.Comalias) &&
                    await ExistsByAliasAsync(companyDto.Comalias, companyDto.Id))
                    throw new ArgumentException($"Company with alias '{companyDto.Comalias}' already exists");

                // Actualizar propiedades siguiendo la estructura de Velneo
                existingCompany.Comnom = companyDto.Comnom;
                existingCompany.Comrazsoc = companyDto.Comrazsoc;
                existingCompany.Comruc = companyDto.Comruc;
                existingCompany.Comdom = companyDto.Comdom;
                existingCompany.Comtel = companyDto.Comtel;
                existingCompany.Comfax = companyDto.Comfax;
                existingCompany.Comsumodia = companyDto.Comsumodia;
                existingCompany.Comcntcli = companyDto.Comcntcli;
                existingCompany.Comcntcon = companyDto.Comcntcon;
                existingCompany.Comprepes = companyDto.Comprepes;
                existingCompany.Compredol = companyDto.Compredol;
                existingCompany.Comcomipe = companyDto.Comcomipe;
                existingCompany.Comcomido = companyDto.Comcomido;
                existingCompany.Comtotcomi = companyDto.Comtotcomi;
                existingCompany.Comtotpre = companyDto.Comtotpre;
                existingCompany.Comalias = companyDto.Comalias;
                existingCompany.Comlog = companyDto.Comlog;
                existingCompany.Broker = companyDto.Broker;
                existingCompany.Cod_srvcompanias = companyDto.Cod_srvcompanias;
                existingCompany.No_utiles = companyDto.No_utiles;
                existingCompany.Paq_dias = companyDto.Paq_dias;
                existingCompany.Activo = companyDto.Activo;
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

        public async Task<bool> ExistsByRucAsync(string comruc, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comruc))
                    return false;

                var companies = await _companyRepository.FindAsync(c => c.Comruc == comruc);
                return companies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking company RUC existence: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByAliasAsync(string comalias, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comalias))
                    return false;

                var companies = await _companyRepository.FindAsync(c => c.Comalias.ToLower() == comalias.ToLower());
                return companies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking company alias existence: {ex.Message}", ex);
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
                        c.Comnom.ToLower().Contains(normalizedSearchTerm) ||
                        (c.Comalias != null && c.Comalias.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Comruc != null && c.Comruc.Contains(normalizedSearchTerm)) ||
                        (c.Comrazsoc != null && c.Comrazsoc.ToLower().Contains(normalizedSearchTerm))
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
    }