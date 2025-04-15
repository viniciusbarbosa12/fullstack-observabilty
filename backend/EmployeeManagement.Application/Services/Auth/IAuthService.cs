using EmployeeManagement.Application.Dtos.Auth;

namespace EmployeeManagement.Application.Service.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
