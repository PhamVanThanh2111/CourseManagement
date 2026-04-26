using AutoMapper;
using CourseManagement.API.DTOs;
using CourseManagement.API.Entities;

namespace CourseManagement.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateCourseDto, Course>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
