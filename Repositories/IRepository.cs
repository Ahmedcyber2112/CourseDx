using System.Linq.Expressions;
using CourseDx.DTOs;

namespace CourseDx.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
        
        // Pagination support
        Task<PaginatedResponse<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PaginatedResponse<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
    }
}
