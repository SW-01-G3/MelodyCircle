﻿using MelodyCircle.Data;
using MelodyCircle.Models;
using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using SendGrid.Helpers.Mail;
using System.Numerics;

namespace MelodyCircle.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            return View(await users.ToListAsync());
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> Profile(string? id)
        {
            if (id == null || _userManager.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.UserName == id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new ProfileViewModel
            {
                User = user,
                Roles = roles,
                Context = _context,
                UserManager = _userManager
            };

            await _context.Entry(user)
              .Collection(u => u.Connections)
              .LoadAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddConnection(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userToAdd = await _userManager.FindByNameAsync(id);


            if (currentUser == null || userToAdd == null)
            {
                return NotFound();
            }

            if (currentUser.UserName == userToAdd.UserName)
            {
                return BadRequest("Cannot add yourself as a connection.");
            }

            if (currentUser.Connections == null)
            {
                currentUser.Connections = new List<User>();
            }

            if (currentUser.Connections.Contains(userToAdd))
            {
                return BadRequest("User is already in your connections list.");
            }


            await _context.Entry(currentUser).Collection(u => u.Connections).LoadAsync();
            currentUser.Connections.Add(userToAdd);
            await _userManager.UpdateAsync(currentUser);


            return RedirectToAction("Profile", new { id = userToAdd.UserName });
        }

        public async Task<IActionResult> RemoveConnection(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var connectionToRemove = await _userManager.FindByNameAsync(id);


            if (currentUser != null && connectionToRemove != null)
            {
                if (currentUser.Connections.Contains(connectionToRemove))
                {
                    await _context.Entry(currentUser).Collection(u => u.Connections).LoadAsync();

                    currentUser.Connections.Remove(connectionToRemove);
                    await _userManager.UpdateAsync(currentUser);
                }
            }

            return RedirectToAction("Profile", new { id = connectionToRemove.UserName });
        }

        public async Task<IActionResult> ListConnections(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id == null || _userManager.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.UserName == id);

            if (user == null)
            {
                return NotFound();
            }

            await _context.Entry(user).Collection(u => u.Connections).LoadAsync();

            return View(user.Connections);
        }

        [HttpPost]
        public async Task<IActionResult> PutProfilePicture(string id, IFormFile profilePicture)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return BadRequest("User doens't exist.");
            }

            if (profilePicture == null || profilePicture.Length == 0)
            {
                return BadRequest("No image selected.");
            }

            if (profilePicture.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("profilePicture", "Only JPEG files are allowed.");
                return BadRequest("Not the right format."); 
            }
            
            if (user == null)
            {
                return NotFound();
            }
            byte[] profilePictureBytes;
            using (var ms = new MemoryStream())
            {
                await profilePicture.CopyToAsync(ms);
                profilePictureBytes = ms.ToArray();
            }

            user.ProfilePicture = profilePictureBytes;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500);
            }

            return RedirectToAction("Profile", new { id });
        }
    }
}
