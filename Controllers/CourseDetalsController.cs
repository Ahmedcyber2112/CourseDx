using CourseDx.Data;
using CourseDx.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; 

namespace CourseDx.Controllers
{
    [Authorize]
    public class CourseDetalsController : Controller
    {
        private readonly CourseDxContext _context;

        public CourseDetalsController(CourseDxContext context)
        {
            _context = context;
        }

        // GET: CourseDetals
        public async Task<IActionResult> Index()
        {
            var data = _context.CourseDetals
                .Include(c => c.Course)
                .Include(c => c.Instractor);
            return View(await data.ToListAsync());
        }

     
         

        // GET: CourseDetals/Create
        public IActionResult Create()
        {
            ViewData["InstractorId"] = new SelectList(_context.Instractor, "Id", "Name");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        // POST: CourseDetals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseDetals courseDetals)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseDetals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstractorId"] = new SelectList(_context.Instractor, "Id", "Name", courseDetals.InstractorId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseDetals.CourseId);
            return View(courseDetals);
        }

        // GET: CourseDetals/Edit/5/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var courseDetals = await _context.CourseDetals.FindAsync(id);
            if (courseDetals == null) return NotFound();

            ViewData["InstractorId"] = new SelectList(_context.Instractor, "Id", "Name", courseDetals.InstractorId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseDetals.CourseId);
            return View(courseDetals);
        }

        // POST: CourseDetals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseDetals courseDetals)
        {
            if (id != courseDetals.id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseDetals);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseDetalsExists(courseDetals.id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["InstractorId"] = new SelectList(_context.Instractor, "Id", "Name", courseDetals.InstractorId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseDetals.CourseId);
            return View(courseDetals);
        }

        //// GET: CourseDetals/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var courseDetals = await _context.CourseDetals
        //        .Include(c => c.Course)
        //        .Include(c => c.Instractor)
        //        .FirstOrDefaultAsync(m => m.id == id);

        //    if (courseDetals == null) return NotFound();

        //    return View(courseDetals);
        //}

        // POST: CourseDetals/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseDetails = await _context.CourseDetals
                .Include(x=>x.CourseEnrollment)
                .FirstOrDefaultAsync(x=>x.id==id);

            //التحقق من Null
            if (courseDetails == null)
                return NotFound();
             
                _context.CourseEnrollment.RemoveRange(courseDetails.CourseEnrollment);
                _context.CourseDetals.Remove(courseDetails);
                await _context.SaveChangesAsync();
              
            return RedirectToAction(nameof(Index));
        }




        private bool CourseDetalsExists(int id)
        {
            return _context.CourseDetals.Any(e => e.id == id);
        }
        [HttpGet]
        public JsonResult GetCoursesByInstructor(int instructorId)
        {
            // Replace this with your actual logic to get courses by instructor
            var courses = _context.Courses
                .Where(c => c.Id == instructorId)
                .Select(c => new {
                    Value = c.Id,
                    Text = c.Name
                })
                .ToList();

            return Json(courses);
        }
    }

}
