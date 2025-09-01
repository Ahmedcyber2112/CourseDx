using CourseDx.Data;
using CourseDx.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseDx.Controllers
{
    [Authorize]
    public class InstractorController : Controller
    {
        private readonly CourseDxContext _context;

        public InstractorController(CourseDxContext context)
        {
            _context = context;
        }

        // GET: Instractor
        public async Task<IActionResult> Index()
        {
            return View(await _context.Instractor.ToListAsync());
        }

        // GET: Instractor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instractor = await _context.Instractor.FirstOrDefaultAsync(m => m.Id == id);
            if (instractor == null) return NotFound();

            return View(instractor);
        }

        // GET: Instractor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instractor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Instractor instractor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instractor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instractor);
        }

        // GET: Instractor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var instractor = await _context.Instractor.FindAsync(id);
            if (instractor == null) return NotFound();

            return View(instractor);
        }

        // POST: Instractor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Instractor instractor)
        {
            if (id != instractor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instractor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Instractor.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instractor);
        }

        // GET: Instractor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instractor = await _context.Instractor.FirstOrDefaultAsync(m => m.Id == id);
            if (instractor == null) return NotFound();

            return View(instractor);
        }

        // POST: Instractor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instractor = await _context.Instractor.FindAsync(id);
            if (instractor != null)
            {
                _context.Instractor.Remove(instractor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));






        }

        // POST: Instractor/Detals/6
        public async Task<IActionResult> Detals(int? id)
        {
            if (id == null) return NotFound();

            var instractor = await _context.Instractor.FirstOrDefaultAsync(m => m.Id == id);
            if (instractor == null) return NotFound();

            return View(instractor);
        }







    }
}
