using Microsoft.AspNetCore.Mvc;
using MelodyCircle.Data;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    public class CollaborationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollaborationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Collaboration
        public async Task<IActionResult> Index()
        {
            var collaborations = await _context.Collaborations.ToListAsync();
            return View(collaborations);
        }

        // GET: CollaborationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CollaborationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CollaborationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CollaborationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CollaborationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CollaborationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
