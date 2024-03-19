using System;
using System.Linq;
using System.Threading.Tasks;
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
            var inscricao = new SubscribeTutorial { User = await _userManager.GetUserAsync(User), TutorialId = tutorialId };
            _context.Add(inscricao);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Tutorial");
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
                return NotFound();

            _context.SubscribeTutorials.Remove(inscricao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyTutorials));
        }
    }
}