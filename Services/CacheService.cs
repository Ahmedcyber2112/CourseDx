using Microsoft.Extensions.Caching.Memory;

namespace CourseDx.Services
{
    /// <summary>
    /// Memory cache service implementation
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(10);

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _cache.Set(key, value, options);
            _logger.LogDebug("Cache set for key: {Key}", key);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            Set(key, value, expiration);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _logger.LogDebug("Cache removed for key: {Key}", key);
        }

        public Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(Exists(key));
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null)
        {
            return _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration;
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                _logger.LogDebug("Cache miss for key: {Key}, creating new value", key);
                return factory();
            })!;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            return (await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration;
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                _logger.LogDebug("Cache miss for key: {Key}, creating new value async", key);
                return await factory();
            }))!;
        }
    }

    /// <summary>
    /// Cache keys constants
    /// </summary>
    public static class CacheKeys
    {
        public const string AllCourses = "courses_all";
        public const string AllInstructors = "instructors_all";
        public const string AllStudents = "students_all";
        public const string CourseById = "course_{0}";
        public const string InstructorById = "instructor_{0}";
        public const string StudentById = "student_{0}";
        public const string DashboardStats = "dashboard_stats";
    }
}
