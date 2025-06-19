using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Auth;
using RegularizadorPolizas.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto);
                if (result == null)
                    return Unauthorized("Invalid credentials");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("validate-token")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> ValidateToken([FromBody] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return BadRequest("Token is required");

                var isValid = await _authService.ValidateToken(token);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}