using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MelodyCircleTest
{
    /* Guilherme Bernardino, Rodrigo Nogueira */
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
				new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, AccessPassword = "SAD" },
				new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, AccessPassword = "SAD" },
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
		public async Task UserTutorialStats_RedirectsToIndex()
		{
			var user = new User { Id = Guid.NewGuid().ToString(), UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };

			var photo = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("fake image")), 0, 0, "photo", "photo.jpg");

			var tutorialData = new List<Tutorial>
			{
				new Tutorial { CreationDate = DateTime.Now.AddDays(-10), Creator = "testUser", Title = "SADASD", Description = "asddas"},
				new Tutorial
				{
					CreationDate = DateTime.Now.AddDays(-20), Creator = "testUser", Title = "SADASD",
					Description = "asddas"
				},
			};

			using (var memoryStream = new MemoryStream())
			{
				await photo.CopyToAsync(memoryStream);
				tutorialData[0].Photo = memoryStream.ToArray();
				tutorialData[1].Photo = memoryStream.ToArray();
			}

			_context.Tutorials.AddRange(tutorialData);

			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);

			var result = await _controller.UserTutorialStats(user.UserName);
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task UserCollaborationStats_RedirectsToIndex()
		{
            var user = new User { Id = Guid.NewGuid().ToString(), UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };

            var photo = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("fake image")), 0, 0, "photo", "photo.jpg");

			var collabData = new List<Collaboration>
			{
                new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "AD", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, AccessPassword = "SAD" },
                new Collaboration { Id = Guid.NewGuid(), CreatorId = "1" , Title = "ADs", Description = "asddas", CreatedDate = DateTime.Now, AccessMode = AccessMode.Private, AccessPassword = "SAD" },
			};

			using (var memoryStream = new MemoryStream())
			{
				await photo.CopyToAsync(memoryStream);
				collabData[0].Photo = memoryStream.ToArray();
				collabData[1].Photo = memoryStream.ToArray();
			}

			_context.Collaborations.AddRange(collabData);

			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);

			var result = await _controller.UserCollaborationStats(user.Id);
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task UserStepStats_RedirectsToIndex()
		{
			var user = new User { Id = Guid.NewGuid().ToString(), UserName = "sad", Name = "asddsa", BirthDate = DateOnly.MaxValue, Password = "sad" };

			var tuto = new Tutorial
				{ CreationDate = DateTime.Now.AddDays(-10), Creator = "testUser", Title = "SADASD", Description = "asddas" };

			var stepData = new List<Step>
			{
				new Step { CreationDate = DateTime.Now.AddDays(-10) , Title = "ads", Order = 1, Content = "asdasd", TutorialId = tuto.Id, Tutorial = tuto},
				new Step { CreationDate = DateTime.Now.AddDays(-20) , Title = "ads", Order = 2, Content = "asdasd",TutorialId = tuto.Id, Tutorial = tuto},
			};

			_context.Steps.AddRange(stepData);

			await _context.SaveChangesAsync();

			_mockUserManager.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);

			var result = await _controller.UserStepStats(user.UserName);
			Assert.IsType<ViewResult>(result);
		}

	}
}
