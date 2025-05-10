using RegularizadorPolizas.Application.DTOs.Auth;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> Login(LoginDto loginDto);
        Task<bool> ValidateToken(string token);
    }
}