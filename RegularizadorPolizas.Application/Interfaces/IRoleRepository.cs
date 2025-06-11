using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetActiveRolesAsync();
        Task<Role?> GetRoleWithPermissionsAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}