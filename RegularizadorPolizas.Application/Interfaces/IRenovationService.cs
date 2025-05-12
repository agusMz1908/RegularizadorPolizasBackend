using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IRenovationService
    {
        Task<IEnumerable<RenovationDto>> GetAllRenovationsAsync();
        Task<RenovationDto> GetRenovationByIdAsync(int id);
        Task<IEnumerable<RenovationDto>> GetRenovationsByStatusAsync(string status);
        Task<IEnumerable<RenovationDto>> GetRenovationsByPolizaIdAsync(int polizaId);
        Task<RenovationDto> CreateRenovationAsync(RenovationDto renovationDto);
        Task UpdateRenovationAsync(RenovationDto renovationDto);
        Task<PolizaDto> ProcessRenovationAsync(int renovationId);
        Task CancelRenovationAsync(int renovationId, string reason);
    }
}