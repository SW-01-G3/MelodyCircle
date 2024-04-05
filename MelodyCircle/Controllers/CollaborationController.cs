using Microsoft.AspNetCore.Mvc;
using MelodyCircle.Data;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using MelodyCircle.Services;

namespace MelodyCircle.Controllers
{
    public class CollaborationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly NotificationService _notificationService;

        public CollaborationController(ApplicationDbContext context, UserManager<User> userManager, NotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: Collaboration
        //public async Task<IActionResult> Index()
        //{
        //    var publicCollaborations = await _context.Collaborations
        //        .Include(c => c.WaitingUsers)
        //        //.Where(c => c.AccessMode == AccessMode.Public)
        //        .ToListAsync();

        //    var userId = _userManager.GetUserId(User);

        //    var userInWaitingList = new Dictionary<Guid, bool>();

        //    foreach (var collaboration in publicCollaborations)
        //    {
        //        var isInWaitingList = collaboration.WaitingUsers != null && collaboration.WaitingUsers.Any(u => u.Id.ToString() == userId);

        //        userInWaitingList.Add(collaboration.Id, isInWaitingList);
        //    }

        //    ViewBag.UserInWaitingList = userInWaitingList;

        //    return View(publicCollaborations);
        //}

        //GET: Collaboration
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("EditMode");
        }

        // GET: Collaboration/EditModeCollab
        public async Task<IActionResult> EditMode()
        {
            var userId = _userManager.GetUserId(User);
            var collabsCriados = await _context.Collaborations
                .Where(t => t.CreatorId == userId)
                .ToListAsync();

            return View("EditModeCollab", collabsCriados);
        }

        // GET: Collaboration/ViewModeCollab
        public async Task<IActionResult> ViewMode()
        {
            var userId = _userManager.GetUserId(User);
            var userc = _context.Users.Find(userId);

            var collabsParticipantes = await _context.Collaborations
                .Include(c => c.ContributingUsers)
                .Where(s => s.ContributingUsers.Contains(userc))
                .ToListAsync();

            return View("ViewModeCollab", collabsParticipantes);
        }

        // GET: /Collaboration/WaitingList/{id}
        public async Task<IActionResult> WaitingList(Guid id)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.WaitingUsers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collaboration == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (userId != collaboration.CreatorId)
                return Forbid();

            var sortedWaitingUsers = collaboration.WaitingUsers.OrderBy(u => u.SignupTime).ToList();

            collaboration.WaitingUsers = sortedWaitingUsers;

