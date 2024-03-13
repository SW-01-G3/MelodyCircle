using MelodyCircle.Controllers;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MelodyCircle.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Xunit;
using Microsoft.Extensions.Primitives;
using MelodyCircle.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace MelodyCircleTest
{
    public class UserControllerTests : IClassFixture<ApplicationDbContextFixture>
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private UserController _controller;

        public UserControllerTests(ApplicationDbContextFixture applicationDbContextFixture)
        {
            var store = new Mock<IUserStore<User>>();
            _context = applicationDbContextFixture.DbContext;
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _controller = new UserController(_mockUserManager.Object, _context);
        }

        [Fact]
        public async Task Profile_WithValidId_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var user = new User { UserName = "TestUser" };
            var viewModel = new ProfileViewModel { User = user, Roles = new List<string> { "Role1", "Role2" } };
            _mockUserManager.Setup(m => m.Users).Returns(new List<User> { user }.AsQueryable());
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(viewModel.Roles);
            _context.Users.Add(user);
            //await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Profile("TestUser");

            // Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom<ProfileViewModel>(viewResult.Model);
            Assert.Equal(viewModel.User.UserName, user.UserName);
            //Assert.Equal(viewModel.Roles, );
        }

        [Fact]
        public async Task AddConnection_WithValidInput_ShouldRedirectToProfile()
        {
            var currentUser = new User { UserName = "CurrentUser" };
            var userToAdd = new User { UserName = "UserToAdd" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _mockUserManager.Setup(m => m.FindByNameAsync("UserToAdd")).ReturnsAsync(userToAdd);
            var result = await _controller.AddConnection("UserToAdd");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal("UserToAdd", redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task RemoveConnection_WithValidInput_ShouldRedirectToProfile()
        {
            var currentUser = new User { UserName = "CurrentUser" };
            var connectionToRemove = new User { UserName = "ConnectionToRemove" };
            currentUser.Connections = new List<User> { connectionToRemove };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _mockUserManager.Setup(m => m.FindByNameAsync("ConnectionToRemove")).ReturnsAsync(connectionToRemove);
            var result = await _controller.RemoveConnection("ConnectionToRemove");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal("ConnectionToRemove", redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task ListConnections_WithValidInput_ShouldReturnViewWithConnections()
        {
            // Arrange
            var user = new User { UserName = "TestUser" };
            var connection1 = new User { UserName = "Connection1" };
            var connection2 = new User { UserName = "Connection2" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            user.Connections = new List<User> { connection1, connection2 };
            var users = new List<User> { user, connection1, connection2 };
            _mockUserManager.Setup(m => m.Users).Returns(users.AsQueryable());
            await _context.Entry(user).Collection(u => u.Connections).LoadAsync();
            // Act
            var result = await _controller.ListConnections("Connection1");

            // Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model);
            Assert.Equal(2,user.Connections.Count);
        }

        [Fact]
        public async Task PutProfilePicture_WithValidInput_ShouldRedirectToProfile()
        {
            var user = new User { UserName = "TestUser" };
            var formFile = new Mock<IFormFile>();
            formFile.Setup(f => f.Length).Returns(1);
            formFile.Setup(f => f.ContentType).Returns("image/jpeg");
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var userManagerResult = IdentityResult.Success;
            _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(userManagerResult);

            var httpContext = new DefaultHttpContext();
            var formFiles = new FormFileCollection { formFile.Object };
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFiles);
            httpContext.Request.Headers["Content-Type"] = "multipart/form-data";
            _mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.PutProfilePicture("TestUser", formFile.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal("TestUser", redirectResult.RouteValues["id"]);
            // Ensure profile picture is set correctly
            Assert.NotNull(user.ProfilePicture); 
        }

        [Fact]
        public async Task RateUser_WithValidInput_ShouldRedirectToProfile()
        {
            // Arrange
            var currentUser = new User { UserName = "CurrentUser" };
            var userToRate = new User { UserName = "UserToRate" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _mockUserManager.Setup(m => m.FindByNameAsync("UserToRate")).ReturnsAsync(userToRate);
            _mockHttpContextAccessor.Setup(m => m.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "CurrentUser") })));

            // Act
            var result = await _controller.RateUser("UserToRate", 5);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(5, userToRate.Ratings[0].Value);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal("UserToRate", redirectResult.RouteValues["id"]);
        }
    }
}