using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;

namespace RegularizadorPolizas.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IAuditService _auditService;
        private readonly IMapper _mapper;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IRolePermissionRepository rolePermissionRepository,
            IAuditService auditService,
            IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _auditService = auditService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task<PermissionDto> GetPermissionByIdAsync(int id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task<PermissionDto> GetPermissionByNameAsync(string name)
        {
            var permission = await _permissionRepository.GetByNameAsync(name);
            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync()
        {
            var permissions = await _permissionRepository.GetActivePermissionsAsync();
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task<IEnumerable<PermissionLookupDto>> GetPermissionsForLookupAsync()
        {
            var permissions = await GetActivePermissionsAsync();
            return permissions.Select(p => new PermissionLookupDto
            {
                Id = p.Id,
                Name = p.Name,
                Resource = p.Resource,
                Action = p.Action
            });
        }

        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(permissionDto.Name))
                    throw new ArgumentException("Permission name is required");

                if (await ExistsByNameAsync(permissionDto.Name))
                    throw new ArgumentException($"Permission with name '{permissionDto.Name}' already exists");

                var permission = _mapper.Map<Permission>(permissionDto);
                permission.CreatedAt = DateTime.UtcNow;
                permission.IsActive = true;

                var createdPermission = await _permissionRepository.AddAsync(permission);

                await _auditService.LogAsync(
                    AuditEventType.ClientCreated, // Cambiar por PermissionCreated cuando esté disponible
                    $"Permiso creado: {createdPermission.Name}",
                    new { PermissionId = createdPermission.Id, PermissionName = createdPermission.Name });

                return _mapper.Map<PermissionDto>(createdPermission);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating permission: {ex.Message}", ex);
            }
        }

        public async Task UpdatePermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                var existingPermission = await _permissionRepository.GetByIdAsync(permissionDto.Id);
                if (existingPermission == null)
                    throw new ApplicationException($"Permission with ID {permissionDto.Id} not found");

                if (await ExistsByNameAsync(permissionDto.Name, permissionDto.Id))
                    throw new ArgumentException($"Permission with name '{permissionDto.Name}' already exists");

                var oldData = _mapper.Map<PermissionDto>(existingPermission);

                existingPermission.Name = permissionDto.Name;
                existingPermission.Resource = permissionDto.Resource;
                existingPermission.Action = permissionDto.Action;
                existingPermission.Description = permissionDto.Description;
                existingPermission.IsActive = permissionDto.IsActive;

                await _permissionRepository.UpdateAsync(existingPermission);

                await _auditService.LogAsync(
                    AuditEventType.ClientUpdated, // Cambiar por PermissionUpdated cuando esté disponible
                    $"Permiso actualizado: {existingPermission.Name}",
                    new { PermissionId = existingPermission.Id, OldData = oldData, NewData = permissionDto });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating permission: {ex.Message}", ex);
            }
        }

        public async Task DeletePermissionAsync(int id)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(id);
                if (permission == null)
                    throw new ApplicationException($"Permission with ID {id} not found");

                var usageCount = await GetPermissionUsageCountAsync(id);
                if (usageCount > 0)
                    throw new ApplicationException($"Cannot delete permission. It is used by {usageCount} role(s)");

                permission.IsActive = false;
                await _permissionRepository.UpdateAsync(permission);

                await _auditService.LogAsync(
                    AuditEventType.ClientDeleted, // Cambiar por PermissionDeleted cuando esté disponible
                    $"Permiso eliminado: {permission.Name}",
                    new { PermissionId = id, PermissionName = permission.Name });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting permission: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _permissionRepository.ExistsByNameAsync(name, excludeId);
        }

        public async Task<IEnumerable<PermissionDto>> GetByResourceAsync(string resource)
        {
            var permissions = await _permissionRepository.GetByResourceAsync(resource);
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task<IEnumerable<string>> GetAvailableResourcesAsync()
        {
            var permissions = await _permissionRepository.GetActivePermissionsAsync();
            return permissions.Select(p => p.Resource).Distinct().OrderBy(r => r);
        }

        public async Task<IEnumerable<string>> GetAvailableActionsAsync()
        {
            var permissions = await _permissionRepository.GetActivePermissionsAsync();
            return permissions.Select(p => p.Action).Distinct().OrderBy(a => a);
        }

        public async Task<IEnumerable<RoleDto>> GetPermissionRolesAsync(int permissionId)
        {
            var rolePermissions = await _rolePermissionRepository.GetByPermissionIdAsync(permissionId);
            var roles = rolePermissions.Where(rp => rp.IsActive).Select(rp => rp.Role);
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<int> GetPermissionUsageCountAsync(int permissionId)
        {
            try
            {
                var rolePermissions = await _rolePermissionRepository.GetByPermissionIdAsync(permissionId);
                return rolePermissions.Count(rp => rp.IsActive && rp.Role.IsActive);
            }
            catch
            {
                return 0;
            }
        }
    }
}