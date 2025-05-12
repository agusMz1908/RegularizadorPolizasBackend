using RegularizadorPolizas.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IPolizaService
    {
        Task<IEnumerable<PolizaDto>> GetAllPolizasAsync();
        Task<PolizaDto> GetPolizaByIdAsync(int id);
        Task<IEnumerable<PolizaDto>> GetPolizasByClienteAsync(int clienteId);
        Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto);
        Task UpdatePolizaAsync(PolizaDto polizaDto);
        Task DeletePolizaAsync(int id);
        Task<PolizaDto> RenovarPolizaAsync(int polizaId, RenovationDto renovationDto);
        Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm);
    }
}