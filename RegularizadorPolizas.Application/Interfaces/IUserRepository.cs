using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<User?> GetUserWithRolesAsync(int id);
        Task<IEnumerable<User>> SearchByNameOrEmailAsync(string searchTerm);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
    }
}