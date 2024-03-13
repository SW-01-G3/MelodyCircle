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

        public StepController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Step
        public async Task<IActionResult> Index(Guid? tutorialId)
        {
            if (tutorialId == null)
                return NotFound();

            var steps = await _context.Steps
                .Include(s => s.Tutorial)
                .Where(s => s.TutorialId == tutorialId)
                .ToListAsync();

            ViewBag.TutorialId = tutorialId;

            return View(steps);
        }

        // GET: Step/Create
        public IActionResult Create(Guid? tutorialId)
        {
            ViewBag.TutorialId = tutorialId;

            if (tutorialId == null)
                return NotFound();

            return View();
        }

        // POST: Step/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TutorialId,Title,Content,Order")] Step step)
        {
            if (string.IsNullOrEmpty(step.Title) || string.IsNullOrEmpty(step.Content))
                ModelState.AddModelError(nameof(step.Title), "O título é obrigatório");

            if (string.IsNullOrEmpty(step.Content) || string.IsNullOrEmpty(step.Title))
                ModelState.AddModelError(nameof(step.Title), "O conteúdo é obrigatório");

            _context.Add(step);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { tutorialId = step.TutorialId });
        }

        // GET: Step/Delete/tutorialId
        public async Task<IActionResult> Delete(Guid? tutorialId)
        {
            if (tutorialId == null)
                return NotFound();

            var step = await _context.Steps.FirstOrDefaultAsync(m => m.TutorialId == tutorialId);

            if (step == null)
                return NotFound();

            return View(step);
        }

        // POST: Step/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid stepId)
        {
            var step = await _context.Steps.FindAsync(stepId);

            if (step == null)
                return NotFound();

            _context.Steps.Remove(step);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { tutorialId = step.TutorialId });
        }

        private bool StepExists(Guid tutorialId)
        {
            return _context.Steps.Any(e => e.Id == tutorialId);
        }
    }
}