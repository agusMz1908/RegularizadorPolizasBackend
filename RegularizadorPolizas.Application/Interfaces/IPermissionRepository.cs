using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<Permission?> GetByNameAsync(string name);
        Task<IEnumerable<Permission>> GetActivePermissionsAsync();
        Task<IEnumerable<Permission>> GetByResourceAsync(string resource);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}