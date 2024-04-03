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

        // GET: Forum
        public async Task<IActionResult> Index()
        {
            return View(await _context.ForumPost.ToListAsync());
        }

        // GET: Forum/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Forum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] ForumPost forumPost)
        {
            if (ModelState.IsValid)
            {
                forumPost.Id = Guid.NewGuid();
                _context.Add(forumPost);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index","Home");
        }

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

        // POST: Forum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Content")] ForumPost forumPost)
        {
            if (id != forumPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumPostExists(forumPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(forumPost);
        }

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

        // GET: Forum/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var forumPost = await _context.ForumPost
                .Include(fp => fp.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (forumPost == null)
                return NotFound();

            return View(forumPost);
        }

        // POST: Forum/Comment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(Guid id, [Bind("Content")] Comment comment)
        {
            if (comment.Content != null)
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedAt = DateTime.Now;
                comment.ForumPostId = id;

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                comment.UserId = userId;

                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }

        private bool ForumPostExists(Guid id)
        {
            return _context.ForumPost.Any(e => e.Id == id);
        }
    }
}
