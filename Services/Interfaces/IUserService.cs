using CourseManagement.API.DTOs;

namespace CourseManagement.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetProfileAsync(Guid userId);
        Task<UserWithCoursesDto?> GetUserWithCoursesSpAsync(Guid userId);
        Task<bool> UpdateUserAvatarAsync(Guid userId, string avatarUrl);
    }
}