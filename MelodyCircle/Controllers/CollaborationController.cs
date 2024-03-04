using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MelodyCircle.Controllers
{
    public class CollaborationController : Controller
    {
        // GET: CollaborationController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CollaborationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
