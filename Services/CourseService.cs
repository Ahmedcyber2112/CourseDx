using CourseDx.Entity;
using CourseDx.Repositories;

namespace CourseDx.Services
{
    /// <summary>
    /// Service implementation for Course business logic
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CourseService> _logger;

        public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            _logger.LogInformation("Fetching all courses");
            return await _unitOfWork.Courses.GetAllAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            _logger.LogInformation("Fetching course with ID: {CourseId}", id);
            return await _unitOfWork.Courses.GetByIdAsync(id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            _logger.LogInformation("Creating new course: {CourseName}", course.Name);
            
            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Course created successfully with ID: {CourseId}", course.Id);
            return course;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _logger.LogInformation("Updating course with ID: {CourseId}", course.Id);
            
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Course updated successfully");
            return course;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            _logger.LogInformation("Deleting course with ID: {CourseId}", id);
            
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for deletion", id);
                return false;
            }

            _unitOfWork.Courses.Remove(course);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Course deleted successfully");
            return true;
        }

        public async Task<bool> CourseExistsAsync(int id)
        {
            return await _unitOfWork.Courses.ExistsAsync(id);
        }

        public async Task<int> GetCourseCountAsync()
        {
            return await _unitOfWork.Courses.CountAsync();
        }
    }
}
