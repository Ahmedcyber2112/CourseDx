using CourseDx.Data;
using CourseDx.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace CourseDx.Repositories
{
    /// <summary>
    /// Unit of Work implementation for managing transactions and repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDxContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<Course>? _courses;
        private IRepository<Student>? _students;
        private IRepository<Instractor>? _instructors;
        private IRepository<CourseDetals>? _courseDetails;
        private IRepository<CourseEnrollment>? _courseEnrollments;
        private IRepository<InstractorCourses>? _instructorCourses;

        public UnitOfWork(CourseDxContext context)
        {
            _context = context;
        }

        public IRepository<Course> Courses =>
            _courses ??= new Repository<Course>(_context);

        public IRepository<Student> Students =>
            _students ??= new Repository<Student>(_context);

        public IRepository<Instractor> Instructors =>
            _instructors ??= new Repository<Instractor>(_context);

        public IRepository<CourseDetals> CourseDetails =>
            _courseDetails ??= new Repository<CourseDetals>(_context);

        public IRepository<CourseEnrollment> CourseEnrollments =>
            _courseEnrollments ??= new Repository<CourseEnrollment>(_context);

        public IRepository<InstractorCourses> InstructorCourses =>
            _instructorCourses ??= new Repository<InstractorCourses>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
