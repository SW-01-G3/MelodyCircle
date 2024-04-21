﻿using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MelodyCircleTest
{
    public class StatControllerTests
	{
		private readonly Mock<ApplicationDbContext> _mockContext;
		private readonly Mock<UserManager<User>> _mockUserManager;
		private readonly StatController _controller;

		public StatControllerTests()
		{
			_mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
			_mockUserManager = new Mock<UserManager<User>>(
		new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
			_controller = new StatController(_mockContext.Object, _mockUserManager.Object);
		}

		[Fact]
		public void UserCreationStats_ReturnsViewResult()
		{
			var result = _controller.UserCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void TutorialCreationStats_ReturnsViewResult()
		{
			var result = _controller.TutorialCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void CollaborationCreationStats_ReturnsViewResult()
		{
			var result = _controller.CollaborationCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void StepCreationStats_ReturnsViewResult()
		{
			var result = _controller.StepCreationStats();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void UserTutorialStats_WithInvalidUserName_RedirectsToIndex()
		{
			var result = _controller.UserTutorialStats("InvalidUser");
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact]
		public void UserCollaborationStats_WithInvalidUserName_RedirectsToIndex()
		{
			var result = _controller.UserCollaborationStats("InvalidUser");
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact]
		public void UserStepStats_WithInvalidUserName_RedirectsToIndex()
		{
			var result = _controller.UserStepStats("InvalidUser");
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

	}
}
