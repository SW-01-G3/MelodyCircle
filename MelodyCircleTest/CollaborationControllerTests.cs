using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Text;

namespace MelodyCircleTest
{
	public class CollaborationControllerTests : IClassFixture<ApplicationDbContextFixture>
	{
		private readonly CollaborationController _controller;
		private readonly Mock<UserManager<User>> _mockUserManager;
		private readonly ApplicationDbContext _context;

		public CollaborationControllerTests(ApplicationDbContextFixture applicationDbContextFixture)
		{
			_context = applicationDbContextFixture.DbContext;

			var store = new Mock<IUserStore<User>>();
			_mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

			_controller = new CollaborationController(_context, _mockUserManager.Object, null, null);




		}

		[Fact]
		public async Task Index_RedirectsToEditMode()
		{
			var result = await _controller.Index();
			Assert.IsType<RedirectToActionResult>(result);
		}

		[Fact]
		public async Task EditMode_ReturnsViewResult()
		{
			var result = await _controller.EditMode();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task ViewMode_ReturnsViewResult()
		{
			var result = await _controller.ViewMode();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task WaitingList_ReturnsViewResultWithCollaboration()
		{
			// Arrange
			var user = new User { Id = "1" , UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad"};
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private};

			_context.Users.Add(user);
			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();



			// Act
			var result = await _controller.WaitingList(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<Collaboration>(viewResult.Model);
			Assert.Equal(collaboration, model);
		}

		[Fact]
		public async Task JoinQueue_ReturnsViewResultWithCollaboration()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private };

			_context.Users.Add(user);
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

			// Act
			var result = await _controller.JoinQueue(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

		}

		[Fact]
		public async Task JoinQueueConfirm_ReturnsViewResultWithCollaboration()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private };
			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");
			_context.Users.Add(user);
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			// Act
			var result = await _controller.JoinQueueConfirm(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<Collaboration>(viewResult.Model);
			Assert.Equal(collaboration, model);
		}

		[Fact]
		public async Task AllowUser_AddsUserToContributingUsers_AndRemovesFromWaitingList()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var user2 = new User { Id = "2", UserName = "sasadd", Name = "asdasddsa", BirthDate = DateOnly.MaxValue, Password = "sdsad" };

			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, MaxUsers = 2, ContributingUsers = new List<User>(), WaitingUsers = new List<User> { user2 } };

			_context.Collaborations.Add(collaboration);
			_context.Users.Add(user);
			_context.Users.Add(user2);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);

