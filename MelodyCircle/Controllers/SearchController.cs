using MelodyCircle.Data;
using MelodyCircle.Models;
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

        public IActionResult Index()
        {
            return View();
        }

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
            return View("Index");
        }
    }
}
