﻿using MelodyCircle.Data;
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
        public async Task<IActionResult> SearchTutorial(Search search)
        {
            var tutorials = await _context.Tutorials
                .Where(t => t.Title.Contains(search.SearchTerm))
                .ToListAsync();

            var limitedTutorials = tutorials.Take(10).ToList();

            return View("TutorialSearchResult", limitedTutorials);
        }

        public IActionResult SearchUser(Search search)
        {
            var users = _context.Users
                .Where(u => u.UserName.Contains(search.SearchTerm))
                .ToList();

            var limitedUsers = users.Take(10).ToList();

            return View("UserSearchResult", limitedUsers);
        }
    }
}