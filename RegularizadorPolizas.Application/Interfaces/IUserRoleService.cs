using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRoleService
{
    Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds, int? assignedByUserId = null);
    Task RemoveUserRoleAsync(int userId, int roleId, int? removedByUserId = null);
}