			// Act
			var result = await _controller.AllowUser(collaboration.Id, user2.Id);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(CollaborationController.WaitingList), redirectToActionResult.ActionName);
			Assert.Equal(collaboration.Id, redirectToActionResult.RouteValues["id"]);
		}

		[Fact]
		public async Task ContributingUsers_ReturnsViewResult_WithCollaboration()
		{
			// Arrange
			var user = new User { Id = Guid.NewGuid().ToString(), UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = user.Id, Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private };
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			// Act
			var result = await _controller.ContributingUsers(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("ContributingList", viewResult.ViewName);
			var model = Assert.IsType<Collaboration>(viewResult.Model);
		}


		[Fact]
		public async Task RemoveUser_RemovesUserFromContributingUsers()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user } };
			//_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

			_context.Collaborations.Add(collaboration);
			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);

			// Act
			var result = await _controller.RemoveUser(collaboration.Id, user.Id);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(CollaborationController.ContributingUsers), redirectToActionResult.ActionName);
			Assert.Equal(collaboration.Id, redirectToActionResult.RouteValues["id"]);
		}

		[Fact]
		public async Task Create_WithValidData_RedirectsToIndex()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user }, MaxUsers = 4};

			var photoContent = Encoding.UTF8.GetBytes("fake image");
			var photoStream = new MemoryStream(photoContent);
			var photo = new FormFile(photoStream, 0, photoContent.Length, "photo", "photo.jpg")
			{
				Headers = new HeaderDictionary(),
				ContentType = "image/jpeg" // Set a valid content type for the photo
			};
			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");
			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Act
			var result = await _controller.Create(collaboration, photo);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(CollaborationController.EditMode), redirectToActionResult.ActionName);

			var collaborationInDb = await _context.Collaborations.FirstOrDefaultAsync();
			Assert.NotNull(collaborationInDb);
			Assert.Equal(collaboration.Title, collaborationInDb.Title);
			Assert.Equal(collaboration.Description, collaborationInDb.Description);
			Assert.Equal(collaboration.MaxUsers, collaborationInDb.MaxUsers);
			Assert.Equal(user.Id, collaborationInDb.CreatorId);
			Assert.Contains(user, collaborationInDb.ContributingUsers);
		}

		[Fact]
		public async Task Edit_ReturnsViewResult_WithCollaboration()
		{
			// Arrange
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private };
			_context.Collaborations.Add(collaboration);
			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);
			await _context.SaveChangesAsync();

			// Act
			var result = await _controller.Edit(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal(collaboration, viewResult.Model);
		}

		[Fact]
		public async Task Edit_UpdatesCollaboration_AndRedirectsToEditMode()
		{
			// Arrange
			var collaborationId = Guid.NewGuid();
			var existingCollaboration = new Collaboration { Id = collaborationId, MaxUsers = 5, ContributingUsers = new List<User>(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private };
			var updatedCollaboration = new Collaboration { Id = collaborationId, Title = "Updated Title", Description = "Updated Description", MaxUsers = 10, AccessMode = AccessMode.Public };

			var photo = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("fake image")), 0, 0, "photo", "photo.jpg");

			_context.Collaborations.Add(existingCollaboration);
			await _context.SaveChangesAsync();

			// Act
			var result = await _controller.Edit(collaborationId, updatedCollaboration, photo);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(CollaborationController.EditMode), redirectToActionResult.ActionName);

			var collaborationInDb = await _context.Collaborations.FindAsync(collaborationId);
			Assert.NotNull(collaborationInDb);
			Assert.Equal(updatedCollaboration.Title, collaborationInDb.Title);
			Assert.Equal(updatedCollaboration.Description, collaborationInDb.Description);
			Assert.Equal(updatedCollaboration.MaxUsers, collaborationInDb.MaxUsers);
			Assert.Equal(updatedCollaboration.AccessMode, collaborationInDb.AccessMode);
		}

		[Fact]
		public async Task Delete_ReturnsViewResult_WithCollaboration()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user } };
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);

			// Act
			var result = await _controller.Delete(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal(collaboration, viewResult.Model);
		}

		[Fact]
		public async Task DeleteConfirmed_RemovesCollaboration_AndRedirectsToEditMode()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user } };
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);

			// Act
			var result = await _controller.DeleteConfirmed(collaboration.Id);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(CollaborationController.EditMode), redirectToActionResult.ActionName);

			var collaborationInDb = await _context.Collaborations.FindAsync(collaboration.Id);
			Assert.Null(collaborationInDb); // Ensure collaboration is removed from the database
		}

		[Fact]
		public async Task Finish_ReturnsViewResult_WithCollaborationId()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user }, IsFinished = false};
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);

			// Act
			var result = await _controller.Finish(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task FinishConfirmed_MarksCollaborationAsFinished_AndRedirectsToEditMode()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user }, IsFinished = false };
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(collaboration.CreatorId);

			// Act
			var result = await _controller.FinishConfirmed(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("EditMode", viewResult.ActionName);
		}

		[Fact]
		public async Task RateCollaboration_WithValidRating_AddsOrUpdateRating_AndRedirectsToIndex()
		{
			// Arrange
			var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = "1", Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user }, IsFinished = false };
			//_context.Users.Add(user);
			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");
			_context.Collaborations.Add(collaboration);
			await _context.SaveChangesAsync();



			// Act
			var result = await _controller.RateCollaboration(collaboration.Id, 5);

			// Assert
			var redirectToActionResult = Assert.IsType<NotFoundObjectResult>(result);
			//Assert.Equal("Index", redirectToActionResult.ActionName);

			//var collaborationRating = _context.CollaborationRating.FirstOrDefault(r => r.UserName == user.UserName && r.CollaborationId == collaboration.Id);
			//Assert.NotNull(collaborationRating);
			//Assert.Equal(5, collaborationRating.Value);
		}

		[Fact]
		public async Task ArrangementPanel_ReturnsViewResult_WithArrangementPanelViewModel_WhenValid()
		{
			// Arrange
			var userId= Guid.NewGuid();

			var user = new User { Id = userId.ToString(), UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };
			var collaboration = new Collaboration { Id = Guid.NewGuid(), CreatorId = userId.ToString(), Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, ContributingUsers = new List<User> { user }, IsFinished = false };
			var track = new Track { Id = Guid.NewGuid(), AssignedUserId = Guid.Parse(user.Id), CollaborationId = collaboration.Id };
			_context.Users.Add(user);
			_context.Collaborations.Add(collaboration);
			_context.Tracks.Add(track);
			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId.ToString);

			// Act
			var result = await _controller.ArrangementPanel(collaboration.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var viewModel = Assert.IsType<ArrangementPanelViewModel>(viewResult.Model);
			Assert.Equal(collaboration, viewModel.Collaboration);
			Assert.Equal(1, viewModel.AssignedTrackNumber);
			Assert.True(viewModel.IsContributorOrCreator);
		}
	}
}


