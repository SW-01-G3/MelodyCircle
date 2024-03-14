using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MelodyCircle.Tests.Controllers
{
    public class StepControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfSteps()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Adiciona um tutorial fictício ao contexto do banco de dados em memória
            var tutorial = new Tutorial
            {
                Id = Guid.NewGuid(),
                Title = "Test Tutorial",
                Creator = "Test Creator", // Defina o Creator como um valor válido
                Description = "Test Description" // Defina a Description como um valor válido
            };
            context.Tutorials.Add(tutorial);

            // Adiciona alguns passos fictícios ao contexto do banco de dados em memória
            context.Steps.AddRange(
                new Step { Id = Guid.NewGuid(), TutorialId = tutorial.Id, Title = "Step 1", Content = "Content 1", Order = 1 },
                new Step { Id = Guid.NewGuid(), TutorialId = tutorial.Id, Title = "Step 2", Content = "Content 2", Order = 2 },
                new Step { Id = Guid.NewGuid(), TutorialId = Guid.NewGuid(), Title = "Another Tutorial Step", Content = "Another Content", Order = 1 } // Um passo de outro tutorial
            );
            await context.SaveChangesAsync();

            var controller = new StepController(context);

            // Act
            var result = await controller.Index(tutorial.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Step>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count); // Verifica se há dois passos para o tutorial especificado
            Assert.All(model, item => Assert.Equal(tutorial.Id, item.TutorialId)); // Verifica se todos os passos pertencem ao tutorial especificado
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesStepAndRedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Adiciona um tutorial e um step fictício ao contexto do banco de dados em memória
            var tutorialId = Guid.NewGuid();
            var stepId = Guid.NewGuid();
            var step = new Step { Id = stepId, TutorialId = tutorialId, Title = "Test Step", Content = "Test Content", Order = 1 };
            context.Steps.Add(step);
            await context.SaveChangesAsync();

            var controller = new StepController(context);

            // Act
            var result = await controller.DeleteConfirmed(stepId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(tutorialId, redirectToActionResult.RouteValues["tutorialId"]);

            // Verifica se o step foi removido do contexto do banco de dados em memória
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

            // Adiciona um tutorial e um step fictício ao contexto do banco de dados em memória
            var tutorialId = Guid.NewGuid();
            var stepId = Guid.NewGuid();
            var originalTitle = "Original Step";
            var step = new Step { Id = stepId, TutorialId = tutorialId, Title = originalTitle, Content = "Original Content", Order = 1 };
            context.Steps.Add(step);
            await context.SaveChangesAsync();

            var controller = new StepController(context);

            // Act
            var result = await controller.Edit(tutorialId);

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

            // Set up a context (connection to the DB)
            using (var context = new ApplicationDbContext(options))
            {
                // Add sample data to the in-memory database
                context.Steps.Add(new Step { Id = stepId, TutorialId = tutorialId, Title = "Title 1", Content = "Content 1", Order = 1 });
                await context.SaveChangesAsync();
            }

            // Set up the controller with the context
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new StepController(context);

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