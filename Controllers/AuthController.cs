using CourseManagement.API.DTOs;
using CourseManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Gọi service xử lý logic đăng ký
            var result = await _authService.RegisterAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Gọi service xử lý logic đăng nhập
            var result = await _authService.LoginAsync(dto);

            if (!result.IsSuccess)
            {
                return Unauthorized(result.Message);
            }

            return Ok(new { Token = result.Token });
        }
    }
}
