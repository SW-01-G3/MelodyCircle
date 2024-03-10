using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    public class TutorialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TutorialController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var tutorials = await _context.Tutorials.ToListAsync();
            return View(tutorials);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Tutorial tutorial)
        {
            tutorial.Id = Guid.NewGuid();

            var user = await _userManager.GetUserAsync(User);

            tutorial.Creator = user.UserName;
            
             _context.Add(tutorial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}