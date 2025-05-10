using RegularizadorPolizas.Application.DTOs;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVelneoApiService
    {
        Task<ClientDto> GetClientAsync(int id);
        Task<PolizaDto> GetPolizaAsync(int id);
        Task<UserDto> GetUsersAsync(int id);
    }
}