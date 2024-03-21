using MelodyCircle.Controllers;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using System.Text;

namespace MelodyCircle.Tests.Controllers
{
    public class TutorialControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfTutorials()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Adiciona alguns tutoriais fictícios ao contexto do banco de dados em memória
            context.Tutorials.AddRange(
                new Tutorial { Id = Guid.NewGuid(), Title = "Tutorial 1", Description = "Description 1", Creator = "User 1" },
                new Tutorial { Id = Guid.NewGuid(), Title = "Tutorial 2", Description = "Description 2", Creator = "User 2" },
                new Tutorial { Id = Guid.NewGuid(), Title = "Tutorial 3", Description = "Description 3", Creator = "User 3" }
            );
            await context.SaveChangesAsync();

            var controller = new TutorialController(context, null);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Tutorial>>(viewResult.ViewData.Model);
            Assert.Equal(4, model.Count());
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

            var tutorial = new Tutorial { Title = "Test Title", Description = "Test Description", Creator = "TestUser" };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            // Act
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
            var result = await controller.Create(tutorial, null); // Passar o arquivo de imagem

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Contains(tutorial, context.Tutorials);
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

            var tutorial = new Tutorial { Id = Guid.NewGuid(), Title = "Test Title", Description = "Test Description", Creator = "TestUser" }; // Definindo manualmente a propriedade Creator
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
            var tutorial = new Tutorial { Id = tutorialId, Title = "Test Title", Description = "Test Description", Creator = "TestUser" }; // Definindo manualmente a propriedade Creator
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