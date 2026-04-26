using CourseManagement.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Khai báo bảng Courses
        public DbSet<Course> Courses { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
