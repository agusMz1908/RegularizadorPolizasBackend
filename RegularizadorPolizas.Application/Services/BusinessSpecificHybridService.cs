using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegularizadorPolizas.Application.Configuration;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Enums;
using System.Text.Json;

namespace RegularizadorPolizas.Application.Services
{ 
    public class BusinessSpecificHybridService : IHybridApiService
    {
    private readonly HybridApiConfiguration _config;
    private readonly IVelneoApiService _velneoApiService;
    private readonly IClientService _localClientService;
    private readonly IBrokerService _localBrokerService;
    private readonly ICurrencyService _localCurrencyService;
    private readonly ICompanyService _localCompanyService;
    private readonly IPolizaService _localPolizaService;
    private readonly IAuditService _auditService;
    private readonly ILogger<BusinessSpecificHybridService> _logger;

    public BusinessSpecificHybridService(
        IOptions<HybridApiConfiguration> config,
        IVelneoApiService velneoApiService,
        IClientService localClientService,
        IBrokerService localBrokerService,
        ICurrencyService localCurrencyService,
        ICompanyService localCompanyService,
        IPolizaService localPolizaService,
        IAuditService auditService,
        ILogger<BusinessSpecificHybridService> logger)
    {
        _config = config.Value;
        _velneoApiService = velneoApiService;
        _localClientService = localClientService;
        _localBrokerService = localBrokerService;
        _localCurrencyService = localCurrencyService;
        _localCompanyService = localCompanyService;
        _localPolizaService = localPolizaService;
        _auditService = auditService;
        _logger = logger;
    }

    #region Client Operations

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        if (_config.ShouldRouteToVelneo("Client", "GET"))
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
        if (_config.ShouldRouteToVelneo("Client", "CREATE"))
        {
            var result = await ExecuteWithAudit(
                async () =>
                {
                    var velneoResult = await _velneoApiService.CreateClientAsync(clientDto);

                    // Sincronizar con base local si está habilitado
                    if (_config.EnableLocalCaching)
                    {
                        try
                        {
                            await _localClientService.CreateClientAsync(velneoResult);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to sync client {ClientId} to local database", velneoResult.Id);
                        }
                    }

                    return velneoResult;
                },
                AuditEventType.ClientCreated,
                "Client.CREATE",
                newData: clientDto);

            return result;
        }

        return await _localClientService.CreateClientAsync(clientDto);
    }

