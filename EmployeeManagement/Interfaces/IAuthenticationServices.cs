using EmployeeManagement.Domain.Dtos;

namespace EmployeeManagement.Interfaces
{
    public interface IAuthenticationServices
    {
       Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDto dto);
    }
}
