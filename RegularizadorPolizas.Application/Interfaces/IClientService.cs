using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<ClientDto> GetClientByIdAsync(int id);
        Task<ClientDto> CreateClientAsync(ClientDto clientDto);
        Task UpdateClientAsync(ClientDto clientDto);
        Task DeleteClientAsync(int id);
        Task<ClientDto> GetClientByEmailAsync(string email);
        Task<ClientDto> GetClientByDocumentAsync(string document);
        Task<IEnumerable<ClientDto>> SearchClientsAsync(string searchTerm);
    }
}