using Microsoft.AspNetCore.Mvc;
using MelodyCircle.Data;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using MelodyCircle.Services;
using NAudio.Wave;

namespace MelodyCircle.Controllers
{
    public class CollaborationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly NotificationService _notificationService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        public CollaborationController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment hostingEnvironment, NotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
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
                .Where(t => t.CreatorId == userId).ToListAsync();

            return View("EditModeCollab", collabsCriados);
        }

        // GET: Collaboration/ViewModeCollab
        public async Task<IActionResult> ViewMode()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Find(userId);

            var collabsParticipantes = await _context.Collaborations
                .Include(c => c.ContributingUsers)
                .Where(s => s.ContributingUsers.Contains(user))
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
        public async Task<IActionResult> JoinQueueConfirm(Guid id)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.WaitingUsers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collaboration == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            if (!collaboration.WaitingUsers.Any(u => u.Id == user.Id))
            {
                collaboration.WaitingUsers.Add(user);
                await _context.SaveChangesAsync();
            }

            return View("JoinQueueConfirmation", collaboration);
        }

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

            var userToAdd = await _context.Users.FindAsync(userId);

            if (userToAdd != null && collaboration.WaitingUsers.Contains(userToAdd))
            {
                collaboration.WaitingUsers.Remove(userToAdd);
                collaboration.ContributingUsers.Add(userToAdd);
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

            return RedirectToAction(nameof(ContributingUsers), new { id = collaborationId });
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

        //public async Task<IActionResult> PrivateCollaborations()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var collaborations = await _context.Collaborations
        //        .Where(c => c.CreatorId == userId && c.AccessMode == AccessMode.Private)
        //        .ToListAsync();

        //    return PartialView("_PrivateCollaborationsPartial", collaborations);
        //}

        public IActionResult Create()
        {
            var model = new Collaboration();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Collaboration collaboration, IFormFile photo)
        {
            collaboration.CreatorId = _userManager.GetUserId(User);

            var user = await _context.Users.FindAsync(collaboration.CreatorId);

            if (user == null)
                return NotFound("User not found");

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

                collaboration.ContributingUsers.Add(user);

                collaboration.CreatedDate = DateTime.Now;

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

        // GET: /Collaboration/ArrangementPanel/{id}
        public async Task<IActionResult> ArrangementPanel(Guid id)
        {
            var collaboration = await _context.Collaborations
                .Include(c => c.ContributingUsers)
                .Include(c => c.Tracks)
                    .ThenInclude(t => t.InstrumentsOnTrack)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collaboration == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            var isContributorOrCreator = collaboration.ContributingUsers.Any(u => u.Id == userId) || collaboration.CreatorId == userId;

            if (!isContributorOrCreator)
                return Forbid();

            Track userTrack = collaboration.Tracks.FirstOrDefault(t => t.AssignedUserId.ToString() == userId);

            if (userTrack == null && collaboration.ContributingUsers.Any(u => u.Id == userId))
            {
                userTrack = new Track
                {
                    Id = Guid.NewGuid(),
                    AssignedUserId = Guid.Parse(userId),
                    CollaborationId = id,
                    BPM = 102,
                    Duration = TimeSpan.FromMinutes(4)
                };

                _context.Tracks.Add(userTrack);

                await _context.SaveChangesAsync();
            }

            var assignedTrackNumber = userTrack != null ? collaboration.Tracks.IndexOf(userTrack) + 1 : 1;

            var arrangementViewModel = new ArrangementPanelViewModel
            {
                Collaboration = collaboration,
                Tracks = collaboration.Tracks,
                IsContributorOrCreator = isContributorOrCreator,
                UserTrack = userTrack,
                AssignedTrackNumber = assignedTrackNumber,
                AvailableInstruments = InstrumentData.AvailableInstruments,
                UploadedInstruments = new List<UploadedInstrument>()
            };

            var uploadedInstruments = await _context.UploadedInstruments
                .Where(ui => ui.CollaborationId == id)
                .ToListAsync();

            arrangementViewModel.UploadedInstruments = uploadedInstruments;

            return View("Painel", arrangementViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInstrumentToTrack([FromBody] InstrumentOnTrackDto dto)
        {
            var track = await _context.Tracks.Include(t => t.InstrumentsOnTrack).FirstOrDefaultAsync(t => t.Id == dto.TrackId);

            if (track == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (track.AssignedUserId.ToString() != userId)
                return Forbid();

            TimeSpan duration;
            if (dto.IsUploaded)
            {
                if (!dto.InstrumentId.HasValue)
                    return BadRequest("Uploaded instrument ID is required.");

                var uploadedInstrument = await _context.UploadedInstruments.FirstOrDefaultAsync(ui => ui.Id == dto.InstrumentId.Value);
                if (uploadedInstrument == null)
                    return NotFound("Uploaded instrument not found.");

                using (var stream = new MemoryStream(uploadedInstrument.SoundContent))
                using (var reader = new Mp3FileReader(stream))
                {
                    duration = reader.TotalTime;
                }
            }
            else
            {
                var instrumentFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "sounds", dto.InstrumentName.ToLower() + ".mp3");
                duration = GetAudioDuration(instrumentFilePath);
            }

            var instrument = new InstrumentOnTrack
            {
                Id = Guid.NewGuid(),
                TrackId = (Guid)dto.TrackId,
                InstrumentType = dto.InstrumentName,
                StartTime = TimeSpan.FromSeconds((double)dto.StartTime),
                Duration = duration,
                Track = track,
                InstrumentId = dto.IsUploaded ? dto.InstrumentId : null
            };

            if (instrument.Id == null || instrument.TrackId == null || instrument.InstrumentType == null || instrument.StartTime == null || instrument.Duration == null || track == null)
                return NotFound();

            _context.InstrumentOnTrack.Add(instrument);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                instrumentId = instrument.Id,
                duration = duration.TotalSeconds
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveInstrumentFromTrack([FromBody] InstrumentOnTrackDto dto)
        {
            var instrumentOnTrack = await _context.InstrumentOnTrack.FindAsync(dto.InstrumentId);

            if (instrumentOnTrack == null)
                return NotFound();

            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == instrumentOnTrack.TrackId);

            var userId = _userManager.GetUserId(User);

            if (track == null || track.AssignedUserId.ToString() != userId)
                return Forbid();

            _context.InstrumentOnTrack.Remove(instrumentOnTrack);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Instrumento removido com sucesso" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadInstrument(Guid collaborationId, string instrumentName, IFormFile soundFile)
        {
            if (soundFile != null && soundFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await soundFile.CopyToAsync(memoryStream);

                var uploadedInstrument = new UploadedInstrument
                {
                    Id = Guid.NewGuid(),
                    Name = instrumentName,
                    SoundContent = memoryStream.ToArray(),
                    CollaborationId = collaborationId
                };

                _context.UploadedInstruments.Add(uploadedInstrument);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            //return Json(new { success = false, message = "Invalid file." });

            return RedirectToAction("ArrangementPanel", new { id = collaborationId });

        }

        [HttpGet]
        public async Task<IActionResult> GetInstrumentAudio(Guid id)
        {
            var uploadedInstrument = await _context.UploadedInstruments
                .FirstOrDefaultAsync(ui => ui.Id == id);

            if (uploadedInstrument == null)
            {
                return NotFound();
            }

            return File(uploadedInstrument.SoundContent, "audio/mp3");
        }

        private TimeSpan GetAudioDuration(string filePath)
        {
            using var reader = new Mp3FileReader(filePath);
            return reader.TotalTime;
        }

        private bool CollaborationExists(Guid id)
        {
            return _context.Collaborations.Any(e => e.Id == id);
        }
    }
}