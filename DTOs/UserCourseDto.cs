using System.ComponentModel.DataAnnotations;

namespace CourseManagement.API.DTOs
{
    public class UserCourseDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid CourseId { get; set; }
    }

    public class UserWithCoursesDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<CourseResponseDto> Courses { get; set; } = new();
    }
}