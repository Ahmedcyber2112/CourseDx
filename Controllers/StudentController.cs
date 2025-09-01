using CourseDx.Data;
using CourseDx.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseDx.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {

        private readonly CourseDxContext _context;

        public StudentController(CourseDxContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {   
            return View(_context.Students.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        { 
                _context.Students.Add(student);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); 
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student); 
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Student == null)
            {
                return NotFound();
            }

            return View(Student);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Student = await _context.Students.FindAsync(id);
            if (Student != null)
            {
                _context.Students.Remove(Student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
