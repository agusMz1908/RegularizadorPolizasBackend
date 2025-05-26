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
                throw new ApplicationException($"Error obtendiendo compañias: {ex.Message}", ex);
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
                throw new ApplicationException($"Error obteniendo compañia por ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<CompanyDto> GetCompanyByCodigoAsync(string codigo)
        {
            try
            {
                var company = await _companyRepository.GetByCodigoAsync(codigo);
                if (company == null)
                    return null;

                var companyDto = _mapper.Map<CompanyDto>(company);
                companyDto.TotalPolizas = await GetPolizasCountAsync(company.Id);

                return companyDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error obteniendo compañia por codigo {codigo}: {ex.Message}", ex);
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
                throw new ApplicationException($"Error obteniendo compañia activas: {ex.Message}", ex);
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
                throw new ApplicationException($"Error obteniendo compañia por lookup: {ex.Message}", ex);
            }
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(companyDto.Nombre))
                    throw new ArgumentException("El nombre de la compañia es obligatorio");

                if (string.IsNullOrWhiteSpace(companyDto.Codigo))
                    throw new ArgumentException("El codigo de la compañia es obligatorio");

                if (await _companyRepository.ExistsByCodigoAsync(companyDto.Codigo))
                    throw new ArgumentException($"Compañia con codigo: '{companyDto.Codigo}' ya existe");

                var company = _mapper.Map<Company>(companyDto);
                company.Activo = true;

                var createdCompany = await _companyRepository.AddAsync(company);
                return _mapper.Map<CompanyDto>(createdCompany);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creando compañia: {ex.Message}", ex);
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
                    throw new ApplicationException($"Compañia con ID: {companyDto.Id} no existe.");

                if (await ExistsByCodigoAsync(companyDto.Codigo, companyDto.Id))
                    throw new ArgumentException($"Compañia con codigo: '{companyDto.Codigo}' ya existe");

                _mapper.Map(companyDto, existingCompany);
                await _companyRepository.UpdateAsync(existingCompany);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error actualizando compañia: {ex.Message}", ex);
            }
        }

        public async Task DeleteCompanyAsync(int id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null)
                    throw new ApplicationException($"Compañia con ID {id} no existe");

                var polizasCount = await GetPolizasCountAsync(id);
                if (polizasCount > 0)
                    throw new ApplicationException($"No se puede eliminar la compañia. La misma tiene {polizasCount} polizas asociadas.");

                company.Activo = false;
                await _companyRepository.UpdateAsync(company);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error eliminando compañia: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            try
            {
                var companies = await _companyRepository.FindAsync(c => c.Codigo == codigo);
                return companies.Any(c => excludeId == null || c.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al comprobar la existencia del código de compañia: {ex.Message}", ex);
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
                        c.Nombre.ToLower().Contains(normalizedSearchTerm) ||
                        c.Alias.ToLower().Contains(normalizedSearchTerm) ||
                        c.Codigo.ToLower().Contains(normalizedSearchTerm)
                    )
                );

                return _mapper.Map<IEnumerable<CompanyDto>>(companies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error buscando compañias: {ex.Message}", ex);
            }
        }

        private async Task<int> GetPolizasCountAsync(int companyId)
        {
            try
            {
                var polizas = await _polizaRepository.FindAsync(p => p.CompanyId == companyId && p.Activo);
                return polizas.Count();
            }
            catch
            {
                return 0;
            }
        }
    }
}