using System.ComponentModel.DataAnnotations;

namespace CourseManagement.API.DTOs
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên phải từ 5 đến 100 ký tự")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Range(0, 1000, ErrorMessage = "Giá phải từ 0 đến 1000 USD")]
        public decimal Price { get; set; }
        public string Author { get; set; } = string.Empty;
    }

    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        // Lưu ý: Với kiểu số (decimal, int), phải có dấu ? để phân biệt giữa "không gửi" (null) và "gửi số 0"
        public decimal? Price { get; set; }
        public string? Author { get; set; }
    }

    public class CourseResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; } = string.Empty;
    }
}
