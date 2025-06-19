using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/roles")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoles(int userId, [FromBody] List<int> roleIds)
        {
            int? assignedByUserId = GetCurrentUserId();
            await _userRoleService.SetUserRolesAsync(userId, roleIds, assignedByUserId);
            return NoContent();
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> RemoveRole(int userId, int roleId)
        {
            int? removedByUserId = GetCurrentUserId();
            await _userRoleService.RemoveUserRoleAsync(userId, roleId, removedByUserId);
            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;
        }
    }
}
