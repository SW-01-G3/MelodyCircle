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
    }
}

