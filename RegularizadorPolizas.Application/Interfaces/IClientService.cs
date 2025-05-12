using RegularizadorPolizas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientesAsync();
        Task<ClientDto> GetClienteByIdAsync(int id);
        Task<ClientDto> CreateClienteAsync(ClientDto clientDto);
        Task UpdateClienteAsync(ClientDto clientDto);
        Task DeleteClienteAsync(int id);
        Task<ClientDto> GetClienteByEmailAsync(string email);
        Task<ClientDto> GetClienteByDocumentoAsync(string documento);
        Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm);
    }
}