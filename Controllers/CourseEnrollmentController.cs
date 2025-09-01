using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseDx.Entity;
using CourseDx.Data;
using CourseDx.Models.CourseEnrollment;
using Microsoft.AspNetCore.Authorization;

namespace CourseDx.Controllers
{
    [Authorize]
    public class CourseEnrollmentController : Controller
    {
        private readonly CourseDxContext _context;

        public CourseEnrollmentController(CourseDxContext context)
        {
            _context = context;
        }

        // GET: CourseEnrollment
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.CourseEnrollment
                .Include(e => e.Student)
                .Include(e => e.CourseDetals.Instractor)
                .Include(e => e.CourseDetals.Course)
                .Include(e => e.CourseDetals)
                .ToListAsync();

            return View(enrollments);

        }

        //// GET: CourseEnrollment/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var enrollment = await _context.CourseEnrollment
        //        .Include(e => e.Student)
        //        .Include(e => e.CourseDetals)
        //        .FirstOrDefaultAsync(e => e.Id == id);

        //    if (enrollment == null)
        //        return NotFound();

        //    return View(enrollment);
        //}

        // GET: CourseEnrollment/Create
        public IActionResult Create(int? id)
        {
            if (id == null) return RedirectToAction("Index", "CourseEnrollment");

            var result = _context.CourseDetals
                                .Include(x => x.Course)
                                .Include(x => x.Instractor)
                                .Where(x => x.id == id)
                                .ToList();

            if (result.Count == 0) return RedirectToAction("Index", "CourseEnrollment");

            ViewBag.CourseName = result.FirstOrDefault()?.Course.Name;
            ViewBag.InstructorName = result.FirstOrDefault()?.Instractor.Name;
            ViewBag.From = result.FirstOrDefault()?.From_Date;
            ViewBag.To = result.FirstOrDefault()?.To_Date;
            ViewBag.Price = result.FirstOrDefault()?.Price;


            ViewBag.Students = new SelectList(_context.Students, "Id", "Full_Name");
            ViewBag.course_details_id = id;
            return View();
        }

        // POST: CourseEnrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Create model)
        {
            if (ModelState.IsValid)
            {
                var enrollment = new CourseEnrollment()
                {
                    CourseDetalsId = model.CourseDetalsId,
                    StudentId = model.StudentId
                };
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Students = new SelectList(_context.Students, "Id", "Full_Name", model.StudentId);
            ViewBag.CourseDetails = new SelectList(_context.CourseDetals, "id", "Title", model.CourseDetalsId);

            return View(model);
        }

        //// GET: CourseEnrollment/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var enrollment = await _context.CourseEnrollment.FindAsync(id);
        //    if (enrollment == null)
        //        return NotFound();

        //    ViewBag.StudentId = new SelectList(_context.Students, "Id", "Full_Name", enrollment.StudentId);
        //    ViewBag.CourseDetalsId = new SelectList(_context.CourseDetals, "id", "Title", enrollment.CourseDetalsId);

        //    return View(enrollment);
        //}

        //// POST: CourseEnrollment/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, CourseEnrollment enrollment)
        //{
        //    if (id != enrollment.Id)
        //        return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(enrollment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CourseEnrollmentExists(enrollment.Id))
        //                return NotFound();
        //            else
        //                throw;
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.StudentId = new SelectList(_context.Students, "Id", "Full_Name", enrollment.StudentId);
        //    ViewBag.CourseDetalsId = new SelectList(_context.CourseDetals, "id", "Title", enrollment.CourseDetalsId);

        //    return View(enrollment);
        //}

     
   

        // POST: CourseEnrollment/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.CourseEnrollment.FindAsync(id);
            if (enrollment != null)
            {
                _context.CourseEnrollment.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
         


        private bool CourseEnrollmentExists(int id)
        {
            return _context.CourseEnrollment.Any(e => e.Id == id);
        }
    }
}