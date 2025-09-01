using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseDx.Data;
using CourseDx.Entity;
using CourseDx.Models;
using CourseDx.Models.InstractorCourses;
using Microsoft.AspNetCore.Authorization;

namespace CourseDx.Controllers
{
    [Authorize]
    public class InstractorCoursesController : Controller
    {
        private readonly CourseDxContext _context;

        public InstractorCoursesController(CourseDxContext context)
        {
            _context = context;
        }

        // GET: InstractorCourses
        public async Task<IActionResult> Index()
        {
            var InstCourModel = await _context.InstractorCourses
                .Select(x => new InstractorCoursesInfo
                {
                    Id = x.Id,
                    CourseName = x.Course.Name,
                    InstructorName = x.Instractor.Name
                })
                .ToListAsync();

            return View(InstCourModel);
        }

        // GET: InstractorCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instractorCourses = await _context.InstractorCourses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instractorCourses == null)
            {
                return NotFound();
            }

            return View(instractorCourses);
        }

        // GET: InstractorCourses/Create
        public IActionResult Create()
        {
            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            ViewBag.Instructors = new SelectList(_context.Instractor, "Id", "Name");

            return View();
        }

        // POST: InstractorCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inst_Courses_Model model)
        {
            if (ModelState.IsValid) // Now works correctly!
            {
                var instructorCourse = new InstractorCourses
                {
                    CourseId = model.CourseId,
                    InstractorId = model.InstructorId
                };

                _context.Add(instructorCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // If validation fails, REPOPULATE dropdowns
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");
            ViewBag.Instructors = new SelectList(_context.Instractor.ToList(), "Id", "Name");

            return View(model); // Return model with validation errors
        }

        // GET: InstractorCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instractorCourses = await _context.InstractorCourses.FindAsync(id);
            if (instractorCourses == null)
            {
                return NotFound();
            }

            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");
            ViewBag.Instructors = new SelectList(_context.Instractor.ToList(), "Id", "Name");

            return View(instractorCourses);
        }

        // POST: InstractorCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( InstractorCourses instractorCourses)
        { 
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instractorCourses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstractorCoursesExists(instractorCourses.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");
                ViewBag.Instructors = new SelectList(_context.Instractor.ToList(), "Id", "Name");

                return RedirectToAction(nameof(Index));
            }
            return View(instractorCourses);
        }

        // GET: InstractorCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instractorCourses = await _context.InstractorCourses
                .Include(x => x.Instractor)
                .Include(x => x.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            //var instractorCourses = await _context.InstractorCourses
            //   .Select(x=> new InstractorCoursesInfo
            //       {
            //            Id = x.Id,
            //            CourseName = x.Course.Name,
            //            InstructorName = x.Instractor.Name
            //       }
            //   )
            //   .FirstOrDefaultAsync(m => m.Id == id);


            if (instractorCourses == null)
            {
                return NotFound();
            }

            return View(instractorCourses);
        }

        // POST: InstractorCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instractorCourses = await _context.InstractorCourses.FindAsync(id);
            if (instractorCourses != null)
            {
                _context.InstractorCourses.Remove(instractorCourses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstractorCoursesExists(int id)
        {
            return _context.InstractorCourses.Any(e => e.Id == id);
        }
    }
}
