using RegularizadorPolizas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientesAsync();
        Task<ClientDto> GetClienteByIdAsync(int id);
        Task<ClientDto> CreateClienteAsync(ClientDto clienteDto);
        Task UpdateClienteAsync(ClientDto clienteDto);
        Task DeleteClienteAsync(int id);
    }
}