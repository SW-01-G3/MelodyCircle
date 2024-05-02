using MelodyCircle.Data;
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
		private readonly ApplicationDbContext _context;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
			_context = context;
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

			var random = new Random();
			var currentDate = DateTime.Now.Date;

			for (int i = 1; i <= numberOfUsers; i++)
			{
				var randomDays = random.Next(1, 181); 
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
                    Ratings = new List<UserRating>(),   
					CreationDate = currentDate.AddDays(-randomDays)
				};

				var result = await _userManager.CreateAsync(user, user.Password);

				if (!result.Succeeded)
				{
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateMultipleTutorials(int numberOfTutorials)
		{
			if (numberOfTutorials <= 0)
			{
				ModelState.AddModelError("", "Número inválido de tutoriais.");
				return View();
			}

			var random = new Random();
			var currentDate = DateTime.Now.Date;
			var prefix = "SpecialTutorial";

			for (int i = 1; i <= numberOfTutorials; i++)
			{
				var randomDays = random.Next(1, 181); 
				var tutorial = new Tutorial
				{
					Title = $"{prefix}{i}",
					Description = $"Descrição do Tutorial {i}",
					Creator = $"Creator{i}",
					CreationDate = currentDate.AddDays(-randomDays) 
				};

				_context.Tutorials.Add(tutorial);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAddedTutorials()
		{
			// lógica para excluir todos os tutoriais adicionados
			var tutorials = await _context.Tutorials.Where(t => t.Title.StartsWith("SpecialTutorial")).ToListAsync();

			_context.Tutorials.RemoveRange(tutorials);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateMultipleCollaborations(int numberOfCollaborations)
		{
			if (numberOfCollaborations <= 0)
			{
				ModelState.AddModelError("", "Número inválido de colaborações.");
				return View();
			}

			var random = new Random();
			var currentDate = DateTime.Now.Date;
			var prefix = "SpecialCollaboration"; 

			for (int i = 1; i <= numberOfCollaborations; i++)
			{
				var randomDays = random.Next(1, 181); 
				var collaboration = new Collaboration
				{
					Title = $"{prefix}{i}",
					Description = $"Descrição da Colaboração {i}",
					CreatorId = $"Creator{i}",
					CreatedDate = currentDate.AddDays(-randomDays), 
					AccessMode = AccessMode.Public,
					MaxUsers = random.Next(1, 11), 
					IsFinished = false 
				};

				_context.Collaborations.Add(collaboration);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAddedCollaborations()
		{

			var collaborations = await _context.Collaborations.Where(c => c.Title.StartsWith("SpecialCollaboration")).ToListAsync();

			_context.Collaborations.RemoveRange(collaborations);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultipleSteps(int numberOfSteps)
        {
            if (numberOfSteps <= 0)
            {
                ModelState.AddModelError("", "Número inválido de passos.");
                return View();
            }

            var random = new Random();
            var currentDate = DateTime.Now.Date;
            var prefix = "SpecialStep";

            var tutorial = new Tutorial
            {
                Title = "SpecialTutorial",
                Description = "Descrição do Tutorial Especial",
                Creator = "admin1", 
                CreationDate = currentDate,
                Steps = new List<Step>()
            };

            for (int i = 1; i <= numberOfSteps; i++)
            {
                var randomDays = random.Next(1, 181); 
                var step = new Step
                {
                    Tutorial = tutorial,
                    Title = $"{prefix}{i}",
                    Content = $"Conteudo do Step {i}",
                    Order = 0,
                    CreationDate = currentDate.AddDays(-randomDays),
                };

 
                tutorial.Steps.Add(step);
            }

            _context.Tutorials.Add(tutorial);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddedSteps()
        {
            var tutorials = await _context.Tutorials.
                Where(t => t.Title == "SpecialTutorial").ToListAsync();

            _context.Tutorials.RemoveRange(tutorials);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

