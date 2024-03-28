using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    [Authorize]
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
            if (User.IsInRole("Teacher") || User.IsInRole("Mod") || User.IsInRole("Admin"))
                return RedirectToAction("EditMode");

            return RedirectToAction("ViewMode");
        }

        // GET: Tutorial/EditMode
        public async Task<IActionResult> EditMode()
        {
            var userId = _userManager.GetUserId(User);
            var tutoriaisCriados = await _context.Tutorials
                .Where(t => t.Creator == User.Identity.Name)
                .ToListAsync();

            return View("EditMode", tutoriaisCriados);
        }

        // GET: Tutorial/ViewMode
        public async Task<IActionResult> ViewMode()
        {
            var userId = _userManager.GetUserId(User);
            var tutoriaisInscritos = await _context.SubscribeTutorials
                .Where(s => s.User.Id.ToString() == userId)
                .Select(s => s.Tutorial)
                .ToListAsync();

            return View("ViewMode", tutoriaisInscritos);
        }

        // GET: Tutorial/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Tutorial tutorial, IFormFile photo)
        {
            if (string.IsNullOrWhiteSpace(tutorial.Title) || string.IsNullOrWhiteSpace(tutorial.Description))
            {
                ModelState.AddModelError(nameof(tutorial.Title), "Campo obrigatório");
                ModelState.AddModelError(nameof(tutorial.Description), "Campo obrigatório");
            }

            else
            {
                tutorial.Id = Guid.NewGuid();

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                    tutorial.Creator = user.UserName;


                if (photo != null || photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photo.CopyToAsync(memoryStream);
                        tutorial.Photo = memoryStream.ToArray();
                        tutorial.PhotoContentType = photo.ContentType;
                    }
                }

                _context.Add(tutorial);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(tutorial);
        }

        // GET: Tutorial/Edit/id
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var tutorial = await _context.Tutorials.FindAsync(id);

            if (tutorial == null)
                return NotFound();

            return View(tutorial);
        }

        // POST: Tutorial/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Creator")] Tutorial tutorial, IFormFile photo)
        {
            if (id != tutorial.Id)
                return NotFound();

            if (string.IsNullOrWhiteSpace(tutorial.Title) || string.IsNullOrWhiteSpace(tutorial.Description))
            {
                ModelState.AddModelError(nameof(tutorial.Title), "Campo obrigatório");
                ModelState.AddModelError(nameof(tutorial.Description), "Campo obrigatório");
            }

            else 
            {
                var existingTutorial = await _context.Tutorials.FindAsync(id);

                if (photo != null && photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photo.CopyToAsync(memoryStream);
                        existingTutorial.Photo = memoryStream.ToArray();
                        existingTutorial.PhotoContentType = photo.ContentType;
                    }
                }

                existingTutorial.Title = existingTutorial.Title;
                existingTutorial.Description = existingTutorial.Description;

                _context.Update(existingTutorial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tutorial);
        }

        // GET: Tutorial/Delete/id
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var tutorial = await _context.Tutorials.FindAsync(id);

            if (tutorial == null)
                return NotFound();

            return View(tutorial);
        }

        // POST: Tutorial/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);

            if (tutorial == null)
                return NotFound();

            _context.Tutorials.Remove(tutorial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tutorial/AddStep/id
        public IActionResult AddStep(Guid id)
        {
            return RedirectToAction("Index", "Step", new { tutorialId = id });
        }

        public async Task<IActionResult> RateTutorial(Guid id, int rating)
        {
            if (rating < 0 || rating > 10)
            {
                return BadRequest("Rating value must be between 0 and 10");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            var tutorialToRate = await _context.Tutorials.FindAsync(id);

            if (tutorialToRate == null)
            {
                return NotFound("User to rate not found");
            }

            if (tutorialToRate.Ratings == null)
            {
                tutorialToRate.Ratings = new List<TutorialRating>();
            }

            var existingRatings = _context.TutorialRating.AsEnumerable().Where(r => r.UserName.Equals(currentUser.UserName));
            var existingRating = existingRatings.FirstOrDefault(u => u.TutorialId.Equals(id));

            if (existingRating != null)
            {
                existingRating.Value = rating;
            }
            else
            {
                tutorialToRate.Ratings.Add(new TutorialRating { UserName = currentUser.UserName, TutorialId = id, Value = rating });
            }

            // Update the user in the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool TutorialExists(Guid id)
        {
            return _context.Tutorials.Any(e => e.Id == id);
        }
    }
}