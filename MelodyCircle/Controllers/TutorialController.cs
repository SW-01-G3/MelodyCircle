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

        // GET: Tutorial/Index
        //public async Task<IActionResult> Index()
        //{
        //    var tutorials = await _context.Tutorials
        //        .Select(t => new Tutorial
        //        {
        //            Id = t.Id,
        //            Title = t.Title,
        //            Description = t.Description,
        //            Creator = t.Creator,
        //            Photo = t.Photo,
        //            PhotoFileName = t.PhotoFileName,
        //            PhotoContentType = t.PhotoContentType,
        //            StepCount = t.Steps.Count,
        //            SubscribersCount = t.SubscribersCount
        //        })
        //        .ToListAsync();

        //    return View(tutorials);
        //}

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Verifica se o usuário é um Teacher, Mod ou Admin
            var isTeacherOrHigher = User.IsInRole("Teacher") || User.IsInRole("Mod") || User.IsInRole("Admin");

            if (isTeacherOrHigher)
            {
                // Se o usuário for Teacher ou superior, mostra apenas os tutoriais que ele criou
                var tutoriais = await _context.Tutorials
                    .Where(t => t.Creator == User.Identity.Name)
                    .ToListAsync();

                return View(tutoriais);
            }
            else
            {
                // Caso contrário, mostra apenas os tutoriais que o usuário se inscreveu
                var tutoriaisInscritos = await _context.SubscribeTutorials
                    .Where(s => s.User.Id.ToString() == userId)
                    .Select(s => s.Tutorial)
                    .ToListAsync();

                return View(tutoriaisInscritos);
            }
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
                ModelState.AddModelError(nameof(tutorial.Title), "Campo obrigatório");

            if (string.IsNullOrWhiteSpace(tutorial.Description) || string.IsNullOrWhiteSpace(tutorial.Title))
                ModelState.AddModelError(nameof(tutorial.Description), "Campo obrigatório");

            else
            {
                if (photo != null && photo.Length > 0 && photo.ContentType == "image/jpeg")
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photo.CopyToAsync(memoryStream);
                        tutorial.Photo = memoryStream.ToArray();
                        tutorial.PhotoFileName = photo.FileName;
                        tutorial.PhotoContentType = photo.ContentType;
                    }
                }
                else
                    ModelState.AddModelError(nameof(tutorial.Photo), "Only JPEG files are allowed");

                tutorial.Id = Guid.NewGuid();

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                    tutorial.Creator = user.UserName;

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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Creator")] Tutorial tutorial)
        {
            if (id != tutorial.Id)
                return NotFound();

            if (string.IsNullOrWhiteSpace(tutorial.Title) || string.IsNullOrWhiteSpace(tutorial.Description))
                ModelState.AddModelError(nameof(tutorial.Title), "Campo obrigatório");

            if (string.IsNullOrWhiteSpace(tutorial.Description) || string.IsNullOrWhiteSpace(tutorial.Title))
                ModelState.AddModelError(nameof(tutorial.Description), "Campo obrigatório");

            else
            {
                _context.Update(tutorial);
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

        private bool TutorialExists(Guid id)
        {
            return _context.Tutorials.Any(e => e.Id == id);
        }
    }
}