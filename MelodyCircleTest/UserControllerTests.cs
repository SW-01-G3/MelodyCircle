using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace MelodyCircleTest
{
    /* Guilherme Bernardino, Rodrigo Nogueira */
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

            // Act
            var result = await _controller.Profile("TestUser");

            // Assert
            Assert.Equal(viewModel.User.UserName, user.UserName);
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

        [Fact]
        public async Task AddMusicCard_WithValidInput_ShouldAddMusicAndRedirectToProfile()
        {
            // Arrange
            var user = new User { UserName = "TestUser", MusicURI = new List<string>() {} };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var uri = "https://open.spotify.com/track/3ZV60LVhqQ2uHXG6LL6F0V?si=ee2c438186cc4492"; // Exemplo de URI válido
            var regex = new Regex(@"\/track\/(\w+)");
            var match = regex.Match(uri);
            var id = "TestUser";
            // Act
            var result = await _controller.AddMusicCard(id, uri);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal(id, redirectResult.RouteValues["id"]);
            Assert.Contains(match.ToString(), user.MusicURI.ToList());
        }

        [Fact]
        public async Task RemoveMusicCard_WithValidInput_ShouldRemoveMusicAndRedirectToProfile()
        {
            // Arrange
            var user = new User { UserName = "TestUser", MusicURI = new List<string> { "https://open.spotify.com/track/3ZV60LVhqQ2uHXG6LL6F0V?si=ee2c438186cc4492" } };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var uri = "https://open.spotify.com/track/3ZV60LVhqQ2uHXG6LL6F0V?si=ee2c438186cc4492"; // Exemplo de URI válido
            var id = "TestUser";

            // Act
            var result = await _controller.RemoveMusicCard(uri);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal(id, redirectResult.RouteValues["id"]);
            Assert.DoesNotContain(uri, user.MusicURI);
        }

        [Fact]
        public async Task EditMusicCard_WithValidInput_ShouldEditMusicAndRedirectToProfile()
        {
            // Arrange
            var user = new User { UserName = "TestUser", MusicURI = new List<string> { "/track/3ZV60LVhqQ2uHXG6LL6F0V?si=ee2c438186cc4492" } };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var oldUri = "https://open.spotify.com/track/3ZV60LVhqQ2uHXG6LL6F0V?si=ee2c438186cc4492";
            var newUri = "https://open.spotify.com/track/3ZV60LVhqQ2uHXG6FL6F0V?si=ee2c438186cc4492";

            var id = "TestUser";

            // Act
            var result = await _controller.EditMusicCard(oldUri, newUri);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            Assert.Equal(id, redirectResult.RouteValues["id"]);
            Assert.DoesNotContain(oldUri, user.MusicURI);
            Assert.Contains("/track/3ZV60LVhqQ2uHXG6LL6F0V?si=ee2c438186cc4492", user.MusicURI);
        }
    }
}