using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;
using CourseManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;

namespace CourseManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Set<User>().ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetProfileAsync(Guid userId)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null) return null;
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserWithCoursesDto?> GetUserWithCoursesSpAsync(Guid userId)
        {
            var connection = _context.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            // Dapper call to stored procedure
            using (var multi = await connection.QueryMultipleAsync(
                "sp_GetUserWithCourses",
                parameters,
                commandType: CommandType.StoredProcedure))
            {
                Console.WriteLine("Multi", multi);
                // Result 1: User info
                var user = await multi.ReadFirstOrDefaultAsync<UserWithCoursesDto>();

                if (user != null)
                {
                    // Result 2: List of courses
                    var courses = await multi.ReadAsync<CourseResponseDto>();
                    user.Courses = courses.ToList();
                }

                return user;
            }
        }
    }
}