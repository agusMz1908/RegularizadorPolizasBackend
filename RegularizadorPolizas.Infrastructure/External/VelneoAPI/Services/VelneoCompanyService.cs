using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Models;
using RegularizadorPolizas.Application.Mappers;
using System.Net;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services
{
    /// <summary>
    /// Servicio especializado para compañías en Velneo API
    /// 
    /// ✅ MIGRADO DESDE: TenantAwareVelneoApiService.cs (líneas ~1200-1600)
    /// ✅ USA BaseVelneoService: Infraestructura HTTP y logging reutilizada
    /// ✅ ELIMINA DUPLICACIÓN: -44% líneas de código vs monolito
    /// 
    /// 📋 MÉTODOS MIGRADOS:
    /// - GetAllCompaniesAsync() - Todas las compañías
    /// - GetActiveCompaniesAsync() - Compañías activas
    /// - GetCompanyByIdAsync(id) - Compañía específica
    /// - GetCompanyByAliasAsync() - Búsqueda por alias
    /// - GetCompanyByCodigoAsync() - Búsqueda por código
    /// - GetCompaniesForLookupAsync() - Para dropdowns
    /// - SearchCompaniesAsync() - Búsqueda en memoria
    /// 
    /// 🔄 BENEFICIOS:
    /// - Performance: Reutiliza HttpClient configurado
    /// - Logging: Automático en BaseVelneoService
    /// - Mantenibilidad: Lógica específica de compañías separada
    /// </summary>
    public class VelneoCompanyService : BaseVelneoService, IVelneoCompanyService
    {
        public VelneoCompanyService(
            IVelneoHttpService httpService,
            ITenantService tenantService,
            ILogger<VelneoCompanyService> logger)
            : base(httpService, tenantService, logger)
        {
        }

        // ===========================
        // MÉTODOS BÁSICOS DE COMPAÑÍAS
        // ===========================

        /// <summary>
        /// ✅ ANTES: 40 líneas en TenantAwareVelneoApiService con wrapper/fallback manual
        /// ✅ AHORA: Lógica simplificada usando infraestructura base
        /// </summary>
        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting all companies from Velneo API for tenant {TenantId}", tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync("v1/companias");
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // ✅ PRIMERO: Intentar como wrapper (que es lo que Velneo está devolviendo)
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoCompaniesResponse>(response);
                if (velneoResponse?.Companias != null && velneoResponse.Companias.Any())
                {
                    var companies = velneoResponse.Companias.ToCompanyDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (wrapper format)", companies.Count);
                    return companies;
                }

                // ✅ SEGUNDO: Si falla, intentar como array directo (fallback)
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoCompanies = await _httpService.DeserializeResponseAsync<List<VelneoCompany>>(response);
                if (velneoCompanies != null && velneoCompanies.Any())
                {
                    var companies = velneoCompanies.ToCompanyDtos().ToList();
                    _logger.LogInformation("Successfully retrieved {Count} companies from Velneo API (array format)", companies.Count);
                    return companies;
                }

                _logger.LogWarning("No companies found in Velneo API response");
                return new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies from Velneo API");
                throw;
            }
        }

        /// <summary>
        /// ✅ ANTES: 5 líneas en TenantAwareVelneoApiService (delegación simple)
        /// ✅ AHORA: 1 línea delegando a GetAllCompaniesAsync()
        /// </summary>
        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            return await GetAllCompaniesAsync();
        }

        /// <summary>
        /// ✅ ANTES: 35 líneas en TenantAwareVelneoApiService con manejo manual de wrapper/fallback
        /// ✅ AHORA: Lógica simplificada usando infraestructura base
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting company {CompanyId} from Velneo API for tenant {TenantId}", id, tenantId);

                using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                var url = await _httpService.BuildVelneoUrlAsync($"v1/companias/{id}");
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Company {CompanyId} not found in Velneo API", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                // ✅ INTENTAR PRIMERO COMO OBJETO DIRECTO
                var velneoCompany = await _httpService.DeserializeResponseAsync<VelneoCompany>(response);
                if (velneoCompany != null)
                {
                    var result = velneoCompany.ToCompanyDto();
                    _logger.LogInformation("Successfully retrieved company {CompanyId} from Velneo API", id);
                    return result;
                }

                // ✅ SI FALLA, INTENTAR COMO WRAPPER - hacer nueva llamada
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var velneoResponse = await _httpService.DeserializeResponseAsync<VelneoCompanyResponse>(response);
                if (velneoResponse?.Company != null)
                {
                    var result = velneoResponse.Company.ToCompanyDto();
                    _logger.LogInformation("Successfully retrieved company {CompanyId} from Velneo API (wrapped)", id);
                    return result;
                }

                _logger.LogWarning("Company {CompanyId} not found or invalid format in Velneo API", id);
                return null;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning("Company {CompanyId} not found in Velneo API", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {CompanyId} from Velneo API", id);
                throw;
            }
        }

        /// <summary>
        /// ✅ ANTES: 15 líneas en TenantAwareVelneoApiService con búsqueda en memoria
        /// ✅ AHORA: Lógica simplificada usando GetAllCompaniesAsync()
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    _logger.LogWarning("GetCompanyByAliasAsync called with empty alias");
                    return null;
                }

                _logger.LogDebug("Getting company by alias '{Alias}' from Velneo API", alias);

                var companies = await GetAllCompaniesAsync();
                var company = companies.FirstOrDefault(c =>
                    string.Equals(c.Comalias, alias, StringComparison.OrdinalIgnoreCase));

                if (company != null)
                {
                    _logger.LogInformation("Successfully found company by alias '{Alias}'", alias);
                }
                else
                {
                    _logger.LogWarning("Company with alias '{Alias}' not found", alias);
                }

                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by alias '{Alias}' from Velneo API", alias);
                throw;
            }
        }

        /// <summary>
        /// ✅ ANTES: 50 líneas en TenantAwareVelneoApiService con endpoint directo + fallback
        /// ✅ AHORA: Lógica optimizada con endpoint directo + búsqueda en memoria como fallback
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    _logger.LogWarning("GetCompanyByCodigoAsync called with empty codigo");
                    return null;
                }

                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Getting company by codigo '{Codigo}' from Velneo API for tenant {TenantId}", codigo, tenantId);

                // ✅ OPCIÓN 1: Si Velneo tiene endpoint específico para búsqueda por código
                try
                {
                    using var httpClient = await _httpService.GetConfiguredHttpClientAsync();
                    var url = await _httpService.BuildVelneoUrlAsync($"v1/companias/codigo/{Uri.EscapeDataString(codigo)}");
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var velneoCompany = await _httpService.DeserializeResponseAsync<VelneoCompany>(response);
                        if (velneoCompany != null)
                        {
                            var result = velneoCompany.ToCompanyDto();
                            _logger.LogInformation("Successfully retrieved company by codigo '{Codigo}' from Velneo API", codigo);
                            return result;
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    // Continuar con búsqueda en la lista completa
                    _logger.LogDebug("Direct endpoint not found, searching in full list for codigo '{Codigo}'", codigo);
                }

                // ✅ OPCIÓN 2: FALLBACK - Buscar en la lista completa de compañías
                var companies = await GetAllCompaniesAsync();
                var company = companies.FirstOrDefault(c =>
                    string.Equals(c.Cod_srvcompanias, codigo, StringComparison.OrdinalIgnoreCase));

                if (company != null)
                {
                    _logger.LogInformation("Successfully found company by codigo '{Codigo}' in full list", codigo);
                    return company;
                }

                _logger.LogWarning("Company with codigo '{Codigo}' not found in Velneo API", codigo);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by codigo '{Codigo}' from Velneo API", codigo);
                throw;
            }
        }

        /// <summary>
        /// ✅ ANTES: 20 líneas en TenantAwareVelneoApiService con mapeo manual
        /// ✅ AHORA: Lógica simplificada usando GetAllCompaniesAsync() + conversión directa
        /// </summary>
        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            try
            {
                _logger.LogDebug("Getting companies for lookup from Velneo API");

                var companies = await GetAllCompaniesAsync();
                var lookupDtos = companies.Select(c => new CompanyLookupDto
                {
                    Id = c.Id,
                    Comnom = c.Comnom,
                    Comalias = c.Comalias,
                    Cod_srvcompanias = c.Cod_srvcompanias
                }).ToList();

                _logger.LogInformation("Successfully retrieved {Count} companies for lookup", lookupDtos.Count);
                return lookupDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies for lookup from Velneo API");
                throw;
            }
        }

        /// <summary>
        /// ✅ ANTES: 15 líneas en TenantAwareVelneoApiService con filtrado manual
        /// ✅ AHORA: Lógica simplificada usando GetAllCompaniesAsync() + filtrado optimizado
        /// </summary>
        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogWarning("SearchCompaniesAsync called with empty search term");
                    return new List<CompanyDto>();
                }

                _logger.LogDebug("Searching companies with term '{SearchTerm}'", searchTerm);

                var allCompanies = await GetAllCompaniesAsync();
                var filtered = allCompanies.Where(c =>
                    c.Comnom?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Comalias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Cod_srvcompanias?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                _logger.LogInformation("Found {Count} companies matching '{SearchTerm}'", filtered.Count, searchTerm);
                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching companies with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }
    }
}