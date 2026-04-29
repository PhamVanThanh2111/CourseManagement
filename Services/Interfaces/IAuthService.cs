using CourseManagement.API.DTOs;

namespace CourseManagement.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterDto dto);
        Task<(bool IsSuccess, string Message, string? Token)> LoginAsync(LoginDto dto);
    }
}