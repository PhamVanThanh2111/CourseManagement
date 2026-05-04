using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagement.API.Entities
{
    [Table("UserCourses")]
    public class UserCourse
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
