using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Configuration;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IHybridApiService
    {
        #region Clientes - CRUD Directo a Velneo
        Task<ClientDto> GetClientAsync(int id);
        Task<ClientDto> CreateClientAsync(ClientDto clientDto);
        Task<ClientDto> UpdateClientAsync(ClientDto clientDto);
        Task<bool> DeleteClientAsync(int id);
        Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);
        #endregion

        #region Brokers - CRUD Directo a Velneo
        Task<BrokerDto> GetBrokerAsync(int id);
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task<BrokerDto> UpdateBrokerAsync(BrokerDto brokerDto);
        Task<bool> DeleteBrokerAsync(int id);
        #endregion

        #region Monedas - CRUD Directo a Velneo
        Task<CurrencyDto> GetCurrencyAsync(int id);
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task<CurrencyDto> UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task<bool> DeleteCurrencyAsync(int id);
        #endregion

        #region Compañías - CRUD Directo a Velneo
        Task<CompanyDto> GetCompanyAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task<CompanyDto> UpdateCompanyAsync(CompanyDto companyDto);
        Task<bool> DeleteCompanyAsync(int id);
        #endregion

        #region Pólizas - Consultas desde Velneo
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto);
        Task<PolizaDto> UpdatePolizaAsync(PolizaDto polizaDto);
        Task<bool> DeletePolizaAsync(int id);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);
        #endregion

        #region Documentos - Tu Valor Diferencial (Local)
        Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file);
        Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentId);
        Task<ProcessDocumentDto> LinkDocumentToPolizaAsync(int documentId, int polizaId);
        Task<PolizaCreationResult> CreatePolizaFromDocumentAsync(int documentId);
        #endregion

        #region Renovaciones - Para el futuro
        Task<RenovationDto> CreateRenovationAsync(RenovationDto renovationDto);
        Task<PolizaDto> ProcessRenovationAsync(int renovationId);
        Task<bool> CancelRenovationAsync(int renovationId, string reason);
        #endregion

        #region Configuración y utilidades
        Task<ApiTarget> GetRoutingFor(string entity, string operation);
        Task<bool> TestConnectivityAsync();
        #endregion
    }
}