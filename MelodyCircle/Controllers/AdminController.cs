using MelodyCircle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByNameAsync(id);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.Id = id;
            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = _roleManager.Roles.ToList();

            return PartialView("_EditUserRoles", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(string userId, string selectedRole)
        {
            var user = await _userManager.FindByNameAsync(userId);

            if (user == null)
	            return NotFound();

			var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());

            await _userManager.AddToRoleAsync(user, selectedRole);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByNameAsync(id);
            ViewBag.Id = id;
            return PartialView("_DeleteUser", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string userId)
        {
            var user = await _userManager.FindByNameAsync(userId);
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateMultipleUsers(int numberOfUsers)
		{
			if (numberOfUsers <= 0)
			{
				ModelState.AddModelError("", "Número inválido de utilizadores.");
				return View(); 
			}

			for (int i = 1; i <= numberOfUsers; i++)
			{
				var user = new User
				{
					UserName = $"user{i}",
					Email = $"user{i}@melodycircle.pt",
					Name = $"User {i}",
					BirthDate = new DateOnly(2000, 1, 1),
					Password = "Password-123", 
					NormalizedEmail = $"USER{i}@MELODYCIRCLE.PT",
					EmailConfirmed = true,
					Gender = Gender.Other,
					ProfilePicture = new byte[0], 
					Locality = "Portugal",
					Connections = new List<User>(),
					Ratings = new List<UserRating>()
				};

				var result = await _userManager.CreateAsync(user, user.Password);

				if (!result.Succeeded)
				{
					// Handle errors if user creation fails
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View(); 
				}
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAddedUsers()
		{
			var users = await _userManager.Users.ToListAsync();

			foreach (var user in users)
			{
				if (user.UserName.StartsWith("user")) 
				{
					await _userManager.DeleteAsync(user);
				}
			}

			return RedirectToAction("Index");
		}
	}
}

