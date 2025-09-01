using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CourseDx.Data;
using CourseDx.Models;

namespace CourseDx.Controllers
{
    [Authorize]
    public class CourseDescovery : Controller
    {
        private readonly CourseDxContext _context;

        public CourseDescovery(CourseDxContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Load all courses with their details and instructors
            var courses = await _context.Courses
                .Include(c => c.CourseDetals)
                    .ThenInclude(cd => cd.Instractor)
                .Include(c => c.InstractorCourses)
                    .ThenInclude(ic => ic.Instractor)
                .ToListAsync();

            // Load detailed entries (course sessions) with enrollments
            var courseDetals = await _context.CourseDetals
                .Include(cd => cd.Instractor)
                .Include(cd => cd.Course)
                .Include(cd => cd.CourseEnrollment)
                    .ThenInclude(e => e.Student)
                .ToListAsync();

            var instructors = await _context.Instractor.ToListAsync();

            var enrollments = await _context.CourseEnrollment
                .Include(e => e.Student)
                .Include(e => e.CourseDetals)
                .ToListAsync();

            var vm = new CourseDiscoveryViewModel
            {
                Courses = courses,
                CourseDetails = courseDetals,
                Instructors = instructors,
                Enrollments = enrollments
            };

            return View(vm);
        }
    }
}
