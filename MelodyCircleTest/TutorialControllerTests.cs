using MelodyCircle.Controllers;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MelodyCircleTest
{
    public class TutorialControllerTests
    {
        [Fact]
        public async Task Index_RedirectsToEditMode_WhenUserIsInTeacherOrModOrAdminRole()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Configurar o utilizador autenticado com o papel de professor
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Teacher")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthType");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var controller = new TutorialController(context, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userPrincipal }
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditMode", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Index_RedirectsToViewMode_WhenUserIsNotInTeacherOrModOrAdminRole()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Configurar o utilizador autenticado sem papel de professor, moderador ou administrador
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Student")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthType");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var controller = new TutorialController(context, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userPrincipal }
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewMode", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new TutorialController(null, null);
            controller.ModelState.AddModelError("Error", "Test error");
            var tutorial = new Tutorial();

            // Act
            var result = await controller.Create(tutorial, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tutorial, viewResult.Model);
        }

        [Fact]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);
            var controller = new TutorialController(context, userManager);

            var tutorial = new Tutorial { Title = "Test Title", Description = "Test Description" };

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthType");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            // Act
            var result = await controller.Create(tutorial, null); // Pass image file

            // Assert
            var createdTutorial = await context.Tutorials.FirstOrDefaultAsync(t => t.Title == tutorial.Title && t.Description == tutorial.Description);
            Assert.NotNull(createdTutorial);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);
            var controller = new TutorialController(context, userManager);

            // Act
            var result = await controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenTutorialIsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);
            var controller = new TutorialController(context, userManager);

            // Act
            var result = await controller.Edit(Guid.NewGuid()); // Assuming the id does not exist in the database

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WhenIdAndTutorialAreValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);
            var controller = new TutorialController(context, userManager);

            var tutorial = new Tutorial { Id = Guid.NewGuid(), Title = "Test Title", Description = "Test Description", Creator = "TestUser" };
            context.Tutorials.Add(tutorial);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Edit(tutorial.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tutorial, viewResult.Model);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);
            var controller = new TutorialController(context, null);

            // Act
            var result = await controller.Delete(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTutorialNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);
            var controller = new TutorialController(context, null);

            // Act
            var result = await controller.Delete(Guid.NewGuid()); // Assuming the id does not exist

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsRedirectToActionResult_WhenTutorialDeleted()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var tutorialId = Guid.NewGuid();
            var tutorial = new Tutorial { Id = tutorialId, Title = "Test Title", Description = "Test Description", Creator = "TestUser" }; 
            context.Tutorials.Add(tutorial);
            await context.SaveChangesAsync();

            var controller = new TutorialController(context, null);

            // Act
            var result = await controller.DeleteConfirmed(tutorialId); // Use DeleteConfirmed instead of Delete

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}