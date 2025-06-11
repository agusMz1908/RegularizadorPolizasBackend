using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<PermissionDto> GetPermissionByIdAsync(int id);
        Task<PermissionDto> GetPermissionByNameAsync(string name);
        Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync();
        Task<IEnumerable<PermissionLookupDto>> GetPermissionsForLookupAsync();
        Task<PermissionDto> CreatePermissionAsync(PermissionDto permissionDto);
        Task UpdatePermissionAsync(PermissionDto permissionDto);
        Task DeletePermissionAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        // Resource-based operations
        Task<IEnumerable<PermissionDto>> GetByResourceAsync(string resource);
        Task<IEnumerable<string>> GetAvailableResourcesAsync();
        Task<IEnumerable<string>> GetAvailableActionsAsync();

        // Permission usage
        Task<IEnumerable<RoleDto>> GetPermissionRolesAsync(int permissionId);
        Task<int> GetPermissionUsageCountAsync(int permissionId);
    }
}