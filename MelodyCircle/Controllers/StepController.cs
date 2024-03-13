using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    public class StepController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StepController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Guid? tutorialId)
        {
            if (tutorialId == null)
                return NotFound();

            var steps = await _context.Steps
                .Include(s => s.Tutorial)
                .Where(s => s.TutorialId == tutorialId)
                .ToListAsync();

            var creator = await _context.Tutorials
                .Where(t => t.Id == tutorialId)
                .Select(t => t.Creator)
                .FirstOrDefaultAsync();

            ViewBag.TutorialId = tutorialId;
            ViewBag.Creator = creator;

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

        private bool StepExists(Guid tutorialId)
        {
            return _context.Steps.Any(e => e.Id == tutorialId);
        }
    }
}