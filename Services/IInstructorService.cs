using CourseDx.Entity;

namespace CourseDx.Services
{
    /// <summary>
    /// Service interface for Instructor business logic
    /// </summary>
    public interface IInstructorService
    {
        Task<IEnumerable<Instractor>> GetAllInstructorsAsync();
        Task<Instractor?> GetInstructorByIdAsync(int id);
        Task<Instractor> CreateInstructorAsync(Instractor instructor);
        Task<Instractor> UpdateInstructorAsync(Instractor instructor);
        Task<bool> DeleteInstructorAsync(int id);
        Task<bool> InstructorExistsAsync(int id);
        Task<int> GetInstructorCountAsync();
    }
}
