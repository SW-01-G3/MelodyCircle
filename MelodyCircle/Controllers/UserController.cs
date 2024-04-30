using MelodyCircle.Data;
using MelodyCircle.Models;
using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="musicUri"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMusicCard(string id, string musicUri)
        {
            var user = await _userManager.GetUserAsync(User);


            if (musicUri == null || !IsValidSpotifyUri(musicUri))
            {
                //ModelState.AddModelError("musicUri", "O URI da música está em um formato inválido.");
                TempData["UriError"] = "The song URI is in an invalid format.";
                return RedirectToAction("Profile", new { id , error = "The song URI is in an invalid format." });
            }

            var regex = new Regex(@"\/track\/(\w+)");
            var match = regex.Match(musicUri);

            if (user.MusicURI.Contains(match.Value))
            {
                //ModelState.AddModelError("musicUri", "Esta música já está na sua lista de favoritos.");
                TempData["UriError"] = "This song is already on your favorites list.";
                return RedirectToAction("Profile", new { id, error = "This song is already on your favorites list." });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Profile", new { id ,error = "The song URI is in an invalid format." });
            }

            //var regex = new Regex(@"\/track\/(\w+)");
            //var match = regex.Match(musicUri);

            user.MusicURI.Add(match.Value.ToString());
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile", new { id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveMusicCard(string uri)
        {
            var user = await _userManager.GetUserAsync(User);
            string id = user.UserName;

            if (user == null)
            {
                return RedirectToAction("Profile", new { id });
            }


            if (!user.MusicURI.Contains(uri))
            {
                TempData["UriError"] = "The song was not found in the favorites list.";
                return RedirectToAction("Profile", new { id, error = "The song was not found in the favorites list." });
            }

            user.MusicURI.Remove(uri);
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile", new { id }); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="newMusicUri"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditMusicCard(string uri, string newMusicUri)
        {
            var user = await _userManager.GetUserAsync(User);
            string id = user.UserName;

            if (user == null)
            {
                return RedirectToAction("Profile", new { id });
            }

            if (newMusicUri == null || !IsValidSpotifyUri(newMusicUri))
            {
                TempData["UriError"] = "The song URI is in an invalid format.";
                return RedirectToAction("Profile", new { id, error = "The song URI is in an invalid format." });
            }

            var regex = new Regex(@"\/track\/(\w+)");
            var match = regex.Match(newMusicUri);

            if (user.MusicURI.Contains(match.Value))
            {
                TempData["UriError"] = "This song is already on your favorites list.";
                return RedirectToAction("Profile", new { id, error = "This song is already on your favorites list." });
            }

            user.MusicURI.Remove(uri);


            user.MusicURI.Add(match.Value.ToString());
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile", new { id }); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private bool IsValidSpotifyUri(string uri)
        {
            if (uri.StartsWith("https://open.spotify.com/track/"))
            {
                int trackIdStartIndex = "https://open.spotify.com/track/".Length;
                return uri.Length > trackIdStartIndex && uri.Length == 73;
            }
            else if (uri.StartsWith("https://open.spotify.com/intl-pt/track/"))
            {
                int trackIdStartIndex = "https://open.spotify.com/intl-pt/track/".Length;
                return uri.Length > trackIdStartIndex && uri.Length == 81;
            }
            else
            {
                // A URI não corresponde aos formatos esperados
                return false;
            }
        }

    }
}