using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MelodyCircle.Controllers
{
    public class StatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public StatController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult UserCreationStats()
        {
            var userCreationData = _context.Users
                .Where(u => u.CreationDate.Year == DateTime.Now.Year)
                .GroupBy(u => new { u.CreationDate.Month, u.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            var monthsAndYears = userCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.UserCounts = userCreationData.Select(d => d.Count);

            return View();
        }

        public IActionResult TutorialCreationStats()
        {
            var tutorialCreationData = _context.Tutorials
                .Where(t => t.CreationDate.Year == DateTime.Now.Year)
                .GroupBy(t => new { t.CreationDate.Month, t.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            var monthsAndYears = tutorialCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.TutorialCounts = tutorialCreationData.Select(d => d.Count);

            return View();
        }

        public IActionResult CollaborationCreationStats()
        {
            var collaborationCreationData = _context.Collaborations
                .Where(c => c.CreatedDate.Year == DateTime.Now.Year)
                .GroupBy(c => new { c.CreatedDate.Month, c.CreatedDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            var monthsAndYears = collaborationCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.CollaborationCounts = collaborationCreationData.Select(d => d.Count);

            return View();
        }

        public IActionResult UserTutorialStats(string userName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                return RedirectToAction("Index"); 
            }

            var tutorialStats = _context.Tutorials
                .Where(t => t.Creator == userName && t.CreationDate.Year == DateTime.Now.Year)
                .GroupBy(t => new { t.CreationDate.Month, t.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            var monthsAndYears = tutorialStats.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {DateTime.Now.Year}");

            ViewBag.UserName = userName;
            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.TutorialCounts = tutorialStats.Select(d => d.Count);

            return View();
        }

        public IActionResult UserCollaborationStats(string userName)
        {
            // var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            var user = _userManager.FindByIdAsync(userName);

            //var user = _userManager.Users.FirstOrDefault(u => u.Id == Guid.Parse( userName));

            Console.WriteLine(userName);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var collaborationStats = _context.Collaborations
                .Where(c => c.CreatorId == userName && c.CreatedDate.Year == DateTime.Now.Year)
                .GroupBy(c => new { c.CreatedDate.Month, c.CreatedDate.Year })
                .Select(g => new { Month = g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            var monthsAndYears = collaborationStats.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {DateTime.Now.Year}");

            ViewBag.UserName = userName;
            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.CollaborationCounts = collaborationStats.Select(d => d.Count);

            return View();
        }
	}
}
