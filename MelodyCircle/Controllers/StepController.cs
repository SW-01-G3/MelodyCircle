using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text;

namespace MelodyCircle.Controllers
{
    public class StepController : Controller
    {
        private readonly ApplicationDbContext _context;
        private Guid _tutorialId;

        public StepController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Step
        public async Task<IActionResult> Index(Guid? tutorialId)
        {
            if (tutorialId == null)
                return NotFound();

            _tutorialId = tutorialId.Value;

            var steps = await _context.Steps
                .Include(s => s.Tutorial)
                .Where(s => s.TutorialId == _tutorialId)
                .ToListAsync();

            return View(steps);
        }

        // GET: Step/Create
        public IActionResult Create(Guid? tutorialId)
        {
            Console.WriteLine(_tutorialId);

            ViewBag.TutorialId = tutorialId;
            return View();
        }

        // POST: Step/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TutorialId,Title,Content,Order")] Step step)
        {
            _context.Add(step);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { tutorialId = step.TutorialId });
        }

        // GET: Step/Edit/id
        public async Task<IActionResult> Edit(Guid? tutorialId)
        {
            if (tutorialId == null)
                return NotFound();

            var step = await _context.Steps.FindAsync(tutorialId);

            if (step == null)
                return NotFound();

            return View(step);
        }

        // POST: Step/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid tutorialId, [Bind("Id,TutorialId,Title,Content,Order")] Step step)
        {
            if (tutorialId != step.Id)
                return NotFound();

            try
            {
                _context.Update(step);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StepExists(step.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction("Details", "Tutorial", new { tutorialId = step.TutorialId });
        }

        // GET: Step/Delete/id
        public async Task<IActionResult> Delete(Guid? tutorialId)
        {
            if (tutorialId == null)
                return NotFound();

            var step = await _context.Steps.FirstOrDefaultAsync(m => m.Id == tutorialId);

            if (step == null)
                return NotFound();

            return View(step);
        }

        // POST: Step/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid tutorialId)
        {
            var step = await _context.Steps.FindAsync(tutorialId);
            _context.Steps.Remove(step);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Tutorial", new { tutorialId = step.TutorialId });
        }

        private bool StepExists(Guid tutorialId)
        {
            return _context.Steps.Any(e => e.Id == tutorialId);
        }
    }
}