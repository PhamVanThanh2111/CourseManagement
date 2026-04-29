using CourseManagement.API.Data;
using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;
using CourseManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterDto dto)
        {
            // Kiểm tra username đã tồn tại chưa
            if (await _context.Set<User>().AnyAsync(u => u.Username == dto.Username))
            {
                return (false, "Username đã tồn tại");
            }

            // Mapping từ DTO sang Entity và Hash mật khẩu
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            // Lưu người dùng mới vào database
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            return (true, "Đăng ký thành công");
        }

        public async Task<(bool IsSuccess, string Message, string? Token)> LoginAsync(LoginDto dto)
        {
            // Tìm user theo username
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Username == dto.Username);

            // Kiểm tra user có tồn tại và mật khẩu có khớp không
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return (false, "Sai tài khoản hoặc mật khẩu", null);
            }

            // Tạo token JWT
            var token = GenerateJwtToken(user);
            return (true, "Đăng nhập thành công", token);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}