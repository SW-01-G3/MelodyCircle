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
                .OrderBy(t => t.Id)
                .Take(2)
                .Select(tutorial => new
                {
                    Tutorial = tutorial,
                    StepCount = _context.Steps.Count(step => step.TutorialId == tutorial.Id)
                })
                .ToListAsync();

            var tutoriaisCriadosComContagem = tutoriaisCriados
                .Select(t => { t.Tutorial.StepCount = t.StepCount;
                    return t.Tutorial;
                })
                .ToList();

            return View("EditMode", tutoriaisCriadosComContagem);
        }

        public async Task<IActionResult> EditModePartials(Guid lastId)
        {
            var userId = _userManager.GetUserId(User); 

            var tutoriaisCriados = await _context.Tutorials
                .Where(t => t.Creator == User.Identity.Name && (lastId == null || t.Id > lastId))
                .OrderBy(t => t.Id)
                .Take(2)
                .Select(tutorial => new
                {
                    Tutorial = tutorial,
                    StepCount = _context.Steps.Count(step => step.TutorialId == tutorial.Id)
                }).ToListAsync();

            var tutoriaisCriadosComContagem = tutoriaisCriados
                .Select(t => {
                    t.Tutorial.StepCount = t.StepCount;
                    return t.Tutorial;
                }).ToList();

            return PartialView("_EditModePartial", tutoriaisCriadosComContagem);
        }

        // GET: Tutorial/ViewMode
        public async Task<IActionResult> ViewMode()
        {
            var userId = _userManager.GetUserId(User);

            var tutoriaisInscritos = await _context.SubscribeTutorials
                .Where(elem => elem.User.Id.ToString() == userId)
                .OrderBy(t => t.Id)
                .Take(2)
                .Select(tutorial => new { Tutorial = tutorial.Tutorial, StepCount = _context.Steps.Count(elem => elem.TutorialId == tutorial.TutorialId) })
                .ToListAsync();

            var tutoriaisInscritosComContagem = tutoriaisInscritos.Select(elem => { elem.Tutorial.StepCount = elem.StepCount; return elem.Tutorial; }).ToList();

            return View("ViewMode", tutoriaisInscritosComContagem);
        }

        public async Task<IActionResult> ViewModePartials(Guid lastId)
        {
            var userId = _userManager.GetUserId(User); 

            var tutoriaisInscritos = await _context.SubscribeTutorials
                .Where(elem => elem.User.Id.ToString() == userId && (elem.Tutorial.Id > lastId))
                .OrderBy(t => t.Id)
                .Take(2)
                .Select(tutorial => new { Tutorial = tutorial.Tutorial, StepCount = _context.Steps.Count(elem => elem.TutorialId == tutorial.TutorialId) })
                .ToListAsync();

            var tutoriaisInscritosComContagem = tutoriaisInscritos.Select(elem => { elem.Tutorial.StepCount = elem.StepCount; return elem.Tutorial; }).ToList();

            return PartialView("_ViewModePartial", tutoriaisInscritosComContagem);
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
            var allowedExtensions = new List<string> { ".jpeg", ".jpg", ".png" };

            bool hasValidationError = false;

            if (string.IsNullOrWhiteSpace(tutorial.Title))
            {
                ModelState.AddModelError(nameof(tutorial.Title), "O título é obrigatório");
                hasValidationError = true;
            }

            if (string.IsNullOrWhiteSpace(tutorial.Description))
            {
                ModelState.AddModelError(nameof(tutorial.Description), "A descrição é obrigatória");
                hasValidationError = true;
            }

            if (photo == null)
            {
                ModelState.AddModelError(nameof(tutorial.Photo), "A fotografia é obrigatória");
                hasValidationError = true;
            }

            else
            {
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(tutorial.Photo), "Só são suportados ficheiros .jpeg, .jpg, .png");
                    hasValidationError = true;
                }
            }

            if (hasValidationError)
                return View(tutorial);

            tutorial.Id = Guid.NewGuid();

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                tutorial.Creator = user.UserName;
            else
                return Unauthorized("Utilizador não encontrado");

            using (var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);
                tutorial.Photo = memoryStream.ToArray();
                tutorial.PhotoContentType = photo.ContentType;
            }

            tutorial.CreationDate = DateTime.Now;

            _context.Add(tutorial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
            var allowedExtensions = new List<string> { ".jpeg", ".jpg", ".png" };

            bool hasValidationError = false;

            if (id != tutorial.Id)
                return NotFound();

            if (string.IsNullOrWhiteSpace(tutorial.Title))
            {
                ModelState.AddModelError(nameof(tutorial.Title), "O título é obrigatório");
                hasValidationError = true;
            }

            if (string.IsNullOrWhiteSpace(tutorial.Description))
            {
                ModelState.AddModelError(nameof(tutorial.Description), "A descrição é obrigatória");
                hasValidationError = true;
            }

            if (hasValidationError)
                return View(tutorial);

            var existingTutorial = await _context.Tutorials.FindAsync(id);

            if (existingTutorial == null)
                return NotFound();

            existingTutorial.Title = tutorial.Title;
            existingTutorial.Description = tutorial.Description;

            if (photo != null)
            {
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(tutorial.Photo), "Só são suportados ficheiros .jpeg, .jpg, .png");
                    return View(tutorial);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    existingTutorial.Photo = memoryStream.ToArray();
                    existingTutorial.PhotoContentType = photo.ContentType;
                }
            }

            _context.Update(existingTutorial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
                return BadRequest("Rating value must be between 0 and 10");

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return NotFound("User not found");

            var tutorialToRate = await _context.Tutorials.FindAsync(id);

            if (tutorialToRate == null)
                return NotFound("Tutorial to rate not found");

            if (tutorialToRate.Ratings == null)
                tutorialToRate.Ratings = new List<TutorialRating>();

            var existingRatings = _context.TutorialRating.AsEnumerable().Where(r => r.UserName.Equals(currentUser.UserName));
            var existingRating = existingRatings.FirstOrDefault(u => u.TutorialId.Equals(id));

            if (existingRating != null)
                existingRating.Value = rating;

            else
                tutorialToRate.Ratings.Add(new TutorialRating { UserName = currentUser.UserName, TutorialId = id, Value = rating });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool TutorialExists(Guid id)
        {
            return _context.Tutorials.Any(e => e.Id == id);
        }
    }
}