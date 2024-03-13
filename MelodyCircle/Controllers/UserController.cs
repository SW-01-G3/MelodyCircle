using MelodyCircle.Data;
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

        /// <summary>
        /// Return the index of all users list.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            return View(await users.ToListAsync());
        }

        /// <summary>
        /// Action for checking user profile by param (Username).
        /// Displays the profile of the user.
        /// Finds user by username, checks if exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null || _userManager.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new ProfileViewModel
            {
                User = user,
                Roles = roles,
            };

            await _context.Entry(user)
              .Collection(u => u.Connections)
              .LoadAsync();

            return View(viewModel);
        }

        /// <summary>
        /// Action for adding a new connection of a user.
        /// Checks for user existence, connection existence.
        /// Redirects to profile of added user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Action for removing an existing connection of a user.
        /// Checks for user existence, connection existence.
        /// Redirects to profile of removed user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Lists all user's connections.
        /// Checks for user existence and list's connections.
        /// Displays a list of connections.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ListConnections(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id == null || _userManager.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _context.Entry(user).Collection(u => u.Connections).LoadAsync();

            return View(user.Connections);
        }

        /// <summary>
        /// Insert a new profile picture on the editing user's profile.
        /// Checks for picture's format, transforms it into an array of bytes.
        /// Redirects to profile of logged user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profilePicture"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Action for rating a user's profile.
        /// Checks existence of user and if rating is between 0 and 10 (included).
        /// A User cannot rate multiple times, if rating already exists, then it updates said rating.
        /// Redirects to profile of rated user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        public async Task<IActionResult> RateUser(string id, int rating)
        {
            if (rating < 0 || rating > 10)
            {
                return BadRequest("Rating value must be between 0 and 10");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            if (currentUser.UserName == id)
            {
                return BadRequest("Cannot rate yourself");
            }

            var userToRate = await _userManager.FindByNameAsync(id);

            if (userToRate == null)
            {
                return NotFound("User to rate not found");
            }

            if (userToRate.Ratings == null)
            {
                userToRate.Ratings = new List<UserRating>();
            }

            var existingRatings = _context.UserRating.AsEnumerable().Where(r => r.UserName.Equals(currentUser.UserName));
            var existingRating = existingRatings.Where(u => u.RatedUserName.Equals(userToRate.UserName)).FirstOrDefault();

            if (existingRating != null )
            {
                existingRating.Value = rating;
            }
            else
            {
                userToRate.Ratings.Add(new UserRating { UserName = currentUser.UserName, RatedUserName = userToRate.UserName, Value = rating });
            }

            // Update the user in the database
            await _userManager.UpdateAsync(userToRate);

            return RedirectToAction("Profile", new { id });
        }

        /// <summary>
        /// Listing of all rating by user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewRatings(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            Console.WriteLine(id.ToString());

            if (user == null)
            {
                return NotFound("User not found");
            }

            return View(_context.UserRating.Where(u => u.UserId.ToString().Equals(id.ToString())));
        }
    }
}
