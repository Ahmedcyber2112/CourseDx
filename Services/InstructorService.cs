using CourseDx.Entity;
using CourseDx.Repositories;

namespace CourseDx.Services
{
    /// <summary>
    /// Service implementation for Instructor business logic
    /// </summary>
    public class InstructorService : IInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InstructorService> _logger;

        public InstructorService(IUnitOfWork unitOfWork, ILogger<InstructorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Instractor>> GetAllInstructorsAsync()
        {
            _logger.LogInformation("Fetching all instructors");
            return await _unitOfWork.Instructors.GetAllAsync();
        }

        public async Task<Instractor?> GetInstructorByIdAsync(int id)
        {
            _logger.LogInformation("Fetching instructor with ID: {InstructorId}", id);
            return await _unitOfWork.Instructors.GetByIdAsync(id);
        }

        public async Task<Instractor> CreateInstructorAsync(Instractor instructor)
        {
            _logger.LogInformation("Creating new instructor: {InstructorName}", instructor.Name);
            
            await _unitOfWork.Instructors.AddAsync(instructor);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Instructor created successfully with ID: {InstructorId}", instructor.Id);
            return instructor;
        }

        public async Task<Instractor> UpdateInstructorAsync(Instractor instructor)
        {
            _logger.LogInformation("Updating instructor with ID: {InstructorId}", instructor.Id);
            
            _unitOfWork.Instructors.Update(instructor);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Instructor updated successfully");
            return instructor;
        }

        public async Task<bool> DeleteInstructorAsync(int id)
        {
            _logger.LogInformation("Deleting instructor with ID: {InstructorId}", id);
            
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null)
            {
                _logger.LogWarning("Instructor with ID {InstructorId} not found for deletion", id);
                return false;
            }

            _unitOfWork.Instructors.Remove(instructor);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Instructor deleted successfully");
            return true;
        }

        public async Task<bool> InstructorExistsAsync(int id)
        {
            return await _unitOfWork.Instructors.ExistsAsync(id);
        }

        public async Task<int> GetInstructorCountAsync()
        {
            return await _unitOfWork.Instructors.CountAsync();
        }
    }
}
