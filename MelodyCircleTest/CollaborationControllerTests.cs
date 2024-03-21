using Microsoft.EntityFrameworkCore;
using MelodyCircle.Controllers;
using MelodyCircle.Data;
using Microsoft.AspNetCore.Mvc;
using MelodyCircle.Models;

namespace MelodyCircle.Tests
{
    public class CollaborationControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfCollaborations()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Collaborations.AddRange(new List<Collaboration>
                {
                    new Collaboration { Id = Guid.NewGuid(), Title = "Collab 1" },
                    new Collaboration { Id = Guid.NewGuid(), Title = "Collab 2" },
                    new Collaboration { Id = Guid.NewGuid(), Title = "Collab 3" }
                });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new CollaborationController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Collaboration>>(viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
            }
        }
    }
}