using CourseDx.Entity;

namespace CourseDx.Repositories
{
    /// <summary>
    /// Unit of Work interface for managing transactions and repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Course> Courses { get; }
        IRepository<Student> Students { get; }
        IRepository<Instractor> Instructors { get; }
        IRepository<CourseDetals> CourseDetails { get; }
        IRepository<CourseEnrollment> CourseEnrollments { get; }
        IRepository<InstractorCourses> InstructorCourses { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
