using MelodyCircle.Data;
using MelodyCircle.Models;
using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        /* Rodrigo Nogueira */
        public async Task<IActionResult> Index()
        {
            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreationDate)
                .Take(5)
                .ToListAsync();

            var recentTutorials = await _context.Tutorials
                .OrderByDescending(t => t.CreationDate)
                .Take(5)
                .ToListAsync();

            var recentCollaborations = await _context.Collaborations
                .OrderByDescending(c => c.CreatedDate)
                .Where(c => c.AccessMode == AccessMode.Public)
                .Take(5)
                .ToListAsync();

            var viewModel = new SearchResultViewModel
            {
                Users = recentUsers,
                Tutorials = recentTutorials,
                Collaborations = recentCollaborations
            };

            return View(viewModel);
        }

        /* Rodrigo Nogueira */
        [HttpPost]
        public async Task<IActionResult> Search(Search search)
        {
            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                if (search.SearchType == SearchType.None)
                {
                    var users = await _context.Users
                         .Where(u => u.UserName.Contains(search.SearchTerm))
                         .ToListAsync();

                    var tutorials = await _context.Tutorials
                        .Where(t => t.Title.Contains(search.SearchTerm))
                        .ToListAsync();

                    var collabs = await _context.Collaborations
                        .Where(t => t.Title.Contains(search.SearchTerm) &&
                        t.AccessMode == AccessMode.Public)
                        .ToListAsync();

                    var viewModel = new SearchResultViewModel
                    {
                        Users = users,
                        Tutorials = tutorials,
                        Collaborations = collabs
                    };

                    return View("SearchResults", viewModel);
                }

                if (search.SearchType == SearchType.User)
                {
                    var users = await _context.Users
                        .Where(u => u.UserName.Contains(search.SearchTerm))
                        .ToListAsync();

                    return View("UserSearchResult", users);
                }

                if (search.SearchType == SearchType.Tutorial)
                {
                    var tutorials = await _context.Tutorials
                        .Where(t => t.Title.Contains(search.SearchTerm))
                        .ToListAsync();

                    return View("TutorialSearchResult", tutorials);
                }

                if (search.SearchType == SearchType.Collaboration)
                {
                    var collabs = await _context.Collaborations
                        .Where(t => t.Title.Contains(search.SearchTerm) &&
                        t.AccessMode == AccessMode.Public)
                        .ToListAsync();

                    return View("CollaborationSearchResult", collabs);
                }
            }
            return RedirectToAction("Index");
        }
    }
}