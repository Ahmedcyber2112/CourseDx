using CourseDx.Entity;
using CourseDx.Repositories;

namespace CourseDx.Services
{
    /// <summary>
    /// Service implementation for Student business logic
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            _logger.LogInformation("Fetching all students");
            return await _unitOfWork.Students.GetAllAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            _logger.LogInformation("Fetching student with ID: {StudentId}", id);
            return await _unitOfWork.Students.GetByIdAsync(id);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _logger.LogInformation("Creating new student: {StudentName}", student.Full_Name);
            
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Student created successfully with ID: {StudentId}", student.Id);
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", student.Id);
            
            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Student updated successfully");
            return student;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);
            
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found for deletion", id);
                return false;
            }

            _unitOfWork.Students.Remove(student);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Student deleted successfully");
            return true;
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _unitOfWork.Students.ExistsAsync(id);
        }

        public async Task<int> GetStudentCountAsync()
        {
            return await _unitOfWork.Students.CountAsync();
        }
    }
}
