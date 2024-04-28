using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MelodyCircleTest
{
    public class StepControllerTests
    {
        private readonly UserManager<User> _userManager;


        public StepControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManager = new UserManager<User>(store.Object, null, null, null, null, null, null, null, null);
        }
           

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfSteps()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var tutorial = new Tutorial
            {
                Id = Guid.NewGuid(),
                Title = "Test Tutorial",
                Creator = "Test Creator",
                Description = "Test Description" 
            };
            context.Tutorials.Add(tutorial);

            context.Steps.AddRange(
                new Step { Id = Guid.NewGuid(), TutorialId = tutorial.Id, Title = "Step 1", Content = "Content 1", Order = 1 },
                new Step { Id = Guid.NewGuid(), TutorialId = tutorial.Id, Title = "Step 2", Content = "Content 2", Order = 2 },
                new Step { Id = Guid.NewGuid(), TutorialId = Guid.NewGuid(), Title = "Another Tutorial Step", Content = "Another Content", Order = 1 } 
            );
            await context.SaveChangesAsync();

            var controller = new StepController(context, _userManager);

            // Act
            var result = await controller.Index(tutorial.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Step>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count); 
            Assert.All(model, item => Assert.Equal(tutorial.Id, item.TutorialId)); // 
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesStepAndRedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var tutorialId = Guid.NewGuid();
            var stepId = Guid.NewGuid();
            var step = new Step { Id = stepId, TutorialId = tutorialId, Title = "Test Step", Content = "Test Content", Order = 1 };
            context.Steps.Add(step);
            await context.SaveChangesAsync();

            var controller = new StepController(context, _userManager);

            // Act
            var result = await controller.DeleteConfirmed(stepId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(tutorialId, redirectToActionResult.RouteValues["tutorialId"]);

            var stepInDatabase = await context.Steps.FirstOrDefaultAsync(s => s.Id == stepId);
            Assert.Null(stepInDatabase);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithStep()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            var tutorialId = Guid.NewGuid();
            var stepId = Guid.NewGuid();
            var originalTitle = "Original Step";
            var step = new Step { Id = stepId, TutorialId = tutorialId, Title = originalTitle, Content = "Original Content", Order = 1 };
            context.Steps.Add(step);
            await context.SaveChangesAsync();

            var controller = new StepController(context, _userManager);

            // Act
            var result = await controller.Edit(stepId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Step>(viewResult.ViewData.Model);
            Assert.Equal(stepId, model.Id);
            Assert.Equal(originalTitle, model.Title);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirectToActionResult_WithUpdatedStep()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            Guid tutorialId = Guid.NewGuid();
            Guid stepId = Guid.NewGuid();

            using (var context = new ApplicationDbContext(options))
            {
                context.Steps.Add(new Step { Id = stepId, TutorialId = tutorialId, Title = "Title 1", Content = "Content 1", Order = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new StepController(context, _userManager);

                // Act
                var step = new Step { Id = stepId, TutorialId = tutorialId, Title = "Updated Title", Content = "Updated Content", Order = 1 };
                var result = await controller.Edit(tutorialId, step);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.Equal(tutorialId, redirectToActionResult.RouteValues["tutorialId"]);
            }
        }
    }
}