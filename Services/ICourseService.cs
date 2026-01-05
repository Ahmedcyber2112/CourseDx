using CourseDx.Entity;

namespace CourseDx.Services
{
    /// <summary>
    /// Service interface for Course business logic
    /// </summary>
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<bool> CourseExistsAsync(int id);
        Task<int> GetCourseCountAsync();
    }
}
