using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;
using RegularizadorPolizas.Application.Helpers;

namespace RegularizadorPolizas.Application.Services
{
    public class UserService : IUserService

    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepository;
        private readonly IGenericRepository<Permission> _permissionRepository;
        private readonly IAuditService _auditService;
        private readonly IMapper _mapper;

        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IGenericRepository<RolePermission> rolePermissionRepository,
            IGenericRepository<Permission> permissionRepository,
            IAuditService auditService,
            IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _rolePermissionRepository = rolePermissionRepository ?? throw new ArgumentNullException(nameof(rolePermissionRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = await GetUserRolesInternalAsync(user.Id);
                    userDto.RoleNames = userDto.Roles.Select(r => r.Name).ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving users: {ex.Message}", ex);
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return null;

                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = await GetUserRolesInternalAsync(id);
                userDto.RoleNames = userDto.Roles.Select(r => r.Name).ToList();

                return userDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving user with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var users = await _userRepository.FindAsync(u => u.Email == email);
                var user = users.FirstOrDefault();
                if (user == null) return null;

                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = await GetUserRolesInternalAsync(user.Id);
                userDto.RoleNames = userDto.Roles.Select(r => r.Name).ToList();

                return userDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving user with email {email}: {ex.Message}", ex);
            }
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCreateDto.Nombre))
                    throw new ArgumentException("User name is required");

                var user = new User
                {
                    Nombre = userCreateDto.Nombre.Trim(),
                    Email = userCreateDto.Email?.Trim().ToLower() ?? string.Empty,
                    Activo = userCreateDto.Activo,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    TenantId = userCreateDto.TenantId,
                    Password = PasswordHelper.HashPassword(userCreateDto.Password)
                };

                var createdUser = await _userRepository.AddAsync(user);

                if (userCreateDto.RoleIds.Any())
                {
                    foreach (var roleId in userCreateDto.RoleIds)
                    {
                        await AssignRoleToUserAsync(createdUser.Id, roleId);
                    }
                }

                await _auditService.LogAsync(
                    AuditEventType.UserCreated,
                    $"Usuario creado: {createdUser.Nombre} ({createdUser.Email})",
                    new { UserId = createdUser.Id, Email = createdUser.Email, Roles = userCreateDto.RoleIds });

                return await GetUserByIdAsync(createdUser.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating user: {ex.Message}", ex);
            }
        }

        public async Task UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userUpdateDto.Id);
                if (existingUser == null)
                    throw new ApplicationException($"User with ID {userUpdateDto.Id} not found");

                var oldData = _mapper.Map<UserDto>(existingUser);

                existingUser.Nombre = userUpdateDto.Nombre.Trim();
                existingUser.Email = userUpdateDto.Email?.Trim().ToLower() ?? string.Empty;
                existingUser.Activo = userUpdateDto.Activo;
                existingUser.FechaModificacion = DateTime.Now;
                existingUser.TenantId = userUpdateDto.TenantId;
                if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
                {
                    existingUser.Password = PasswordHelper.HashPassword(userUpdateDto.Password);
                }

                await _userRepository.UpdateAsync(existingUser);

                await _auditService.LogAsync(
                    AuditEventType.UserUpdated, 
                    $"Usuario actualizado: {existingUser.Nombre}",
                    new { UserId = existingUser.Id, OldData = oldData, NewData = userUpdateDto });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating user: {ex.Message}", ex);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new ApplicationException($"User with ID {id} not found");

                if (await UserHasRoleAsync(id, "SuperAdmin"))
                {
                    var superAdmins = await GetUsersByRoleAsync("SuperAdmin");
                    if (superAdmins.Count() <= 1)
                        throw new ApplicationException("Cannot delete the last SuperAdmin user");
                }

                user.Activo = false;
                user.FechaModificacion = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == id && ur.IsActive);
                foreach (var userRole in userRoles)
                {
                    userRole.IsActive = false;
                    await _userRoleRepository.UpdateAsync(userRole);
                }

                await _auditService.LogAsync(
                    AuditEventType.UserDeleted, 
                    $"Usuario eliminado: {user.Nombre}",
                    new { UserId = id, UserName = user.Nombre });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting user: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllUsersAsync();

                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                var users = await _userRepository.FindAsync(u =>
                    (u.Nombre != null && u.Nombre.ToLower().Contains(normalizedSearchTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(normalizedSearchTerm))
                );

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = await GetUserRolesInternalAsync(user.Id);
                    userDto.RoleNames = userDto.Roles.Select(r => r.Name).ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching users: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId)
        {
            return await GetUserRolesInternalAsync(userId);
        }

        public async Task AssignRoleToUserAsync(int userId, int roleId, int? assignedBy = null)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ApplicationException($"User with ID {userId} not found");

                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    throw new ApplicationException($"Role with ID {roleId} not found");

                var existingUserRole = await _userRoleRepository.FindAsync(ur =>
                    ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole.Any())
                {
                    var existing = existingUserRole.First();
                    if (existing.IsActive)
                        throw new ArgumentException($"User already has role '{role.Name}'");

                    existing.IsActive = true;
                    existing.AssignedAt = DateTime.UtcNow;
                    existing.AssignedBy = assignedBy;
                    await _userRoleRepository.UpdateAsync(existing);
                }
                else
                {
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = assignedBy,
                        IsActive = true
                    };

                    await _userRoleRepository.AddAsync(userRole);
                }

                await _auditService.LogAsync(
                    AuditEventType.UserRoleAssigned,
                    $"Rol '{role.Name}' asignado a usuario '{user.Nombre}'",
                    new { UserId = userId, RoleId = roleId, AssignedBy = assignedBy });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error assigning role: {ex.Message}", ex);
            }
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var userRoles = await _userRoleRepository.FindAsync(ur =>
                    ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);

                var userRole = userRoles.FirstOrDefault();
                if (userRole == null)
                    throw new ApplicationException("User does not have this role assigned");

                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role?.Name == "SuperAdmin")
                {
                    var superAdmins = await GetUsersByRoleAsync("SuperAdmin");
                    if (superAdmins.Count() <= 1)
                        throw new ApplicationException("Cannot remove the last SuperAdmin role");
                }

                userRole.IsActive = false;
                await _userRoleRepository.UpdateAsync(userRole);

                var user = await _userRepository.GetByIdAsync(userId);
                await _auditService.LogAsync(
                    AuditEventType.UserRoleRemoved, 
                    $"Rol '{role?.Name}' removido de usuario '{user?.Nombre}'",
                    new { UserId = userId, RoleId = roleId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error removing role: {ex.Message}", ex);
            }
        }

        public async Task<bool> UserHasRoleAsync(int userId, string roleName)
        {
            try
            {
                var userRoles = await _userRoleRepository.FindAsync(ur =>
                    ur.UserId == userId && ur.IsActive);

                foreach (var userRole in userRoles)
                {
                    var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                    if (role?.Name == roleName && role.IsActive)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            try
            {
                var permissions = await _permissionRepository.FindAsync(p =>
                    p.Name == permissionName && p.IsActive);

                var permission = permissions.FirstOrDefault();
                if (permission == null)
                    return false;

                var userRoles = await _userRoleRepository.FindAsync(ur =>
                    ur.UserId == userId && ur.IsActive);

                if (!userRoles.Any())
                    return false;

                var userRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

                var rolePermissions = await _rolePermissionRepository.FindAsync(rp =>
                    userRoleIds.Contains(rp.RoleId) &&
                    rp.PermissionId == permission.Id &&
                    rp.IsActive);

                return rolePermissions.Any();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(ex,
                    $"Error checking permission '{permissionName}' for user {userId}");
                return false;
            }
        }

        public async Task ActivateUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new ApplicationException($"User with ID {id} not found");

                user.Activo = true;
                user.FechaModificacion = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                await _auditService.LogAsync(
                    AuditEventType.UserActivated,
                    $"Usuario activado: {user.Nombre}",
                    new { UserId = id });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error activating user: {ex.Message}", ex);
            }
        }

        public async Task DeactivateUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new ApplicationException($"User with ID {id} not found");

                if (await UserHasRoleAsync(id, "SuperAdmin"))
                {
                    var superAdmins = await GetUsersByRoleAsync("SuperAdmin");
                    if (superAdmins.Count() <= 1)
                        throw new ApplicationException("Cannot deactivate the last SuperAdmin user");
                }

                user.Activo = false;
                user.FechaModificacion = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                await _auditService.LogAsync(
                    AuditEventType.UserDeactivated,
                    $"Usuario desactivado: {user.Nombre}",
                    new { UserId = id });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deactivating user: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            try
            {
                var users = await _userRepository.FindAsync(u => u.Email == email.ToLower());
                return users.Any(u => excludeId == null || u.Id != excludeId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        {
            try
            {
                var users = await _userRepository.FindAsync(u => u.Activo);
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = await GetUserRolesInternalAsync(user.Id);
                    userDto.RoleNames = userDto.Roles.Select(r => r.Name).ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving active users: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            try
            {
                var roles = await _roleRepository.FindAsync(r => r.Name == roleName && r.IsActive);
                var role = roles.FirstOrDefault();
                if (role == null) return new List<UserDto>();

                var userRoles = await _userRoleRepository.FindAsync(ur => ur.RoleId == role.Id && ur.IsActive);
                var userDtos = new List<UserDto>();

                foreach (var userRole in userRoles)
                {
                    var user = await _userRepository.GetByIdAsync(userRole.UserId);
                    if (user != null && user.Activo)
                    {
                        var userDto = _mapper.Map<UserDto>(user);
                        userDto.Roles = await GetUserRolesInternalAsync(user.Id);
                        userDto.RoleNames = userDto.Roles.Select(r => r.Name).ToList();
                        userDtos.Add(userDto);
                    }
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving users by role: {ex.Message}", ex);
            }
        }

        public async Task<UserSummaryDto> GetUserSummaryAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return null;

                var roles = await GetUserRolesInternalAsync(userId);
                var primaryRole = roles.FirstOrDefault()?.Name ?? "Sin rol";

                return new UserSummaryDto
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Email = user.Email,
                    Activo = user.Activo,
                    TotalRoles = roles.Count(),
                    PrimaryRole = primaryRole,
                    UltimaActividad = user.FechaModificacion,
                    PuedeEliminar = !await UserHasRoleAsync(userId, "SuperAdmin") ||
                                   (await GetUsersByRoleAsync("SuperAdmin")).Count() > 1
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving user summary: {ex.Message}", ex);
            }
        }

        private async Task<List<RoleDto>> GetUserRolesInternalAsync(int userId)
        {
            try
            {
                var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId && ur.IsActive);
                var roles = new List<RoleDto>();

                foreach (var userRole in userRoles)
                {
                    var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                    if (role != null && role.IsActive)
                    {
                        roles.Add(_mapper.Map<RoleDto>(role));
                    }
                }

                return roles;
            }
            catch
            {
                return new List<RoleDto>();
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId)
        {
            try
            {
                var permissions = new List<string>();

                var userRoles = await _userRoleRepository.FindAsync(ur =>
                    ur.UserId == userId && ur.IsActive);

                foreach (var userRole in userRoles)
                {
                    var rolePermissions = await _rolePermissionRepository.FindAsync(rp =>
                        rp.RoleId == userRole.RoleId && rp.IsActive);

                    foreach (var rolePermission in rolePermissions)
                    {
                        var permission = await _permissionRepository.GetByIdAsync(rolePermission.PermissionId);
                        if (permission != null && permission.IsActive)
                        {
                            permissions.Add(permission.Name);
                        }
                    }
                }

                return permissions.Distinct().ToList();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(ex, $"Error getting permissions for user {userId}");
                return new List<string>();
            }
        }

        public async Task PatchUserAsync(int id, UserPatchDto patchDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ApplicationException($"User with ID {id} not found");

            if (patchDto.Nombre != null && string.IsNullOrWhiteSpace(patchDto.Nombre))
                throw new ArgumentException("El nombre no puede ser vacío.");
            if (patchDto.Password != null && string.IsNullOrWhiteSpace(patchDto.Password))
                throw new ArgumentException("La contraseña no puede ser vacía.");
            if (patchDto.RoleIds != null && !patchDto.RoleIds.Any())
                throw new ArgumentException("Debe asignar al menos un rol.");

            if (patchDto.Nombre != null)
                user.Nombre = patchDto.Nombre.Trim();
            if (patchDto.Email != null)
                user.Email = patchDto.Email.Trim().ToLower();
            if (patchDto.Activo.HasValue)
                user.Activo = patchDto.Activo.Value;
            if (patchDto.TenantId != null)
                user.TenantId = patchDto.TenantId;
            if (patchDto.Password != null)
                user.Password = PasswordHelper.HashPassword(patchDto.Password);

            user.FechaModificacion = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditEventType.UserUpdated,
                $"Usuario actualizado parcialmente: {user.Nombre}",
                new { UserId = user.Id, Patch = patchDto }
            );
        }

    }
}