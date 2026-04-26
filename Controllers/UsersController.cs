using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CourseManagement.API.Entities;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Set<User>().ToListAsync();
            return Ok(_mapper.Map<IEnumerable<UserResponseDto>>(users));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy ID từ Token của người đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Set<User>().FindAsync(Guid.Parse(userId!));
            return Ok(_mapper.Map<UserResponseDto>(user));
        }
    }
}
