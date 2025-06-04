using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Configuration;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.Services
{
    public class BusinessSpecificHybridService : IHybridApiService
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly IProcessDocumentService _localDocumentService;
        private readonly IAuditService _auditService;
        private readonly HybridApiConfiguration _config;
        private readonly ILogger<BusinessSpecificHybridService> _logger;

        public BusinessSpecificHybridService(
            IVelneoApiService velneoApiService,
            IProcessDocumentService localDocumentService,
            IAuditService auditService,
            IOptions<HybridApiConfiguration> config,
            ILogger<BusinessSpecificHybridService> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _localDocumentService = localDocumentService ?? throw new ArgumentNullException(nameof(localDocumentService));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Operaciones Velneo
        public async Task<ClientDto> GetClientAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Client", "GET", id,
                () => _velneoApiService.GetClientAsync(id));
        }
        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto)
        {
            return await ExecuteVelneoWithAudit("Client", "CREATE", null,
                () => _velneoApiService.CreateClientAsync(clientDto), clientDto);
        }
        public async Task<ClientDto> UpdateClientAsync(ClientDto clientDto)
        {
            return await ExecuteVelneoWithAudit("Client", "UPDATE", clientDto.Id,
                () => _velneoApiService.UpdateClientAsync(clientDto), clientDto);
        }
        public async Task<bool> DeleteClientAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Client", "DELETE", id,
                () => _velneoApiService.DeleteClientAsync(id));
        }
        public async Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm)
        {
            return await ExecuteVelneoWithAudit("Client", "SEARCH", null,
                () => _velneoApiService.SearchClientsAsync(searchTerm),
                new { searchTerm });
        }

        public async Task<BrokerDto> GetBrokerAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Broker", "GET", id,
                () => _velneoApiService.GetBrokerByIdAsync(id));
        }
        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            return await ExecuteVelneoWithAudit("Broker", "CREATE", null,
                () => _velneoApiService.CreateBrokerAsync(brokerDto), brokerDto);
        }
        public async Task<BrokerDto> UpdateBrokerAsync(BrokerDto brokerDto)
        {
            return await ExecuteVelneoWithAudit("Broker", "UPDATE", brokerDto.Id,
                () => _velneoApiService.UpdateBrokerAsync(brokerDto), brokerDto);
        }
        public async Task<bool> DeleteBrokerAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Broker", "DELETE", id,
                () => _velneoApiService.DeleteBrokerAsync(id));
        }

        public async Task<CurrencyDto> GetCurrencyAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Currency", "GET", id,
                () => _velneoApiService.GetCurrencyByIdAsync(id));
        }
        public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto)
        {
            return await ExecuteVelneoWithAudit("Currency", "CREATE", null,
                () => _velneoApiService.CreateCurrencyAsync(currencyDto), currencyDto);
        }
        public async Task<CurrencyDto> UpdateCurrencyAsync(CurrencyDto currencyDto)
        {
            return await ExecuteVelneoWithAudit("Currency", "UPDATE", currencyDto.Id,
                () => _velneoApiService.UpdateCurrencyAsync(currencyDto), currencyDto);
        }
        public async Task<bool> DeleteCurrencyAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Currency", "DELETE", id,
                () => _velneoApiService.DeleteCurrencyAsync(id));
        }

        public async Task<CompanyDto> GetCompanyAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Company", "GET", id,
                () => _velneoApiService.GetCompanyByIdAsync(id));
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            return await ExecuteVelneoWithAudit("Company", "CREATE", null,
                () => _velneoApiService.CreateCompanyAsync(companyDto), companyDto);
        }

        public async Task<CompanyDto> UpdateCompanyAsync(CompanyDto companyDto)
        {
            return await ExecuteVelneoWithAudit("Company", "UPDATE", companyDto.Id,
                () => _velneoApiService.UpdateCompanyAsync(companyDto), companyDto);
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Company", "DELETE", id,
                () => _velneoApiService.DeleteCompanyAsync(id));
        }

        #endregion

        #region Operaciones API

        public async Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file)
        {
            return await ExecuteLocalWithAudit("Document", "PROCESS", null,
                () => _localDocumentService.ProcessDocumentAsync(file),
                new { fileName = file.FileName, fileSize = file.Length });
        }

        public async Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentId)
        {
            return await ExecuteLocalWithAudit("Document", "EXTRACT", documentId,
                () => _localDocumentService.ExtractPolizaFromDocumentAsync(documentId));
        }

        public async Task<ProcessDocumentDto> LinkDocumentToPolizaAsync(int documentId, int polizaId)
        {
            return await ExecuteLocalWithAudit("Document", "LINK", documentId,
                () => _localDocumentService.LinkDocumentToPolizaAsync(documentId, polizaId),
                new { polizaId });
        }

        public async Task<PolizaCreationResult> CreatePolizaFromDocumentAsync(int documentId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting poliza creation from document {DocumentId}", documentId);

                //Extraer datos del documento
                var extractedPoliza = await _localDocumentService.ExtractPolizaFromDocumentAsync(documentId);
                if (extractedPoliza == null)
                {
                    throw new ApplicationException($"Could not extract poliza data from document {documentId}");
                }

                _logger.LogInformation("Successfully extracted poliza data from document {DocumentId}", documentId);

                //Crear póliza en Velneo
                var createdPoliza = await _velneoApiService.CreatePolizaAsync(extractedPoliza);
                if (createdPoliza == null)
                {
                    throw new ApplicationException("Failed to create poliza in Velneo");
                }

                _logger.LogInformation("Successfully created poliza {PolizaId} in Velneo from document {DocumentId}",
                    createdPoliza.Id, documentId);

                //Vincular documento con póliza
                var linkedDocument = await _localDocumentService.LinkDocumentToPolizaAsync(documentId, createdPoliza.Id);

                stopwatch.Stop();

                //Auditar todo el flujo
                await _auditService.LogAsync(
                    AuditEventType.PolicyCreated,
                    $"Poliza created from document processing - Document {documentId} → Poliza {createdPoliza.Id}",
                    new
                    {
                        DocumentId = documentId,
                        PolizaId = createdPoliza.Id,
                        ProcessingTimeMs = stopwatch.ElapsedMilliseconds,
                        ExtractedData = extractedPoliza,
                        VelneoResponse = createdPoliza
                    });

                return new PolizaCreationResult
                {
                    Success = true,
                    PolizaDto = createdPoliza,
                    DocumentDto = linkedDocument,
                    ProcessingTimeMs = stopwatch.ElapsedMilliseconds,
                    Message = "Poliza created successfully from document"
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error creating poliza from document {DocumentId}", documentId);

                await _auditService.LogErrorAsync(ex,
                    $"Failed to create poliza from document {documentId}",
                    new { DocumentId = documentId, ProcessingTimeMs = stopwatch.ElapsedMilliseconds });

                return new PolizaCreationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ProcessingTimeMs = stopwatch.ElapsedMilliseconds,
                    Message = "Failed to create poliza from document"
                };
            }
        }

        #endregion

        #region Operaciones de Pólizas 

        public async Task<PolizaDto> GetPolizaAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Poliza", "GET", id,
                () => _velneoApiService.GetPolizaAsync(id));
        }

        public async Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto)
        {
            return await ExecuteVelneoWithAudit("Poliza", "CREATE", null,
                () => _velneoApiService.CreatePolizaAsync(polizaDto), polizaDto);
        }

        public async Task<PolizaDto> UpdatePolizaAsync(PolizaDto polizaDto)
        {
            return await ExecuteVelneoWithAudit("Poliza", "UPDATE", polizaDto.Id,
                () => _velneoApiService.UpdatePolizaAsync(polizaDto), polizaDto);
        }

        public async Task<bool> DeletePolizaAsync(int id)
        {
            return await ExecuteVelneoWithAudit("Poliza", "DELETE", id,
                () => _velneoApiService.DeletePolizaAsync(id));
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            return await ExecuteVelneoWithAudit("Poliza", "SEARCH", null,
                () => _velneoApiService.SearchPolizasAsync(searchTerm),
                new { searchTerm });
        }

        #endregion

        #region Métodos de utilidad

        public async Task<ApiTarget> GetRoutingFor(string entity, string operation)
        {
            await Task.CompletedTask;
            var key = $"{entity}.{operation}";
            return _config.EntityRouting.TryGetValue(key, out var target) ? target : GetDefaultTarget(entity);
        }

        public async Task<bool> TestConnectivityAsync()
        {
            try
            {
                return await _velneoApiService.TestConnectionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Velneo connectivity");
                return false;
            }
        }

        #endregion

        #region Métodos de renovaciones

        public Task<RenovationDto> CreateRenovationAsync(RenovationDto renovationDto)
        {
            throw new NotImplementedException("Renovations will be implemented in Phase 2 with specific business logic");
        }

        public Task<PolizaDto> ProcessRenovationAsync(int renovationId)
        {
            throw new NotImplementedException("Renovations will be implemented in Phase 2 with specific business logic");
        }

        public Task<bool> CancelRenovationAsync(int renovationId, string reason)
        {
            throw new NotImplementedException("Renovations will be implemented in Phase 2 with specific business logic");
        }

        #endregion

        #region Métodos privados de ejecución

        private async Task<T> ExecuteVelneoWithAudit<T>(string entityType, string operation, int? entityId,
            Func<Task<T>> action, object? additionalData = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Executing {Operation} on {EntityType} {EntityId} via Velneo API",
                    operation, entityType, entityId);

                var result = await action();
                stopwatch.Stop();

                if (_config.EnableLocalAudit)
                {
                    await _auditService.LogAsync(
                        GetAuditEventType(entityType, operation),
                        $"{operation} {entityType} executed successfully via Velneo API",
                        new { EntityId = entityId, DurationMs = stopwatch.ElapsedMilliseconds, AdditionalData = additionalData, Target = "Velneo" });
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error executing {Operation} on {EntityType} {EntityId} via Velneo API",
                    operation, entityType, entityId);

                if (_config.EnableLocalAudit)
                {
                    await _auditService.LogErrorAsync(ex,
                        $"Error executing {operation} on {entityType} via Velneo API",
                        new { EntityId = entityId, DurationMs = stopwatch.ElapsedMilliseconds, AdditionalData = additionalData, Target = "Velneo" });
                }

                throw;
            }
        }

        private async Task<T> ExecuteLocalWithAudit<T>(string entityType, string operation, int? entityId,
            Func<Task<T>> action, object? additionalData = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Executing {Operation} on {EntityType} {EntityId} via Local API",
                    operation, entityType, entityId);

                var result = await action();
                stopwatch.Stop();

                if (_config.EnableLocalAudit)
                {
                    await _auditService.LogAsync(
                        GetAuditEventType(entityType, operation),
                        $"{operation} {entityType} executed successfully via Local API",
                        new { EntityId = entityId, DurationMs = stopwatch.ElapsedMilliseconds, AdditionalData = additionalData, Target = "Local" });
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error executing {Operation} on {EntityType} {EntityId} via Local API",
                    operation, entityType, entityId);

                if (_config.EnableLocalAudit)
                {
                    await _auditService.LogErrorAsync(ex,
                        $"Error executing {operation} on {entityType} via Local API",
                        new { EntityId = entityId, DurationMs = stopwatch.ElapsedMilliseconds, AdditionalData = additionalData, Target = "Local" });
                }

                throw;
            }
        }

        private ApiTarget GetDefaultTarget(string entity)
        {
            return entity switch
            {
                "Document" => ApiTarget.Local,
                _ => ApiTarget.Velneo
            };
        }

        private AuditEventType GetAuditEventType(string entityType, string operation)
        {
            return (entityType.ToLower(), operation.ToUpper()) switch
            {
                ("client", "CREATE") => AuditEventType.ClientCreated,
                ("client", "UPDATE") => AuditEventType.ClientUpdated,
                ("client", "DELETE") => AuditEventType.ClientDeleted,
                ("broker", "CREATE") => AuditEventType.BrokerCreated,
                ("broker", "UPDATE") => AuditEventType.BrokerUpdated,
                ("broker", "DELETE") => AuditEventType.BrokerDeleted,
                ("company", "CREATE") => AuditEventType.CompanyCreated,
                ("company", "UPDATE") => AuditEventType.CompanyUpdated,
                ("company", "DELETE") => AuditEventType.CompanyDeleted,
                ("currency", "CREATE") => AuditEventType.CurrencyCreated,
                ("currency", "UPDATE") => AuditEventType.CurrencyUpdated,
                ("currency", "DELETE") => AuditEventType.CurrencyDeleted,
                ("poliza", "CREATE") => AuditEventType.PolicyCreated,
                ("poliza", "UPDATE") => AuditEventType.PolicyUpdated,
                ("poliza", "DELETE") => AuditEventType.PolicyDeleted,
                ("document", "PROCESS") => AuditEventType.DocumentProcessed,
                ("document", "EXTRACT") => AuditEventType.DocumentDataExtracted,
                ("document", "LINK") => AuditEventType.DocumentLinkedToPolicy,
                _ => AuditEventType.SystemInfo
            };
        }

        #endregion
    }
}