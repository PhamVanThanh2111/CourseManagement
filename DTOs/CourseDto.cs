namespace CourseManagement.API.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
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
}
