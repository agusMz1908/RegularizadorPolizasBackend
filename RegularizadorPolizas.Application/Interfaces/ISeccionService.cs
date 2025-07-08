using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface ISeccionService
    {
        Task<IEnumerable<SeccionDto>> GetAllSeccionesAsync();
        Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync();
        Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync();
        Task<SeccionDto?> GetSeccionByIdAsync(int id);
        Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm);
        Task<SeccionDto> CreateSeccionAsync(CreateSeccionDto createDto);
        Task<SeccionDto> UpdateSeccionAsync(int id, UpdateSeccionDto updateDto);
        Task DeleteSeccionAsync(int id);
        Task<bool> SeccionExistsAsync(string name, int? excludeId = null);
    }
}