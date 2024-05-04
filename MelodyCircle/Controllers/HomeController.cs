using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MelodyCircle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /* Rodrigo Nogueira */
        public IActionResult Index()
        {
            var listOfPosts = _context.ForumPost.OrderBy(elem => elem.Id).Take(3).ToList();
            return View(listOfPosts);
        }

        /* Rodrigo Nogueira */
        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        /* Rodrigo Nogueira */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
