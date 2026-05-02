using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;

namespace CourseManagement.API.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<(bool IsSuccess, CourseResponseDto? Data)> GetCourseAsync(Guid id);
        Task<CourseResponseDto?> GetCourseBySpAsync(Guid id);
        Task<Course> CreateCourseAsync(CreateCourseDto courseDto);
        Task<bool> UpdateCourseAsync(Guid id, UpdateCourseDto courseDto);
        Task<bool> DeleteCourseAsync(Guid id);
    }
}