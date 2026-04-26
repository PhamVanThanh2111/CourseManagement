namespace CourseManagement.API.DTOs
{
    public class UserDto
    {
    }

    public record RegisterDto(string Username, string Email, string Password, string FullName);
    public record LoginDto(string Username, string Password);
    public record UpdateUserDto(string? Email, string? FullName);
    public record UserResponseDto(Guid Id, string Username, string Email, string FullName, string Role);
}
