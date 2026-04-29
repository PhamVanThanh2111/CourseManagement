using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;
using CourseManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    }
}