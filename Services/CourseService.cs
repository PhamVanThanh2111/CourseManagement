using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;
using CourseManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Dapper;
using System.Data;

namespace CourseManagement.API.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public CourseService(AppDbContext context, IMapper mapper, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<(bool IsSuccess, CourseResponseDto? Data)> GetCourseAsync(Guid id)
        {
            string cacheKey = $"course_{id}";

            // 1. Thử lấy từ Redis
            var cachedCourse = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCourse))
            {
                var courseFromCache = JsonSerializer.Deserialize<CourseResponseDto>(cachedCourse);
                return (true, courseFromCache);
            }

            // 2. Không có trong Redis thì vào DB
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return (false, null);

            var response = _mapper.Map<CourseResponseDto>(course);

            // 3. Lưu vào Redis
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options);

            return (true, response);
        }

        public async Task<CourseResponseDto?> GetCourseBySpAsync(Guid id)
        {
            // Trong EF Core, không thể dùng FirstOrDefaultAsync() trực tiếp sau FromSqlRaw chạy Stored Procedure (EXEC) 
            // vì EF Core sẽ cố gắng thêm "SELECT TOP(1)" bao quanh lệnh EXEC, gây ra lỗi SQL syntax.
            // Giải pháp là gọi ToListAsync() / AsEnumerable() trước rồi mới lấy phần tử đầu tiên.
            var courses = await _context.Courses.FromSqlRaw("EXEC sp_GetCourseById @CourseId = {0}", id)
                .AsNoTracking()
                .ToListAsync();

            var course = courses.FirstOrDefault();

            if (course == null) return null;

            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseDiscountDto?> GetCourseDetailsWithDiscountAsync(Guid id, decimal discountPercentage)
        {
            var connection = _context.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CourseId", id);
            parameters.Add("@DiscountPercentage", discountPercentage);

            var course = await connection.QueryFirstOrDefaultAsync<CourseDiscountDto>(
                "sp_GetCourseDetailsWithDiscount",
                parameters,
                commandType: CommandType.StoredProcedure);

            return course;
        }

        public async Task<Course> CreateCourseAsync(CreateCourseDto courseDto)
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
            return course;
        }

        public async Task<bool> UpdateCourseAsync(Guid id, UpdateCourseDto courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _mapper.Map(courseDto, course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUserToCourseAsync(Guid userId, Guid courseId)
        {
            var userCourse = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

            if (userCourse != null) return false; // Đã tham gia

            _context.UserCourses.Add(new UserCourse
            {
                UserId = userId,
                CourseId = courseId
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUserFromCourseAsync(Guid userId, Guid courseId)
        {
            var userCourse = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

            if (userCourse == null) return false; // Không tìm thấy

            _context.UserCourses.Remove(userCourse);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}