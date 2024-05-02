using System.Diagnostics;
using System.Security.Claims;
using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MelodyCircleTest
{
    public class StatControllerTests : IClassFixture<ApplicationDbContextFixture>
	{
		private ApplicationDbContext _context;
		private readonly Mock<UserManager<User>> _mockUserManager;
		private readonly StatController _controller;
		private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
		private readonly AdminController _adminController;

		public StatControllerTests(ApplicationDbContextFixture applicationDbContextFixture)
		{
			_context = applicationDbContextFixture.DbContext;
			_mockUserManager = new Mock<UserManager<User>>(
		new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
			_controller = new StatController(_context, _mockUserManager.Object);
			var roleStore = new Mock<IRoleStore<IdentityRole>>();
			_mockRoleManager = new Mock<RoleManager<IdentityRole>>(
				roleStore.Object, null, null, null, null);

		}


		[Fact]
		public void UserCreationStats_ReturnsViewResult()
		{
			var usersData = new List<User>
			{
				new User { CreationDate = DateTime.Now.AddDays(-10) },
				new User { CreationDate = DateTime.Now.AddDays(-20) },
			};

			_context.Users.AddRange(usersData);
			_context.SaveChangesAsync();
			
			var result = _controller.UserCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void TutorialCreationStats_ReturnsViewResult()
		{
			var tutorialData = new List<Tutorial>
			{
				new Tutorial { CreationDate = DateTime.Now.AddDays(-10) },
				new Tutorial { CreationDate = DateTime.Now.AddDays(-20) },
			};

			_context.Tutorials.AddRange(tutorialData);
			_context.SaveChangesAsync();

			var result = _controller.TutorialCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void CollaborationCreationStats_ReturnsViewResult()
		{
			var collabData = new List<Collaboration>
			{
				new Collaboration { CreatedDate = DateTime.Now.AddDays(-10) },
				new Collaboration { CreatedDate = DateTime.Now.AddDays(-20) },
			};

			_context.Collaborations.AddRange(collabData);
			_context.SaveChangesAsync();

			var result = _controller.CollaborationCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void StepCreationStats_ReturnsViewResult()
		{
			var stepData = new List<Step>
			{
				new Step { CreationDate = DateTime.Now.AddDays(-10) },
				new Step { CreationDate = DateTime.Now.AddDays(-20) },
			};

			_context.Steps.AddRange(stepData);
			_context.SaveChangesAsync();


			var result = _controller.StepCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void UserTutorialStats_RedirectsToIndex()
		{
			var user = new User { UserName = "testUser", NormalizedUserName = "TESTUSER"};

			var tutorialData = new List<Tutorial>
			{
				new Tutorial { CreationDate = DateTime.Now.AddDays(-10), Creator = "testUser"},
				new Tutorial { CreationDate = DateTime.Now.AddDays(-20), Creator = "testUser"},
			};

			_context.Users.Add(user);
			_context.Tutorials.AddRange(tutorialData);
			_context.SaveChangesAsync();

			var result = _controller.UserTutorialStats(user.UserName);
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void UserCollaborationStats_RedirectsToIndex()
		{
            var user = new User { Id = "1", UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };

            var tutorialData = new List<Collaboration>
			{
                new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private},
                new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private},
			};


			_context.Collaborations.AddRange(tutorialData);
			_context.SaveChangesAsync();

            _mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

            var result = _controller.UserCollaborationStats(user.Id);
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void UserStepStats_RedirectsToIndex()
		{
			var user = new User { UserName = "testUser", NormalizedUserName = "TESTUSER" };

			var stepData = new List<Step>
			{
				new Step { CreationDate = DateTime.Now.AddDays(-10) },
				new Step { CreationDate = DateTime.Now.AddDays(-20) },
			};

			_context.Users.Add(user);

			_context.Steps.AddRange(stepData);
			_context.SaveChangesAsync();

			var result = _controller.UserStepStats(user.UserName);
			Assert.IsType<ViewResult>(result);
		}

	}
}
