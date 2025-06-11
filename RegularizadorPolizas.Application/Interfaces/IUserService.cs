using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IUserService
    {
        // CRUD básico
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);

        // Gestión de roles
        Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);
        Task AssignRoleToUserAsync(int userId, int roleId, int? assignedBy = null);
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        Task<bool> UserHasRoleAsync(int userId, string roleName);
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);

        // Gestión de estado
        Task ActivateUserAsync(int id);
        Task DeactivateUserAsync(int id);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);

        // Información adicional
        Task<IEnumerable<UserDto>> GetActiveUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName);
        Task<UserSummaryDto> GetUserSummaryAsync(int userId);
    }
}