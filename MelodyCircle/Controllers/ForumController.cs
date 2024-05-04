using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Data;
using MelodyCircle.Models;
using System.Security.Claims;

namespace MelodyCircle.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumController(ApplicationDbContext context)
        {
            _context = context;
        }

        //// GET: Forum
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.ForumPost.OrderBy(elem => elem.Id)
        //        .Take(1).ToListAsync());
        //}

        /* Eduardo Andrade */
        public async Task<IActionResult> Posts(Guid lastId)
        {
            var forumPosts = await _context.ForumPost.Where(elem => (elem.Id > lastId))
                .OrderBy(elem => elem.Id)
                .Take(3)
                .ToListAsync();

            return PartialView("_PostsPartial", forumPosts);
        }

        /* Rodrigo Nogueira */
        // GET: Forum/Create
        public IActionResult Create()
        {
            return View();
        }

        /* Rodrigo Nogueira */
        // POST: Forum/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] ForumPost forumPost)
        {
            bool hasValidationError = false;

            if (string.IsNullOrEmpty(forumPost.Title))
            {
                ModelState.AddModelError(nameof(forumPost.Title), "O título é obrigatório");
                hasValidationError = true;
            }

            if (string.IsNullOrEmpty(forumPost.Content))
            {
                ModelState.AddModelError(nameof(forumPost.Content), "O conteúdo é obrigatório");
                hasValidationError = true;
            }

            if (hasValidationError)
                return View(forumPost);

            if (ModelState.IsValid)
            {
                forumPost.Id = Guid.NewGuid();
                forumPost.IsClosed = false;

                _context.Add(forumPost);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        /* Rodrigo Nogueira */
        // GET: Forum/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPost.FindAsync(id);
            if (forumPost == null)
            {
                return NotFound();
            }
            return View(forumPost);
        }

        /* Rodrigo Nogueira */
        // POST: Forum/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Content")] ForumPost forumPost)
        {
            bool hasValidationError = false;

            if (string.IsNullOrEmpty(forumPost.Title))
            {
                ModelState.AddModelError(nameof(forumPost.Title), "O título é obrigatório");
                hasValidationError = true;
            }

            if (string.IsNullOrEmpty(forumPost.Content))
            {
                ModelState.AddModelError(nameof(forumPost.Content), "O conteúdo é obrigatório");
                hasValidationError = true;
            }

            if (hasValidationError)
                return View(forumPost);

            if (id != forumPost.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(forumPost);
                await _context.SaveChangesAsync();
            }

            return View(forumPost);
        }

        /* Rodrigo Nogueira */
        // GET: Forum/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var forumPost = await _context.ForumPost
                .FirstOrDefaultAsync(m => m.Id == id);

            if (forumPost == null)
                return NotFound();

            return View(forumPost);
        }

        /* Rodrigo Nogueira */
        // POST: Forum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forumPost = await _context.ForumPost.FindAsync(id);

            if (forumPost != null)
                _context.ForumPost.Remove(forumPost);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /* Rodrigo Nogueira */
        // GET: Forum/Comments/5
        public async Task<IActionResult> Comments(Guid? id)
        {
            if (id == null)
                return NotFound();

            var forumPost = await _context.ForumPost
                .Include(fp => fp.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (forumPost == null)
                return NotFound();

            var isClosed = forumPost.IsClosed;

            return View(forumPost);
        }

        /* Rodrigo Nogueira */
        // POST: Forum/Comment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(Guid id, [Bind("Content")] Comment comment)
        {
            var forumPost = await _context.ForumPost.FindAsync(id);

            if (forumPost == null || forumPost.IsClosed)
                return RedirectToAction(nameof(Comments), new { id = id });

            if (string.IsNullOrEmpty(comment.Content))
            {
                ModelState.AddModelError(nameof(comment.Content), "Comment can not be empty");
                TempData["ErrorMessage"] = "Comment can not be empty";
            }

            else
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedAt = DateTime.Now;
                comment.ForumPostId = id;

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                comment.UserId = userId;

                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Comments), new { id = id });
        }

        /* Rodrigo Nogueira */
        // POST: Forum/ClosePost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClosePost(Guid id)
        {
            var forumPost = await _context.ForumPost.FindAsync(id);

            if (forumPost == null)
                return NotFound();

            await _context.SaveChangesAsync();

            return View("ClosePost", forumPost);
        }

        /* Rodrigo Nogueira */
        // POST: Forum/ConfirmClosePost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmClosePost(Guid id)
        {
            var forumPost = await _context.ForumPost.FindAsync(id);

            if (forumPost == null)
                return NotFound();

            forumPost.IsClosed = true;

            await _context.SaveChangesAsync();

            return View("ClosePostConfirmation", forumPost);
        }

        /* Rodrigo Nogueira */
        private bool ForumPostExists(Guid id)
        {
            return _context.ForumPost.Any(e => e.Id == id);
        }
    }
}