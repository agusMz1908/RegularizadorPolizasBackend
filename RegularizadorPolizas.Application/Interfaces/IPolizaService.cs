using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IPolizaService
    {
        Task<IEnumerable<PolizaDto>> GetAllPolizas();
        Task<PolizaDto> GetPolizaById(int id);
        Task<PolizaDto> CreatePoliza(PolizaDto polizaDto);
        Task UpdatePoliza(PolizaDto polizaDto);
        Task DeletePoliza(int id);
        Task<IEnumerable<PolizaDto>> GetPolizasByCliente(int clienteId);
        Task<PolizaDto> RenovarPoliza(int polizaId, RenovationDto renovacionDto);
    }
}