            return View(collaboration);
        }
        public async Task<IActionResult> JoinQueue(Guid id)
        {
            var collaboration = await _context.Collaborations.FindAsync(id);

            if (collaboration == null)
                return NotFound();

            return View("JoinQueue", collaboration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinQueueConfim(Guid id)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.WaitingUsers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collaboration == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);

            collaboration.WaitingUsers.Add(user);

            await _context.SaveChangesAsync();
            return View("JoinQueueConfirmation", collaboration);
        }

        // POST: /Collaboration/AllowUser/{collaborationId}/{userId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllowUser(Guid collaborationId, string userId)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.WaitingUsers)
                .Include(c => c.ContributingUsers)
                .FirstOrDefaultAsync(c => c.Id == collaborationId);

            if (collaboration == null)
                return NotFound();

            var creatorId = _userManager.GetUserId(User);

            if (creatorId != collaboration.CreatorId)
                return Forbid();

            if (collaboration.ContributingUsers.Count >= collaboration.MaxUsers)
            {
                ViewData["ErrorMessage"] = "Já atingiu o máximo de utilizadores";

                return View("WaitingList", collaboration);
            }

            var userToRemove = collaboration.WaitingUsers.FirstOrDefault(u => u.Id.ToString() == userId);

            if (userToRemove != null)
            {
                collaboration.WaitingUsers.Remove(userToRemove);
                collaboration.ContributingUsers.Add(userToRemove);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(WaitingList), new { id = collaborationId });
        }

        // GET: /Collaboration/ContributingUsers/{collaborationId}
        public async Task<IActionResult> ContributingUsers(Guid id)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.ContributingUsers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collaboration == null)
                return NotFound();

            return View("ContributingList", collaboration);
        }

        // POST: /Collaboration/RemoveUser/{collaborationId}/{userId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(Guid collaborationId, string userId)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.ContributingUsers)
                .FirstOrDefaultAsync(c => c.Id == collaborationId);

            if (collaboration == null)
                return NotFound();

            var creatorId = _userManager.GetUserId(User);

            if (creatorId != collaboration.CreatorId)
                return Forbid();

            var userToRemove = collaboration.ContributingUsers.FirstOrDefault(u => u.Id.ToString() == userId);

            if (userToRemove != null)
            {
                collaboration.ContributingUsers.Remove(userToRemove);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(WaitingList), new { id = collaborationId });
        }

        public async Task<IActionResult> InviteToCollab(Guid collaborationId, string userId)
        {
            //var collaboration = await _context.Collaborations
            //    .Include(c => c.WaitingUsers)
            //    .FirstOrDefaultAsync(c => c.Id == id);

            var collaboration = await _context.Collaborations
                .Include(c => c.ContributingUsers)
                .FirstOrDefaultAsync(c => c.Id == collaborationId);

            if (collaboration == null)
                return NotFound();

            var user = await _userManager.FindByNameAsync(userId);

            if(user == null)
                return NotFound();

            collaboration.WaitingUsers.Add(user);

            await _context.SaveChangesAsync();

            await _notificationService.SendCollaborationInviteAsync(
               senderId: collaboration.CreatorId, // Assuming creator sends the invite
               recipientId: userId,
               collaborationId: collaboration.Id,
               collaborationTitle: collaboration.Title,
               collaborationDescription: collaboration.Description);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PrivateCollaborations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var collaborations = await _context.Collaborations
                .Where(c => c.CreatorId == userId && c.AccessMode == AccessMode.Private)
                .ToListAsync();

            return PartialView("_PrivateCollaborationsPartial", collaborations);
        }

        public IActionResult Create()
        {
            var model = new Collaboration();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Collaboration collaboration, IFormFile photo)
        {
            if (string.IsNullOrEmpty(collaboration.Title) || collaboration.MaxUsers <= 0 || photo == null || photo.Length == 0)
            {
                ModelState.AddModelError(nameof(collaboration.Title), "O título é obrigatório");
                ModelState.AddModelError(nameof(collaboration.MaxUsers), "É necessário pelo menos 1 utilizador como máximo");
                ModelState.AddModelError(nameof(collaboration.Photo), "A foto é obrigatória");
            }

            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    collaboration.Photo = memoryStream.ToArray();
                    collaboration.PhotoContentType = photo.ContentType;
                }

                collaboration.WaitingUsers = new List<User>();
                collaboration.CreatedDate = DateTime.Now;
                collaboration.CreatorId = _userManager.GetUserId(User);

                _context.Add(collaboration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collaboration);
        }

        // GET: /collaboration/edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var collaboration = await _context.Collaborations.FindAsync(id);

            if (collaboration == null)
                return NotFound();

            if (collaboration.CreatorId != _userManager.GetUserId(User))
                return Forbid();

            if(collaboration.IsFinished)
                return Forbid();

            return View(collaboration);
        }

        // POST: /collaboration/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Collaboration collaboration, IFormFile photo)
        {
            if (!collaboration.IsFinished)
            {
                if (id != collaboration.Id)
                    return NotFound();

                //if (collaboration.CreatorId != _userManager.GetUserId(User))
                //    return Forbid();

                if (string.IsNullOrEmpty(collaboration.Title) || collaboration.MaxUsers <= 0)
                {
                    ModelState.AddModelError(nameof(collaboration.Title), "O título é obrigatório");
                    ModelState.AddModelError(nameof(collaboration.MaxUsers), "É necessário pelo menos 1 utilizador como máximo");
                }

                else
                {
                    var existingCollaboration = await _context.Collaborations.FindAsync(id);

                    if (photo != null && photo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await photo.CopyToAsync(memoryStream);
                            existingCollaboration.Photo = memoryStream.ToArray();
                            existingCollaboration.PhotoContentType = photo.ContentType;
                        }
                    }

                    existingCollaboration.Title = collaboration.Title;
                    existingCollaboration.Description = collaboration.Description;
                    existingCollaboration.MaxUsers = collaboration.MaxUsers;
                    existingCollaboration.AccessMode = collaboration.AccessMode;
                    existingCollaboration.AccessPassword = collaboration.AccessPassword;

                    _context.Update(existingCollaboration);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(collaboration);
            }
            return Forbid();
        }

        // GET: /collaboration/delete/{id}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var collaboration = await _context.Collaborations.FirstOrDefaultAsync(m => m.Id == id);

            if (collaboration == null)
                return NotFound();

            if (collaboration.CreatorId != _userManager.GetUserId(User))
                return Forbid();

            if (collaboration.IsFinished)
                return Forbid();

            return View(collaboration);
        }

        // POST: /collaboration/delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var collaboration = await _context.Collaborations.FindAsync(id);

            if (collaboration == null)
                return NotFound();

            if (collaboration.CreatorId != _userManager.GetUserId(User))
                return Forbid();

            if (!collaboration.IsFinished)
            {
                _context.Collaborations.Remove(collaboration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return Forbid();
        }

        // GET: /collaboration/finish/{id}
        public async Task<IActionResult> Finish(Guid? id)
        {
            if (id == null)
                return NotFound();

            var collaboration = await _context.Collaborations.FindAsync(id);

            if (collaboration == null)
                return NotFound();

            if (collaboration.CreatorId != _userManager.GetUserId(User))
                return Forbid();

            if (collaboration.IsFinished)
                return Forbid();

            ViewBag.CollaborationId = id;

            return View();
        }

        // POST: /collaboration/finishconfirmed/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishConfirmed(Guid id)
        {
            var collaboration = await _context.Collaborations.FindAsync(id);

            if (collaboration == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (userId != collaboration.CreatorId)
                return Forbid();

            if (!collaboration.IsFinished)
            {
                collaboration.IsFinished = true;
                _context.Update(collaboration);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return Forbid();
        }

        private async Task<ActionResult<Collaboration>> GetCollaboration(Guid id)
        {
            var collaboration = await _context.Collaborations.FindAsync(id);

            if (collaboration == null)
                return NotFound();

            return collaboration;
        }

        public async Task<IActionResult> RateCollaboration(Guid id, int rating)
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

            var collabToRate = await _context.Collaborations.FindAsync(id);

            if (collabToRate == null)
            {
                return NotFound("Collaboration to rate not found");
            }

            if (collabToRate.Ratings == null)
            {
                collabToRate.Ratings = new List<CollaborationRating>();
            }

            var existingRatings = _context.CollaborationRating.AsEnumerable().Where(r => r.UserName.Equals(currentUser.UserName));
            var existingRating = existingRatings.FirstOrDefault(u => u.CollaborationId.Equals(id));

            if (existingRating != null)
            {
                existingRating.Value = rating;
            }
            else
            {
                collabToRate.Ratings.Add(new CollaborationRating { UserName = currentUser.UserName, CollaborationId = id, Value = rating });
            }

            // Update the user in the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool CollaborationExists(Guid id)
        {
            return _context.Collaborations.Any(e => e.Id == id);
        }
    }
}