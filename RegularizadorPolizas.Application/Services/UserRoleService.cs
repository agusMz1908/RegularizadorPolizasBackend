using System.Collections.Generic;
using System.Threading.Tasks;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Enums;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditService _auditService;

    public UserRoleService(IUserRoleRepository userRoleRepository, IAuditService auditService)
    {
        _userRoleRepository = userRoleRepository;
        _auditService = auditService;
    }

    public async Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds, int? assignedByUserId = null)
    {
        await _userRoleRepository.SetUserRolesAsync(userId, roleIds, assignedByUserId);

        await _auditService.LogAsync(
            AuditEventType.UserRolesChanged,
            $"Roles asignados a usuario {userId}",
            new { userId, roleIds, assignedByUserId }
        );
    }

    public async Task RemoveUserRoleAsync(int userId, int roleId, int? removedByUserId = null)
    {
        await _userRoleRepository.RemoveUserRoleAsync(userId, roleId, removedByUserId);

        await _auditService.LogAsync(
            AuditEventType.UserRoleRemoved,
            $"Rol {roleId} removido de usuario {userId}",
            new { userId, roleId, removedByUserId }
        );
    }
}