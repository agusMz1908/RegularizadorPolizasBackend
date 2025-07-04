﻿using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services.External; 
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.Services
{
    public class TenantAwareHybridService : IHybridApiService
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly IClientService _localClientService;
        private readonly IBrokerService _localBrokerService;
        private readonly ICurrencyService _localCurrencyService;
        private readonly ICompanyService _localCompanyService;
        private readonly IPolizaService _localPolizaService;
        private readonly IAuditService _auditService;
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantAwareHybridService> _logger;

        public TenantAwareHybridService(
            IVelneoApiService velneoApiService,
            IClientService localClientService,
            IBrokerService localBrokerService,
            ICurrencyService localCurrencyService,
            ICompanyService localCompanyService,
            IPolizaService localPolizaService,
            IAuditService auditService,
            ITenantService tenantService,
            ILogger<TenantAwareHybridService> logger)
        {
            _velneoApiService = velneoApiService;
            _localClientService = localClientService;
            _localBrokerService = localBrokerService;
            _localCurrencyService = localCurrencyService;
            _localCompanyService = localCompanyService;
            _localPolizaService = localPolizaService;
            _auditService = auditService;
            _tenantService = tenantService;
            _logger = logger;
        }

        #region Switch Logic

        private async Task<bool> ShouldRouteToVelneoAsync(string entity, string operation)
        {
            try
            {
                _logger.LogWarning("🔍 ShouldRouteToVelneoAsync START: {Entity}.{Operation}", entity, operation);

                var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

                _logger.LogWarning("🔧 Tenant Config Retrieved: TenantId={TenantId}, Mode={Mode}, BaseUrl={BaseUrl}",
                    tenantConfig.TenantId, tenantConfig.Mode, tenantConfig.BaseUrl);

                // Azure IA siempre local
                if (entity == "Document" && (operation == "PROCESS" || operation == "EXTRACT"))
                {
                    _logger.LogWarning("📄 Document IA operations always go Local");
                    return false;
                }

                // Si el tenant está configurado como LOCAL, todo va local
                if (tenantConfig.Mode.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("🏠 Tenant configured as LOCAL, routing to Local");
                    return false;
                }

                // Si el tenant está configurado como VELNEO, todo va a Velneo (excepto Document IA)
                if (tenantConfig.Mode.Equals("VELNEO", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("📡 Tenant configured as VELNEO, routing to Velneo");
                    return true;
                }

                // Fallback: Si no está claro, va local por seguridad
                _logger.LogWarning("⚠️ Tenant {TenantId} has unclear mode '{Mode}', defaulting to Local",
                    tenantConfig.TenantId, tenantConfig.Mode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR in ShouldRouteToVelneoAsync for {Entity}.{Operation}",
                    entity, operation);
                return false;
            }
        }
        #endregion

        #region Client Operations

        public async Task<ClientDto?> GetClientAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Client", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetClientAsync(id),
                    () => _localClientService.GetClientByIdAsync(id),
                    "Client.GET",
                    id);
            }

            return await _localClientService.GetClientByIdAsync(id);
        }

        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto)
        {
            if (await ShouldRouteToVelneoAsync("Client", "CREATE"))
            {
                return await ExecuteWithAudit(
                    () => _velneoApiService.CreateClientAsync(clientDto),
                    AuditEventType.ClientCreated,
                    "Client.CREATE",
                    newData: clientDto);
            }

            return await _localClientService.CreateClientAsync(clientDto);
        }

        public async Task UpdateClientAsync(ClientDto clientDto)
        {
            if (await ShouldRouteToVelneoAsync("Client", "UPDATE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.UpdateClientAsync(clientDto),
                    AuditEventType.ClientUpdated,
                    "Client.UPDATE",
                    newData: clientDto);
            }
            else
            {
                await _localClientService.UpdateClientAsync(clientDto);
            }
        }

        public async Task DeleteClientAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Client", "DELETE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.DeleteClientAsync(id),
                    AuditEventType.ClientDeleted,
                    "Client.DELETE",
                    additionalData: new { ClientId = id });
            }
            else
            {
                await _localClientService.DeleteClientAsync(id);
            }
        }

        public async Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm)
        {
            if (await ShouldRouteToVelneoAsync("Client", "SEARCH"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.SearchClientsAsync(searchTerm),
                    () => _localClientService.SearchClientsAsync(searchTerm),
                    "Client.SEARCH",
                    searchTerm);
            }

            return await _localClientService.SearchClientsAsync(searchTerm);
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            if (await ShouldRouteToVelneoAsync("Client", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetAllClientsAsync(),
                    () => _localClientService.GetAllClientsAsync(),
                    "Client.GETALL",
                    "all_clients");
            }

            return await _localClientService.GetAllClientsAsync();
        }

        #endregion

        #region Poliza Operations

        public async Task<PolizaDto?> GetPolizaAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Poliza", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetPolizaAsync(id),
                    () => _localPolizaService.GetPolizaByIdAsync(id),
                    "Poliza.GET",
                    id);
            }

            return await _localPolizaService.GetPolizaByIdAsync(id);
        }

        public async Task<PolizaCreationResult> CreatePolizaAsync(PolizaDto polizaDto)
        {
            if (await ShouldRouteToVelneoAsync("Poliza", "CREATE"))
            {
                var velneoPoliza = await ExecuteWithAudit(
                    () => _velneoApiService.CreatePolizaAsync(polizaDto),
                    AuditEventType.PolicyCreated,
                    "Poliza.CREATE",
                    newData: polizaDto);

                return new PolizaCreationResult
                {
                    Success = true,
                    Poliza = velneoPoliza,
                    Source = "Velneo",
                    Message = "Póliza creada exitosamente en Velneo"
                };
            }

            var localPoliza = await _localPolizaService.CreatePolizaAsync(polizaDto);
            return new PolizaCreationResult
            {
                Success = true,
                Poliza = localPoliza,
                Source = "Local",
                Message = "Póliza creada exitosamente en base local"
            };
        }

        public async Task UpdatePolizaAsync(PolizaDto polizaDto)
        {
            if (await ShouldRouteToVelneoAsync("Poliza", "UPDATE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.UpdatePolizaAsync(polizaDto),
                    AuditEventType.PolicyUpdated,
                    "Poliza.UPDATE",
                    newData: polizaDto);
            }
            else
            {
                await _localPolizaService.UpdatePolizaAsync(polizaDto);
            }
        }

        public async Task DeletePolizaAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Poliza", "DELETE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.DeletePolizaAsync(id),
                    AuditEventType.PolicyDeleted,
                    "Poliza.DELETE",
                    additionalData: new { PolizaId = id });
            }
            else
            {
                await _localPolizaService.DeletePolizaAsync(id);
            }
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            if (await ShouldRouteToVelneoAsync("Poliza", "SEARCH"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.SearchPolizasAsync(searchTerm),
                    () => _localPolizaService.SearchPolizasAsync(searchTerm),
                    "Poliza.SEARCH",
                    searchTerm);
            }

            return await _localPolizaService.SearchPolizasAsync(searchTerm);
        }

        #endregion

        #region Broker Operations

        public async Task<BrokerDto?> GetBrokerAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetBrokerAsync(id),
                    () => _localBrokerService.GetBrokerByIdAsync(id),
                    "Broker.GET",
                    id);
            }
            return await _localBrokerService.GetBrokerByIdAsync(id);
        }

        public async Task<BrokerDto?> GetBrokerByIdAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetBrokerAsync(id),
                    () => _localBrokerService.GetBrokerByIdAsync(id),
                    "Broker.GETBYID", 
                    id);
            }
            return await _localBrokerService.GetBrokerByIdAsync(id);
        }

        public async Task<BrokerDto?> GetBrokerByEmailAsync(string email)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetBrokerByEmailAsync(email),
                    () => _localBrokerService.GetBrokerByEmailAsync(email),
                    "Broker.GETBYEMAIL",
                    email);
            }

            return await _localBrokerService.GetBrokerByEmailAsync(email);
        }

        public async Task<BrokerDto?> GetBrokerByCodigoAsync(string codigo)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetBrokerByCodigoAsync(codigo),
                    () => _localBrokerService.GetBrokerByCodigoAsync(codigo),
                    "Broker.GETBYCODIGO",
                    codigo);
            }

            return await _localBrokerService.GetBrokerByCodigoAsync(codigo);
        }
        public async Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync()
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetBrokersForLookupAsync(),
                    () => _localBrokerService.GetBrokersForLookupAsync(),
                    "Broker.GETLOOKUP",
                    "lookup_brokers");
            }

            return await _localBrokerService.GetBrokersForLookupAsync();
        }

        public async Task<IEnumerable<BrokerDto>> GetAllBrokersAsync()
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GETALL"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetAllBrokersAsync(),
                    () => _localBrokerService.GetAllBrokersAsync(),
                    "Broker.GETALL",
                    "all_brokers");
            }

            return await _localBrokerService.GetAllBrokersAsync();
        }

        public async Task<IEnumerable<BrokerDto>> GetActiveBrokersAsync()
        {
            _logger.LogWarning("🔥 GetActiveBrokersAsync CALLED - Starting hybrid routing");

            try
            {
                bool shouldRoute = await ShouldRouteToVelneoAsync("Broker", "GET");
                _logger.LogWarning("🎯 ShouldRouteToVelneo result: {Result}", shouldRoute);

                if (shouldRoute)
                {
                    _logger.LogWarning("📡 ROUTING TO VELNEO API");
                    return await ExecuteWithFallback(
                        () => _velneoApiService.GetActiveBrokersAsync(),
                        () => _localBrokerService.GetActiveBrokersAsync(),
                        "Broker.GETACTIVE",
                        "active_brokers");
                }

                _logger.LogWarning("🏠 ROUTING TO LOCAL SERVICE");
                return await _localBrokerService.GetActiveBrokersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR in GetActiveBrokersAsync");
                throw;
            }
        }

        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "CREATE"))
            {
                var result = await ExecuteWithAudit(
                    () => _velneoApiService.CreateBrokerAsync(brokerDto),
                    AuditEventType.BrokerCreated,
                    "Broker.CREATE",
                    newData: brokerDto);
                return result;
            }
            else
            {
                return await _localBrokerService.CreateBrokerAsync(brokerDto);
            }
        }

        public async Task UpdateBrokerAsync(BrokerDto brokerDto)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "UPDATE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.UpdateBrokerAsync(brokerDto),
                    AuditEventType.BrokerUpdated,
                    "Broker.UPDATE",
                    newData: brokerDto);
            }
            else
            {
                await _localBrokerService.UpdateBrokerAsync(brokerDto);
            }
        }

        public async Task DeleteBrokerAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "DELETE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.DeleteBrokerAsync(id),
                    AuditEventType.BrokerDeleted,
                    "Broker.DELETE",
                    additionalData: new { BrokerId = id });
            }
            else
            {
                await _localBrokerService.DeleteBrokerAsync(id);
            }
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "SEARCH"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.SearchBrokersAsync(searchTerm),
                    () => _localBrokerService.SearchBrokersAsync(searchTerm),
                    "Broker.SEARCH",
                    searchTerm);
            }

            return await _localBrokerService.SearchBrokersAsync(searchTerm);
        }

        public async Task<bool> ExistsBrokerByCodigoAsync(string codigo, int? excludeId = null)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    async () =>
                    {
                        var broker = await _velneoApiService.GetBrokerByCodigoAsync(codigo);
                        return broker != null && (excludeId == null || broker.Id != excludeId);
                    },
                    () => _localBrokerService.ExistsByCodigoAsync(codigo, excludeId),
                    "Broker.EXISTSBYCODIGO",
                    codigo);
            }

            return await _localBrokerService.ExistsByCodigoAsync(codigo, excludeId);
        }

        public async Task<bool> ExistsBrokerByEmailAsync(string email, int? excludeId = null)
        {
            if (await ShouldRouteToVelneoAsync("Broker", "GET"))
            {
                return await ExecuteWithFallback(
                    async () =>
                    {
                        var broker = await _velneoApiService.GetBrokerByEmailAsync(email);
                        return broker != null && (excludeId == null || broker.Id != excludeId);
                    },
                    () => _localBrokerService.ExistsByEmailAsync(email, excludeId),
                    "Broker.EXISTSBYEMAIL",
                    email);
            }

            return await _localBrokerService.ExistsByEmailAsync(email, excludeId);
        }
        #endregion

        #region Company Operations
        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCompanyAsync(id),
                    () => _localCompanyService.GetCompanyByIdAsync(id),
                    "Company.GETBYID",
                    id);
            }

            return await _localCompanyService.GetCompanyByIdAsync(id);
        }

        public async Task<CompanyDto?> GetCompanyAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCompanyAsync(id),
                    () => _localCompanyService.GetCompanyByIdAsync(id),
                    "Company.GET",
                    id);
            }

            return await _localCompanyService.GetCompanyByIdAsync(id);
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                if (await ShouldRouteToVelneoAsync("Company", "CREATE"))
                {
                    _logger.LogInformation("Routing Company creation to Velneo for tenant {TenantId}",
                        _tenantService.GetCurrentTenantId());

                    var velneoCompany = await ExecuteWithAudit(
                        () => _velneoApiService.CreateCompanyAsync(companyDto),
                        AuditEventType.CompanyCreated,
                        "Company.CREATE",
                        newData: companyDto);

                    return velneoCompany;
                }

                _logger.LogInformation("Routing Company creation to Local for tenant {TenantId}",
                    _tenantService.GetCurrentTenantId());

                var localCompany = await _localCompanyService.CreateCompanyAsync(companyDto);

                await _auditService.LogWithUserAsync(AuditEventType.CompanyCreated, "Company.CREATE",
                    null, localCompany, _tenantService.GetCurrentUserId());

                return localCompany;
                return localCompany;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating company {CompanyName}", companyDto.Comnom);
                throw;
            }
        }

        public async Task<CompanyDto?> GetCompanyByCodigoAsync(string codigo)
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCompanyByCodeAsync(codigo),
                    () => _localCompanyService.GetCompanyByCodigoAsync(codigo),
                    "Company.GETBYCODIGO",
                    codigo);
            }

            return await _localCompanyService.GetCompanyByCodigoAsync(codigo);
        }

        public async Task UpdateCompanyAsync(CompanyDto companyDto)
        {
            if (await ShouldRouteToVelneoAsync("Company", "UPDATE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.UpdateCompanyAsync(companyDto),
                    AuditEventType.CompanyUpdated,
                    "Company.UPDATE",
                    newData: companyDto);
            }
            else
            {
                await _localCompanyService.UpdateCompanyAsync(companyDto);
            }
        }

        public async Task DeleteCompanyAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Company", "DELETE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.DeleteCompanyAsync(id),
                    AuditEventType.CompanyDeleted,
                    "Company.DELETE",
                    additionalData: new { CompanyId = id });
            }
            else
            {
                await _localCompanyService.DeleteCompanyAsync(id);
            }
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            if (await ShouldRouteToVelneoAsync("Company", "GETALL"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetAllCompaniesAsync(),
                    () => _localCompanyService.GetAllCompaniesAsync(),
                    "Company.GETALL",
                    "all_companies");
            }

            return await _localCompanyService.GetAllCompaniesAsync();
        }

        public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
        {
            _logger.LogWarning("GetActiveCompaniesAsync CALLED - Starting hybrid routing");

            try
            {
                bool shouldRoute = await ShouldRouteToVelneoAsync("Company", "GET");
                _logger.LogWarning("ShouldRouteToVelneo result: {Result}", shouldRoute);

                if (shouldRoute)
                {
                    _logger.LogWarning("ROUTING TO VELNEO API");
                    return await ExecuteWithFallback(
                        () => _velneoApiService.GetActiveCompaniesAsync(),
                        () => _localCompanyService.GetActiveCompaniesAsync(),
                        "Company.GETACTIVE",
                        "active_companies");
                }

                _logger.LogWarning("ROUTING TO LOCAL SERVICE");
                return await _localCompanyService.GetActiveCompaniesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR in GetActiveCompaniesAsync");
                throw;
            }
        }

        public async Task<IEnumerable<CompanyLookupDto>> GetCompaniesForLookupAsync()
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCompaniesForLookupAsync(),
                    () => _localCompanyService.GetCompaniesForLookupAsync(),
                    "Company.LOOKUP",
                    "companies_lookup");
            }

            return await _localCompanyService.GetCompaniesForLookupAsync();
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm)
        {
            if (await ShouldRouteToVelneoAsync("Company", "SEARCH"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.SearchCompaniesAsync(searchTerm),
                    () => _localCompanyService.SearchCompaniesAsync(searchTerm),
                    "Company.SEARCH",
                    searchTerm);
            }

            return await _localCompanyService.SearchCompaniesAsync(searchTerm);
        }

        public async Task<CompanyDto?> GetCompanyByCodeAsync(string code)
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCompanyByCodeAsync(code),
                    () => _localCompanyService.GetCompanyByCodigoAsync(code), 
                    "Company.GETBYCODE",
                    code);
            }

            return await _localCompanyService.GetCompanyByCodigoAsync(code); 
        }

        public async Task<CompanyDto?> GetCompanyByAliasAsync(string alias)
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCompanyByAliasAsync(alias),
                    () => _localCompanyService.GetCompanyByAliasAsync(alias),
                    "Company.GETBYALIAS",
                    alias);
            }

            return await _localCompanyService.GetCompanyByAliasAsync(alias);
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            if (await ShouldRouteToVelneoAsync("Company", "GET"))
            {
                try
                {
                    var company = await _velneoApiService.GetCompanyByCodeAsync(codigo);
                    if (company == null)
                        return false;

                    return excludeId == null || company.Id != excludeId;
                }
                catch
                {
                    return await _localCompanyService.ExistsByCodigoAsync(codigo, excludeId);
                }
            }

            return await _localCompanyService.ExistsByCodigoAsync(codigo, excludeId);
        }

        #endregion

        #region Currency Operations

        public async Task<CurrencyDto?> GetCurrencyAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Currency", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCurrencyAsync(id),
                    () => _localCurrencyService.GetCurrencyByIdAsync(id),
                    "Currency.GET",
                    id);
            }

            return await _localCurrencyService.GetCurrencyByIdAsync(id);
        }

        public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto)
        {
            if (await ShouldRouteToVelneoAsync("Currency", "CREATE"))
            {
                return await ExecuteWithAudit(
                    () => _velneoApiService.CreateCurrencyAsync(currencyDto),
                    AuditEventType.CurrencyCreated,
                    "Currency.CREATE",
                    newData: currencyDto);
            }

            return await _localCurrencyService.CreateCurrencyAsync(currencyDto);
        }

        public async Task UpdateCurrencyAsync(CurrencyDto currencyDto)
        {
            if (await ShouldRouteToVelneoAsync("Currency", "UPDATE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.UpdateCurrencyAsync(currencyDto),
                    AuditEventType.CurrencyUpdated,
                    "Currency.UPDATE",
                    newData: currencyDto);
            }
            else
            {
                await _localCurrencyService.UpdateCurrencyAsync(currencyDto);
            }
        }

        public async Task DeleteCurrencyAsync(int id)
        {
            if (await ShouldRouteToVelneoAsync("Currency", "DELETE"))
            {
                await ExecuteWithAudit(
                    () => _velneoApiService.DeleteCurrencyAsync(id),
                    AuditEventType.CurrencyDeleted,
                    "Currency.DELETE",
                    additionalData: new { CurrencyId = id });
            }
            else
            {
                await _localCurrencyService.DeleteCurrencyAsync(id);
            }
        }

        public async Task<CurrencyDto?> GetCurrencyByCodigoAsync(string codigo)
        {
            if (await ShouldRouteToVelneoAsync("Currency", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCurrencyByCodeAsync(codigo),
                    () => _localCurrencyService.GetCurrencyByCodigoAsync(codigo),
                    "Currency.GET_BY_CODE",
                    codigo);
            }

            return await _localCurrencyService.GetCurrencyByCodigoAsync(codigo);
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
        {
            if (await ShouldRouteToVelneoAsync("Currency", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetAllCurrenciesAsync(),
                    () => _localCurrencyService.GetAllCurrenciesAsync(),
                    "Currency.GETALL",
                    "all_currencies");
            }

            return await _localCurrencyService.GetAllCurrenciesAsync();
        }

        public async Task<IEnumerable<CurrencyLookupDto>> GetCurrenciesForLookupAsync()
        {
            if (await ShouldRouteToVelneoAsync("Currency", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetCurrenciesForLookupAsync(),
                    () => _localCurrencyService.GetCurrenciesForLookupAsync(),
                    "Currency.LOOKUP",
                    "currencies_lookup");
            }

            return await _localCurrencyService.GetCurrenciesForLookupAsync();
        }

        public async Task<CurrencyDto?> GetDefaultCurrencyAsync()
        {
            if (await ShouldRouteToVelneoAsync("Currency", "GET"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.GetDefaultCurrencyAsync(),
                    () => _localCurrencyService.GetDefaultCurrencyAsync(),
                    "Currency.DEFAULT",
                    "default_currency");
            }

            return await _localCurrencyService.GetDefaultCurrencyAsync();
        }

        public async Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm)
        {
            if (await ShouldRouteToVelneoAsync("Currency", "SEARCH"))
            {
                return await ExecuteWithFallback(
                    () => _velneoApiService.SearchCurrenciesAsync(searchTerm),
                    () => _localCurrencyService.SearchCurrenciesAsync(searchTerm),
                    "Currency.SEARCH",
                    searchTerm);
            }

            return await _localCurrencyService.SearchCurrenciesAsync(searchTerm);
        }

        #endregion

        #region Helper Methods

        private async Task<T> ExecuteWithFallback<T>(
            Func<Task<T>> primaryAction,
            Func<Task<T>> fallbackAction,
            string operation,
            object? identifier = null)
        {
            var startTime = DateTime.UtcNow;
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Executing {Operation} for tenant {TenantId} with identifier {Identifier}",
                    operation, tenantId, identifier);

                var result = await primaryAction();

                var duration = DateTime.UtcNow - startTime;
                _logger.LogDebug("Successfully executed {Operation} for tenant {TenantId} in {Duration}ms",
                    operation, tenantId, duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                var tenantId = _tenantService.GetCurrentTenantId();

                _logger.LogWarning(ex, "Primary action failed for {Operation} (tenant {TenantId}) after {Duration}ms, attempting fallback",
                    operation, tenantId, duration.TotalMilliseconds);

                try
                {
                    var fallbackResult = await fallbackAction();

                    // Usar LogAsync (3 parámetros) para el fallback
                    await _auditService.LogAsync(
                        AuditEventType.SystemWarning,
                        $"Fallback executed for {operation} (tenant: {tenantId})",
                        new
                        {
                            tenantId,
                            identifier,
                            error = ex.Message,
                            durationMs = duration.TotalMilliseconds,
                            operation,
                            timestamp = DateTime.UtcNow
                        });

                    _logger.LogInformation("Fallback successful for {Operation} (tenant {TenantId})", operation, tenantId);
                    return fallbackResult;
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Fallback also failed for {Operation} (tenant {TenantId})", operation, tenantId);

                    // Log the complete failure
                    await _auditService.LogErrorAsync(
                        fallbackEx,
                        $"Complete failure for {operation} (tenant: {tenantId})",
                        new
                        {
                            tenantId,
                            identifier,
                            primaryError = ex.Message,
                            fallbackError = fallbackEx.Message,
                            durationMs = duration.TotalMilliseconds
                        });

                    throw new ApplicationException($"Both primary and fallback actions failed for {operation} (tenant: {tenantId})", ex);
                }
            }
        }


        private async Task ExecuteWithAudit(
            Func<Task> action,
            AuditEventType eventType,
            string operation,
            object? oldData = null,
            object? newData = null,
            object? additionalData = null)
        {
            var startTime = DateTime.UtcNow;
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var userId = _tenantService.GetCurrentUserId();

                _logger.LogDebug("Executing audited {Operation} for tenant {TenantId}", operation, tenantId);

                await action();

                var duration = DateTime.UtcNow - startTime;

                await _auditService.LogWithUserAsync(
                    eventType,
                    $"{operation} executed successfully for tenant {tenantId}",
                    oldData,
                    newData,
                    userId);
            }
            catch (Exception ex)
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var duration = DateTime.UtcNow - startTime;

                await _auditService.LogErrorAsync(
                    ex,
                    $"{operation} failed for tenant {tenantId}",
                    new
                    {
                        tenantId,
                        oldData,
                        newData,
                        additionalData,
                        durationMs = duration.TotalMilliseconds
                    });
                throw;
            }
        }

        private async Task<T> ExecuteWithAudit<T>(
            Func<Task<T>> action,
            AuditEventType eventType,
            string operation,
            object? oldData = null,
            object? newData = null,
            object? additionalData = null)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                _logger.LogDebug("Executing audited {Operation} for tenant {TenantId}", operation, tenantId);

                var result = await action();

                var duration = DateTime.UtcNow - startTime;

                await _auditService.LogAsync(
                    eventType,
                    $"{operation} executed successfully for tenant {tenantId}",
                    new
                    {
                        tenantId,
                        oldData,
                        newData,
                        additionalData,
                        durationMs = duration.TotalMilliseconds
                    });

                return result;
            }
            catch (Exception ex)
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var duration = DateTime.UtcNow - startTime;

                await _auditService.LogErrorAsync(
                    ex,
                    $"{operation} failed for tenant {tenantId}",
                    new
                    {
                        tenantId,
                        oldData,
                        newData,
                        additionalData,
                        durationMs = duration.TotalMilliseconds
                    });

                throw;
            }
        }

        #endregion

        #region System Operations

        public async Task<Dictionary<string, object>> GetSystemHealthAsync()
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            var tenantConfig = await _tenantService.GetCurrentTenantConfigurationAsync();

            var health = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["tenantId"] = tenantId,
                ["tenantMode"] = tenantConfig.Mode,
                ["configuration"] = new
                {
                    mode = tenantConfig.Mode,
                    baseUrl = tenantConfig.BaseUrl,
                    environment = tenantConfig.Environment,
                    active = tenantConfig.Activo
                }
            };

            // Test Velneo connectivity solo si el tenant está en modo VELNEO
            if (tenantConfig.Mode.Equals("VELNEO", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var velneoHealthy = await TestVelneoConnectivityAsync();
                    health["velneo"] = new
                    {
                        status = velneoHealthy ? "healthy" : "unhealthy",
                        timestamp = DateTime.UtcNow,
                        url = tenantConfig.BaseUrl
                    };
                }
                catch (Exception ex)
                {
                    health["velneo"] = new
                    {
                        status = "error",
                        error = ex.Message,
                        timestamp = DateTime.UtcNow,
                        url = tenantConfig.BaseUrl
                    };
                }
            }
            else
            {
                health["velneo"] = new
                {
                    status = "disabled",
                    reason = "Tenant configured for LOCAL mode",
                    timestamp = DateTime.UtcNow
                };
            }

            // Test local services
            try
            {
                var localClientCount = (await _localClientService.GetAllClientsAsync()).Count();
                health["local"] = new
                {
                    status = "healthy",
                    clientCount = localClientCount,
                    timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                health["local"] = new
                {
                    status = "error",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                };
            }

            return health;
        }

        public async Task<bool> TestVelneoConnectivityAsync()
        {
            try
            {
                // Intentar obtener un cliente que no existe para probar conectividad
                await _velneoApiService.GetClientAsync(999999);
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch
            {
                // Si obtenemos otro error, significa que al menos pudimos conectar
                return true;
            }
        }
        #endregion
    }
}