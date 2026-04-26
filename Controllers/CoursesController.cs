using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CoursesController(AppDbContext context, IMapper mapper) { _context = context; _mapper = mapper; }

        // 1. GET: api/courses (Lấy danh sách)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        // 2. GET: api/courses/5 (Lấy chi tiết theo ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { Message = "Không tìm thấy khóa học" });
            return course;
        }

        // 3. POST: api/courses (Tạo mới)
        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse(CreateCourseDto courseDto)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Price = courseDto.Price,
                Author = courseDto.Author,
                CreatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        // 4. PUT: api/courses/5 (Cập nhật)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseDto courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            // Một dòng duy nhất gánh hết các câu lệnh IF bên trên
            _mapper.Map(courseDto, course);

            await _context.SaveChangesAsync();
            return NoContent(); // Trả về 204
        }

        // 5. DELETE: api/courses/5 (Xóa)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đã xóa khóa học thành công" });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
