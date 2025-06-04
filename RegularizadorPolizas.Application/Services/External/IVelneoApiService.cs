using RegularizadorPolizas.Application.DTOs;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVelneoApiService
    {
        #region Métodos de lectura existentes
        Task<ClientDto> GetClientAsync(int id);
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<UserDto> GetUsersAsync(int id);
        #endregion

        #region Métodos de sincronización para Clientes
        Task<ClientDto> CreateClientAsync(ClientDto clientDto);
        Task<ClientDto> UpdateClientAsync(ClientDto clientDto);
        Task<bool> DeleteClientAsync(int clientId);
        Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);
        #endregion

        #region Métodos de sincronización para Pólizas
        Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto);
        Task<PolizaDto> UpdatePolizaAsync(PolizaDto polizaDto);
        Task<bool> DeletePolizaAsync(int polizaId);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);
        #endregion

        #region Métodos de sincronización para Renovaciones (para el futuro)
        Task<RenovationDto> CreateRenovationAsync(RenovationDto renovationDto);
        Task<PolizaDto> ProcessRenovationAsync(int renovationId);
        Task<bool> CancelRenovationAsync(int renovationId, string reason);
        #endregion

        #region Métodos de sincronización para Brokers
        Task<BrokerDto> GetBrokerByIdAsync(int id);
        Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto);
        Task<BrokerDto> UpdateBrokerAsync(BrokerDto brokerDto);
        Task<bool> DeleteBrokerAsync(int brokerId);
        Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm);
        Task<IEnumerable<BrokerDto>> GetAllBrokersAsync();
        #endregion

        #region Métodos de sincronización para Compañías
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task<CompanyDto> UpdateCompanyAsync(CompanyDto companyDto);
        Task<bool> DeleteCompanyAsync(int companyId);
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(string searchTerm);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        #endregion

        #region Métodos de sincronización para Monedas
        Task<CurrencyDto> GetCurrencyByIdAsync(int id);
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task<CurrencyDto> UpdateCurrencyAsync(CurrencyDto currencyDto);
        Task<bool> DeleteCurrencyAsync(int currencyId);
        Task<IEnumerable<CurrencyDto>> SearchCurrenciesAsync(string searchTerm);
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        #endregion

        #region Método de verificación de conectividad
        Task<bool> TestConnectionAsync();
        #endregion
    }
}