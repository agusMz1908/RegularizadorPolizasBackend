using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task<RoleDto> GetRoleByNameAsync(string name);
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<IEnumerable<RoleLookupDto>> GetRolesForLookupAsync();
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task UpdateRoleAsync(RoleDto roleDto);
        Task DeleteRoleAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        // Role permissions
        Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId);
        Task AssignPermissionToRoleAsync(int roleId, int permissionId, int? grantedBy = null);
        Task RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<bool> RoleHasPermissionAsync(int roleId, string permissionName);

        // Role users
        Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId);
        Task<int> GetRoleUserCountAsync(int roleId);
    }
}