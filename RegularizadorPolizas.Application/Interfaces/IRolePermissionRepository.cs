using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(int permissionId);
        Task<RolePermission?> GetByRoleAndPermissionAsync(int roleId, int permissionId);
        Task<IEnumerable<RolePermission>> GetActiveRolePermissionsAsync(int roleId);
    }
}