using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetActivePermissions()
        {
            try
            {
                var permissions = await _permissionService.GetActivePermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(IEnumerable<PermissionLookupDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PermissionLookupDto>>> GetPermissionsForLookup()
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsForLookupAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("resource/{resource}")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsByResource(string resource)
        {
            try
            {
                var permissions = await _permissionService.GetByResourceAsync(resource);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("resources")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableResources()
        {
            try
            {
                var resources = await _permissionService.GetAvailableResourcesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}