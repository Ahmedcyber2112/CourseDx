using AutoMapper;
using CourseDx.DTOs;
using CourseDx.Entity;

namespace CourseDx.Mapping
{
    /// <summary>
    /// AutoMapper profile for entity to DTO mappings
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student mappings
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Full_Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.adddress))
                .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.CourseEnrollment.Count));

            CreateMap<CreateStudentDto, Student>()
                .ForMember(dest => dest.Full_Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.adddress, opt => opt.MapFrom(src => src.Address));

            CreateMap<UpdateStudentDto, Student>()
                .ForMember(dest => dest.Full_Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.adddress, opt => opt.MapFrom(src => src.Address));

            // Course mappings
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.SessionCount, opt => opt.MapFrom(src => src.CourseDetals.Count))
                .ForMember(dest => dest.InstructorCount, opt => opt.MapFrom(src => src.InstractorCourses.Count));

            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // Instructor mappings
            CreateMap<Instractor, InstructorDto>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender == 0 ? "Male" : src.Gender == 1 ? "Female" : "Other"))
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.InstractorCourses.Count));

            CreateMap<CreateInstructorDto, Instractor>();
            CreateMap<UpdateInstructorDto, Instractor>();
        }
    }
}