    public async Task UpdateClientAsync(ClientDto clientDto)
    {
        if (_config.ShouldRouteToVelneo("Client", "UPDATE"))
        {
            await ExecuteWithAudit(
                async () =>
                {
                    await _velneoApiService.UpdateClientAsync(clientDto);

                    // Sincronizar con base local si está habilitado
                    if (_config.EnableLocalCaching)
                    {
                        try
                        {
                            await _localClientService.UpdateClientAsync(clientDto);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to sync client {ClientId} update to local database", clientDto.Id);
                        }
                    }

                    return Task.CompletedTask;
                },
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
        if (_config.ShouldRouteToVelneo("Client", "DELETE"))
        {
            await ExecuteWithAudit(
                async () =>
                {
                    await _velneoApiService.DeleteClientAsync(id);

                    // Sincronizar con base local si está habilitado
                    if (_config.EnableLocalCaching)
                    {
                        try
                        {
                            await _localClientService.DeleteClientAsync(id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to sync client {ClientId} deletion to local database", id);
                        }
                    }

                    return Task.CompletedTask;
                },
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
        if (_config.ShouldRouteToVelneo("Client", "SEARCH"))
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
        if (_config.ShouldRouteToVelneo("Client", "GET"))
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

    #region Broker Operations

        public async Task<BrokerDto?> GetBrokerAsync(int id)
    {
        if (_config.ShouldRouteToVelneo("Broker", "GET"))
        {
            return await ExecuteWithFallback(
                () => _velneoApiService.GetBrokerAsync(id),
                () => _localBrokerService.GetBrokerByIdAsync(id),
                "Broker.GET",
                id);
        }

        return await _localBrokerService.GetBrokerByIdAsync(id);
    }

    public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
    {
        if (_config.ShouldRouteToVelneo("Broker", "CREATE"))
        {
            return await ExecuteWithAudit(
                () => _velneoApiService.CreateBrokerAsync(brokerDto),
                AuditEventType.BrokerCreated,
                "Broker.CREATE",
                newData: brokerDto);
        }

        return await _localBrokerService.CreateBrokerAsync(brokerDto);
    }

    public async Task UpdateBrokerAsync(BrokerDto brokerDto)
    {
        if (_config.ShouldRouteToVelneo("Broker", "UPDATE"))
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
        if (_config.ShouldRouteToVelneo("Broker", "DELETE"))
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
        if (_config.ShouldRouteToVelneo("Broker", "SEARCH"))
        {
            return await ExecuteWithFallback(
                () => _velneoApiService.SearchBrokersAsync(searchTerm),
                () => _localBrokerService.SearchBrokersAsync(searchTerm),
                "Broker.SEARCH",
                searchTerm);
        }

        return await _localBrokerService.SearchBrokersAsync(searchTerm);
    }

    #endregion

    #region Currency Operations

    public async Task<CurrencyDto?> GetCurrencyAsync(int id)
    {
        if (_config.ShouldRouteToVelneo("Currency", "GET"))
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
        if (_config.ShouldRouteToVelneo("Currency", "CREATE"))
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
        if (_config.ShouldRouteToVelneo("Currency", "UPDATE"))
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
        if (_config.ShouldRouteToVelneo("Currency", "DELETE"))
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
        if (_config.ShouldRouteToVelneo("Currency", "GET"))
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
        if (_config.ShouldRouteToVelneo("Currency", "GET"))
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
        if (_config.ShouldRouteToVelneo("Currency", "GET"))
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
        if (_config.ShouldRouteToVelneo("Currency", "GET"))
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
        if (_config.ShouldRouteToVelneo("Currency", "SEARCH"))
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

    #region Company Operations

    public async Task<CompanyDto?> GetCompanyAsync(int id)
    {
        if (_config.ShouldRouteToVelneo("Company", "GET"))
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
        if (_config.ShouldRouteToVelneo("Company", "CREATE"))
        {
            return await ExecuteWithAudit(
                () => _velneoApiService.CreateCompanyAsync(companyDto),
                AuditEventType.CompanyCreated,
                "Company.CREATE",
                newData: companyDto);
        }

        return await _localCompanyService.CreateCompanyAsync(companyDto);
    }

    public async Task UpdateCompanyAsync(CompanyDto companyDto)
    {
        if (_config.ShouldRouteToVelneo("Company", "UPDATE"))
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
        if (_config.ShouldRouteToVelneo("Company", "DELETE"))
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

    #endregion

    #region Poliza Operations

    public async Task<PolizaDto?> GetPolizaAsync(int id)
    {
        if (_config.ShouldRouteToVelneo("Poliza", "GET"))
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
        if (_config.ShouldRouteToVelneo("Poliza", "CREATE"))
        {
            var result = await ExecuteWithAudit(
                async () =>
                {
                    var velneoPoliza = await _velneoApiService.CreatePolizaAsync(polizaDto);
                    return new PolizaCreationResult
                    {
                        Success = true,
                        Poliza = velneoPoliza,
                        Source = "Velneo",
                        Message = "Póliza creada exitosamente en Velneo"
                    };
                },
                AuditEventType.PolicyCreated,
                "Poliza.CREATE",
                newData: polizaDto);

            return result;
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
        if (_config.ShouldRouteToVelneo("Poliza", "UPDATE"))
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
        if (_config.ShouldRouteToVelneo("Poliza", "DELETE"))
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
        if (_config.ShouldRouteToVelneo("Poliza", "SEARCH"))
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

    #region System Operations

    public async Task<Dictionary<string, object>> GetSystemHealthAsync()
    {
        var health = new Dictionary<string, object>
        {
            ["timestamp"] = DateTime.UtcNow,
            ["configuration"] = new
            {
                enableLocalAudit = _config.EnableLocalAudit,
                enableVelneoFallback = _config.EnableVelneoFallback,
                enableLocalCaching = _config.EnableLocalCaching
            }
        };

        // Test Velneo connectivity
        try
        {
            var velneoHealthy = await TestVelneoConnectivityAsync();
            health["velneo"] = new { status = velneoHealthy ? "healthy" : "unhealthy", timestamp = DateTime.UtcNow };
        }
        catch (Exception ex)
        {
            health["velneo"] = new { status = "error", error = ex.Message, timestamp = DateTime.UtcNow };
        }

        // Test local services
        try
        {
            var localClientCount = (await _localClientService.GetAllClientsAsync()).Count();
            health["local"] = new { status = "healthy", clientCount = localClientCount, timestamp = DateTime.UtcNow };
        }
        catch (Exception ex)
        {
            health["local"] = new { status = "error", error = ex.Message, timestamp = DateTime.UtcNow };
        }

        return health;
    }

    public async Task<bool> TestVelneoConnectivityAsync()
    {
        try
        {
            // Intentar obtener un cliente que no existe, pero que nos permita probar conectividad
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
            _logger.LogDebug("Executing {Operation} with identifier {Identifier}", operation, identifier);
            var result = await primaryAction();

            var duration = DateTime.UtcNow - startTime;
            _logger.LogDebug("Successfully executed {Operation} in {Duration}ms", operation, duration.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogWarning(ex, "Primary action failed for {Operation} after {Duration}ms, attempting fallback", operation, duration.TotalMilliseconds);

            if (_config.EnableVelneoFallback && fallbackAction != null)
            {
                try
                {
                    var fallbackResult = await fallbackAction();

                    await _auditService.LogAsync(
                        AuditEventType.SystemWarning,
                        $"Fallback executed for {operation}",
                        new { identifier, error = ex.Message, durationMs = duration.TotalMilliseconds });

                    return fallbackResult;
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Fallback also failed for {Operation}", operation);
                    throw new ApplicationException($"Both primary and fallback actions failed for {operation}", ex);
                }
            }

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
            var result = await action();

            var duration = DateTime.UtcNow - startTime;

            if (_config.EnableLocalAudit)
            {
                await _auditService.LogAsync(
                    eventType,
                    $"Successfully executed {operation}",
                    new
                    {
                        operation,
                        durationMs = duration.TotalMilliseconds,
                        oldData,
                        newData,
                        additionalData
                    });
            }

            return result;
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;

            if (_config.EnableLocalAudit)
            {
                await _auditService.LogErrorAsync(
                    ex,
                    $"Failed to execute {operation}",
                    new
                    {
                        operation,
                        durationMs = duration.TotalMilliseconds,
                        oldData,
                        newData,
                        additionalData
                    });
            }

            throw;
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
        await ExecuteWithAudit(
            async () => { await action(); return Task.CompletedTask; },
            eventType,
            operation,
            oldData,
            newData,
            additionalData);
    }

    #endregion
    }
}