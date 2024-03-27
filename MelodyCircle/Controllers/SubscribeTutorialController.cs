using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Data;
using MelodyCircle.Models;

namespace MelodyCircle.Controllers
{
    public class SubscribeTutorialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public SubscribeTutorialController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SubscribeTutorial/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Subscribe(Guid tutorialId)
        {
            var userId = _userManager.GetUserId(User);

            var alreadySubscribed = await _context.SubscribeTutorials
                .AnyAsync(s => s.User.Id.ToString() == userId && s.TutorialId == tutorialId);

            if (alreadySubscribed)
            {
                TempData["Message"] = "Já está inscrito neste tutorial";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            var inscricao = new SubscribeTutorial { User = user, TutorialId = tutorialId };
            _context.Add(inscricao);
            await _context.SaveChangesAsync();

            var tutorial = await _context.Tutorials.FindAsync(tutorialId);
            if (tutorial != null)
            {
                tutorial.SubscribersCount = (tutorial.SubscribersCount ?? 0) + 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Tutorial");
        }

        [HttpGet]
        public IActionResult SubscribeConfirmation(Guid tutorialId)
        {
            ViewData["TutorialId"] = tutorialId;
            return View();
        }

        // GET: SubscribeTutorial/MyTutorials
        [Authorize]
        public async Task<IActionResult> MyTutorials()
        {
            var userId = _userManager.GetUserId(User);

            var tutoriaisInscritos = await _context.SubscribeTutorials
                .Where(s => s.User.Id == Guid.Parse(userId))
                .Include(s => s.Tutorial)
                .ToListAsync();

            return View(tutoriaisInscritos);
        }

        // GET: SubscribeTutorial/RemoveSubscription/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveSubscription(Guid id)
        {
            var inscricao = await _context.SubscribeTutorials.FindAsync(id);

            if (inscricao == null)
            {
                return NotFound();
            }

            var tutorial = await _context.Tutorials.FindAsync(inscricao.TutorialId);
            tutorial.SubscribersCount--; // Decrementa o contador de inscritos

            _context.SubscribeTutorials.Remove(inscricao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyTutorials));
        }
    }
}