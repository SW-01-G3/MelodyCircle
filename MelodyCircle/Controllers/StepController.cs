using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    public class StepController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public StepController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var title = await _context.Tutorials
                .Where(t => t.Id == tutorialId)
                .Select(t => t.Title)
                .FirstOrDefaultAsync();

            ViewBag.TutorialId = tutorialId;
            ViewBag.Creator = creator;
            ViewBag.Title = title;

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

            else 
            {
                _context.Add(step);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { tutorialId = step.TutorialId });
            }
            return View(step);
        }

        // GET: Step/Delete/tutorialId
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var step = await _context.Steps.FirstOrDefaultAsync(m => m.Id == id);

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

        // GET: Step/Edit/tutorialId
        public async Task<IActionResult> Edit(Guid? id)
        {
            ViewBag.id = id;

            if (id == null)
                return NotFound();

            var step = await _context.Steps.FirstOrDefaultAsync(m => m.Id == id);

            if (step == null)
                return NotFound();

            return View(step);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid tutorialId, [Bind("Id,TutorialId,Title,Content,Order")] Step step)
        {
            var existingStep = await _context.Steps.FindAsync(step.Id);

            if (existingStep == null)
                return NotFound($"Step with ID {step.Id} not found.");

            if (tutorialId != step.TutorialId)
                return NotFound($"The TutorialId of the step does not match the provided tutorialId.");

            if (string.IsNullOrEmpty(step.Title) || string.IsNullOrEmpty(step.Content))
                ModelState.AddModelError(nameof(step.Title), "O título é obrigatório");

            if (string.IsNullOrEmpty(step.Content) || string.IsNullOrEmpty(step.Title))
                ModelState.AddModelError(nameof(step.Content), "O conteúdo é obrigatório");

            else
            {
                try
                {
                    _context.Entry(existingStep).State = EntityState.Detached;
                    _context.Update(step);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StepExists(step.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { tutorialId = step.TutorialId });
            }
            return View(step);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CompleteStep(Guid tutorialId, Guid stepId)
        {
            var userId = _userManager.GetUserId(User);


            var subscription = await _context.SubscribeTutorials
                .Where(s => s.User.Id.ToString() == userId)
                .Include(st => st.CompletedSteps)
                .FirstOrDefaultAsync(st => st.User.Id.ToString() == userId && st.TutorialId == tutorialId);

            if (subscription == null)
            {
                return NotFound(); 
            }

            var step = await _context.Steps.FindAsync(stepId);
            if (step == null)
            {
                return NotFound(); 
            }

            bool alreadyCompleted = subscription.CompletedSteps.Any(s => s.Id == stepId);

            if (!alreadyCompleted)
            {
                subscription.CompletedSteps.Add(step);
            }
            else
            {
                subscription.CompletedSteps.RemoveAll(s => s.Id == stepId);
            }

            int savedChanges =  await _context.SaveChangesAsync();

            var subscriptionAfterSave = await _context.SubscribeTutorials
                .Where(s => s.User.Id.ToString() == userId)
                .Include(st => st.CompletedSteps)
                .FirstOrDefaultAsync(st => st.User.Id.ToString() == userId && st.TutorialId == tutorialId);

            int completedStepsCount = subscriptionAfterSave.CompletedSteps.Count;
            int totalStepsCount = await _context.Steps.Where(s => s.TutorialId == tutorialId).CountAsync();

            ViewBag.CompletedStepsCount = completedStepsCount;
            ViewBag.TotalStepsCount = totalStepsCount;


            return RedirectToAction("Index", new { tutorialId = step.TutorialId }); // Redirecione para a página de tutoriais do usuário
        }

        private bool StepExists(Guid tutorialId)
        {
            return _context.Steps.Any(e => e.Id == tutorialId);
        }
    }
}