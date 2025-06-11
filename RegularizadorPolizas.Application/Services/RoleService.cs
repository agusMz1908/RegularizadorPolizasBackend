using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuditService _auditService;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IRolePermissionRepository rolePermissionRepository,
            IPermissionRepository permissionRepository,
            IAuditService auditService,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
            _auditService = auditService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

            foreach (var roleDto in roleDtos)
            {
                roleDto.TotalUsers = await GetRoleUserCountAsync(roleDto.Id);
                roleDto.TotalPermissions = (await GetRolePermissionsAsync(roleDto.Id)).Count();
            }

            return roleDtos;
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return null;

            var roleDto = _mapper.Map<RoleDto>(role);
            roleDto.TotalUsers = await GetRoleUserCountAsync(id);
            roleDto.TotalPermissions = (await GetRolePermissionsAsync(id)).Count();

            return roleDto;
        }

        public async Task<RoleDto> GetRoleByNameAsync(string name)
        {
            var role = await _roleRepository.GetByNameAsync(name);
            if (role == null) return null;

            var roleDto = _mapper.Map<RoleDto>(role);
            roleDto.TotalUsers = await GetRoleUserCountAsync(role.Id);
            roleDto.TotalPermissions = (await GetRolePermissionsAsync(role.Id)).Count();

            return roleDto;
        }

        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            var roles = await _roleRepository.GetActiveRolesAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<IEnumerable<RoleLookupDto>> GetRolesForLookupAsync()
        {
            var roles = await GetActiveRolesAsync();
            return roles.Select(r => new RoleLookupDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleDto.Name))
                    throw new ArgumentException("Role name is required");

                if (await ExistsByNameAsync(roleDto.Name))
                    throw new ArgumentException($"Role with name '{roleDto.Name}' already exists");

                var role = _mapper.Map<Role>(roleDto);
                role.CreatedAt = DateTime.UtcNow;
                role.UpdatedAt = DateTime.UtcNow;
                role.IsActive = true;

                var createdRole = await _roleRepository.AddAsync(role);

                await _auditService.LogAsync(
                    AuditEventType.ClientCreated, // Cambiar por RoleCreated cuando esté disponible
                    $"Rol creado: {createdRole.Name}",
                    new { RoleId = createdRole.Id, RoleName = createdRole.Name });

                return _mapper.Map<RoleDto>(createdRole);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating role: {ex.Message}", ex);
            }
        }

        public async Task UpdateRoleAsync(RoleDto roleDto)
        {
            try
            {
                var existingRole = await _roleRepository.GetByIdAsync(roleDto.Id);
                if (existingRole == null)
                    throw new ApplicationException($"Role with ID {roleDto.Id} not found");

                if (await ExistsByNameAsync(roleDto.Name, roleDto.Id))
                    throw new ArgumentException($"Role with name '{roleDto.Name}' already exists");

                var oldData = _mapper.Map<RoleDto>(existingRole);

                existingRole.Name = roleDto.Name;
                existingRole.Description = roleDto.Description;
                existingRole.IsActive = roleDto.IsActive;
                existingRole.UpdatedAt = DateTime.UtcNow;

                await _roleRepository.UpdateAsync(existingRole);

                await _auditService.LogAsync(
                    AuditEventType.ClientUpdated, // Cambiar por RoleUpdated cuando esté disponible
                    $"Rol actualizado: {existingRole.Name}",
                    new { RoleId = existingRole.Id, OldData = oldData, NewData = roleDto });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating role: {ex.Message}", ex);
            }
        }

        public async Task DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                    throw new ApplicationException($"Role with ID {id} not found");

                // Verificar si es un rol del sistema que no se puede eliminar
                if (role.Name == "SuperAdmin")
                    throw new ApplicationException("Cannot delete SuperAdmin role");

                var userCount = await GetRoleUserCountAsync(id);
                if (userCount > 0)
                    throw new ApplicationException($"Cannot delete role. It has {userCount} users assigned");

                role.IsActive = false;
                role.UpdatedAt = DateTime.UtcNow;
                await _roleRepository.UpdateAsync(role);

                // Desactivar permisos del rol
                var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(id);
                foreach (var rp in rolePermissions.Where(rp => rp.IsActive))
                {
                    rp.IsActive = false;
                    await _rolePermissionRepository.UpdateAsync(rp);
                }

                await _auditService.LogAsync(
                    AuditEventType.ClientDeleted, // Cambiar por RoleDeleted cuando esté disponible
                    $"Rol eliminado: {role.Name}",
                    new { RoleId = id, RoleName = role.Name });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting role: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _roleRepository.ExistsByNameAsync(name, excludeId);
        }

        public async Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            var rolePermissions = await _rolePermissionRepository.GetActiveRolePermissionsAsync(roleId);
            var permissions = rolePermissions.Select(rp => rp.Permission);
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task AssignPermissionToRoleAsync(int roleId, int permissionId, int? grantedBy = null)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    throw new ApplicationException($"Role with ID {roleId} not found");

                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                    throw new ApplicationException($"Permission with ID {permissionId} not found");

                var existingRolePermission = await _rolePermissionRepository.GetByRoleAndPermissionAsync(roleId, permissionId);
                if (existingRolePermission != null)
                {
                    if (existingRolePermission.IsActive)
                        throw new ArgumentException($"Role already has permission '{permission.Name}'");

                    existingRolePermission.IsActive = true;
                    existingRolePermission.GrantedAt = DateTime.UtcNow;
                    existingRolePermission.GrantedBy = grantedBy;
                    await _rolePermissionRepository.UpdateAsync(existingRolePermission);
                }
                else
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = grantedBy,
                        IsActive = true
                    };

                    await _rolePermissionRepository.AddAsync(rolePermission);
                }

                await _auditService.LogAsync(
                    AuditEventType.ClientUpdated, // Cambiar por RolePermissionAssigned cuando esté disponible
                    $"Permiso '{permission.Name}' asignado al rol '{role.Name}'",
                    new { RoleId = roleId, PermissionId = permissionId, GrantedBy = grantedBy });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error assigning permission to role: {ex.Message}", ex);
            }
        }

        public async Task RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            try
            {
                var rolePermission = await _rolePermissionRepository.GetByRoleAndPermissionAsync(roleId, permissionId);
                if (rolePermission == null || !rolePermission.IsActive)
                    throw new ApplicationException("Role does not have this permission assigned");

                rolePermission.IsActive = false;
                await _rolePermissionRepository.UpdateAsync(rolePermission);

                await _auditService.LogAsync(
                    AuditEventType.ClientUpdated, // Cambiar por RolePermissionRemoved cuando esté disponible
                    $"Permiso '{rolePermission.Permission.Name}' removido del rol '{rolePermission.Role.Name}'",
                    new { RoleId = roleId, PermissionId = permissionId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error removing permission from role: {ex.Message}", ex);
            }
        }

        public async Task<bool> RoleHasPermissionAsync(int roleId, string permissionName)
        {
            try
            {
                var rolePermissions = await _rolePermissionRepository.GetActiveRolePermissionsAsync(roleId);
                return rolePermissions.Any(rp => rp.Permission.Name == permissionName);
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId)
        {
            var userRoles = await _userRoleRepository.GetByRoleIdAsync(roleId);
            var users = userRoles.Where(ur => ur.IsActive).Select(ur => ur.User);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<int> GetRoleUserCountAsync(int roleId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetByRoleIdAsync(roleId);
                return userRoles.Count(ur => ur.IsActive && ur.User.Activo);
            }
            catch
            {
                return 0;
            }
        }
    }
}