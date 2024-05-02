using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var sixMonthsAgo = startOfMonth.AddMonths(-6);

            var userCreationData = _context.Users
                .Where(u => u.CreationDate >= sixMonthsAgo && u.CreationDate <= currentDate)
                .GroupBy(u => new { u.CreationDate.Month, u.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var monthsAndYears = userCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.UserCounts = userCreationData.Select(d => d.Count);

            return View();
        }

        public IActionResult TutorialCreationStats()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var sixMonthsAgo = startOfMonth.AddMonths(-6);

            var tutorialCreationData = _context.Tutorials
                .Where(t => t.CreationDate >= sixMonthsAgo && t.CreationDate <= currentDate)
                .GroupBy(t => new { t.CreationDate.Month, t.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var monthsAndYears = tutorialCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.TutorialCounts = tutorialCreationData.Select(d => d.Count);

            return View();
        }

        public IActionResult CollaborationCreationStats()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var sixMonthsAgo = startOfMonth.AddMonths(-6);

            var collaborationCreationData = _context.Collaborations
                .Where(c => c.CreatedDate >= sixMonthsAgo && c.CreatedDate <= currentDate)
                .GroupBy(c => new { c.CreatedDate.Month, c.CreatedDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var monthsAndYears = collaborationCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.CollaborationCounts = collaborationCreationData.Select(d => d.Count);

            return View();
        }

        public IActionResult StepCreationStats()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var sixMonthsAgo = startOfMonth.AddMonths(-6);

            var stepsCreationData = _context.Steps
                .Where(s => s.CreationDate >= sixMonthsAgo && s.CreationDate <= currentDate)
                .GroupBy(s => new { s.CreationDate.Month, s.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var monthsAndYears = stepsCreationData.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {d.Year}");

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.StepsCounts = stepsCreationData.Select(d => d.Count);

            return View();
        }

        public async Task<IActionResult> UserTutorialStats(string userName)
        {
            //var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            var user = await _userManager.FindByNameAsync(userName);

			if (user == null)
            {
                return RedirectToAction("Index");
            }

            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var sixMonthsAgo = startOfMonth.AddMonths(-6);

            var tutorialStats = await _context.Tutorials
                .Where(t => t.Creator == userName && t.CreationDate >= sixMonthsAgo && t.CreationDate <= currentDate)
                .GroupBy(t => new { t.CreationDate.Month, t.CreationDate.Year })
                .Select(g => new { Month = g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToListAsync();

            var monthsAndYears = tutorialStats.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {DateTime.Now.Year}");

            ViewBag.UserName = userName;
            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.TutorialCounts = tutorialStats.Select(d => d.Count);

            return View();
        }

        public async Task<IActionResult> UserCollaborationStats(string userName)
        {
            var user = await _userManager.FindByIdAsync(userName);


            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var sixMonthsAgo = startOfMonth.AddMonths(-6);

            var collaborationStats = await _context.Collaborations
                .Where(c => c.CreatorId == userName && c.CreatedDate >= sixMonthsAgo && c.CreatedDate <= currentDate)
                .GroupBy(c => new { c.CreatedDate.Month, c.CreatedDate.Year })
                .Select(g => new { Month = g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToListAsync();

            var monthsAndYears = collaborationStats.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {DateTime.Now.Year}");


            ViewBag.UserName = userName;

            if (user != null)
            {
                ViewBag.UserName = user.UserName;
            }

            ViewBag.MonthsAndYears = monthsAndYears;
            ViewBag.CollaborationCounts = collaborationStats.Select(d => d.Count);

            return View();
        }

        public async Task<IActionResult> UserStepStats(string userName)
        {
	        var user = await _userManager.FindByNameAsync(userName);

	        if (user == null)
	        {
		        return RedirectToAction("Index");
	        }

	        var currentDate = DateTime.Now;
	        var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
	        var sixMonthsAgo = startOfMonth.AddMonths(-6);

	        var stepStats = await _context.Steps
		        .Where(s => s.Tutorial.Creator == userName && s.CreationDate >= sixMonthsAgo && s.CreationDate <= currentDate)
		        .GroupBy(s => new { s.CreationDate.Month, s.CreationDate.Year })
		        .Select(g => new { Month = g.Key.Month, Count = g.Count() })
		        .OrderBy(g => g.Month)
		        .ToListAsync();

	        var monthsAndYears = stepStats.Select(d => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(d.Month)} {DateTime.Now.Year}");

	        ViewBag.UserName = userName;
	        ViewBag.MonthsAndYears = monthsAndYears;
	        ViewBag.StepCounts = stepStats.Select(d => d.Count);

	        return View();
        }
	}
}
