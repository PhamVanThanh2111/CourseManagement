using CourseManagement.API.DTOs;
using CourseManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CourseManagement.API.Services;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public UsersController(IUserService userService, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy ID từ Token của người đang đăng nhập
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var user = await _userService.GetProfileAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}/courses")]
        public async Task<IActionResult> GetUserWithCourses(Guid id)
        {
            var userWithCourses = await _userService.GetUserWithCoursesSpAsync(id);
            if (userWithCourses == null) return NotFound();

            return Ok(userWithCourses);
        }

        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
        {
            // Kiểm tra xem ID người dùng cung cấp có khớp với Token đang đăng nhập hay không
            // để đảm bảo không ai có thể upload file cho người khác
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var currentUserId) || currentUserId != id)
                return Forbid("You don't have permission to update this user's avatar");

            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            var url = await _cloudinaryService.UploadImageAsync(file);
            if (string.IsNullOrEmpty(url))
                return BadRequest("Upload failed");

            var success = await _userService.UpdateUserAvatarAsync(id, url);
            if (!success)
                return NotFound("User not found");

            return Ok(new { AvatarUrl = url });
        }
    }
}
