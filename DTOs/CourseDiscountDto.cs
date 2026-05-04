namespace CourseManagement.API.DTOs
{
    public class CourseDiscountDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string Author { get; set; } = string.Empty;
    }
}