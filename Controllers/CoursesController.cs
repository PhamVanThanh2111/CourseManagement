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

        // GET: api/courses/discount/578575EB-69D0-4DB7-F6FE-08DEA2B196CF?percentage=20
        [HttpGet("discount/{id}")]
        public async Task<ActionResult<CourseDiscountDto>> GetCourseDetailsWithDiscount(Guid id, [FromQuery] decimal percentage = 0)
        {
            var result = await _courseService.GetCourseDetailsWithDiscountAsync(id, percentage);

            if (result == null)
            {
                return NotFound();
            }

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

        // 6. POST: api/courses/578575EB-69D0-4DB7-F6FE-08DEA2B196CF/users/3F2504E0-4F89-11D3-9A0C-0305E82C3301 (Thêm user vào khóa học)
        [HttpPost("{courseId}/users/{userId}")]
        public async Task<IActionResult> AddUserToCourse(Guid courseId, Guid userId)
        {
            var isSuccess = await _courseService.AddUserToCourseAsync(userId, courseId);
            if (!isSuccess) return BadRequest("Không thể thêm người dùng vào khóa học. Có thể họ đã đăng ký khóa học này.");

            return Ok(new { Message = "Đã thêm người dùng vào khóa học thành công" });
        }

        // 7. DELETE: api/courses/578575EB-69D0-4DB7-F6FE-08DEA2B196CF/users/3F2504E0-4F89-11D3-9A0C-0305E82C3301 (Xóa user khỏi khóa học)
        [HttpDelete("{courseId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromCourse(Guid courseId, Guid userId)
        {
            var isSuccess = await _courseService.RemoveUserFromCourseAsync(userId, courseId);
            if (!isSuccess) return NotFound("Người dùng này chưa có trong khóa học");

            return Ok(new { Message = "Đã xóa người dùng khỏi khóa học thành công" });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
