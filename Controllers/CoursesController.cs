using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;
using CourseManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // 1. GET: api/courses (Lấy danh sách)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _courseService.GetCoursesAsync();
            return Ok(courses);
        }

        // 2. GET: api/courses/578575EB-69D0-4DB7-F6FE-08DEA2B196CF (Lấy chi tiết theo ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            var result = await _courseService.GetCourseAsync(id);
            if (!result.IsSuccess) return NotFound();

            return Ok(result.Data);
        }

        // GET: api/courses/sp/578575EB-69D0-4DB7-F6FE-08DEA2B196CF
        [HttpGet("sp/{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetCourseBySp(Guid id)
        {
            var result = await _courseService.GetCourseBySpAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        // 3. POST: api/courses (Tạo mới)
        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse(CreateCourseDto courseDto)
        {
            var course = await _courseService.CreateCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        // 4. PUT: api/courses/578575EB-69D0-4DB7-F6FE-08DEA2B196CF (Cập nhật)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseDto courseDto)
        {
            var isSuccess = await _courseService.UpdateCourseAsync(id, courseDto);
            if (!isSuccess) return NotFound();

            return NoContent();
        }

        // 5. DELETE: api/courses/578575EB-69D0-4DB7-F6FE-08DEA2B196CF (Xóa)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var isSuccess = await _courseService.DeleteCourseAsync(id);
            if (!isSuccess) return NotFound();

            return Ok(new { Message = "Đã xóa khóa học thành công" });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
