using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class CollaborationControllerTests
{
	private readonly CollaborationController _controller;
	private readonly Mock<UserManager<User>> _mockUserManager;
	private readonly ApplicationDbContext _context;

	public CollaborationControllerTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "TestDb")
			.Options;
		_context = new ApplicationDbContext(options);

		var store = new Mock<IUserStore<User>>();
		_mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

		_controller = new CollaborationController(_context, _mockUserManager.Object, null, null);
	}

	[Fact]
	public async Task Index_RedirectsToEditMode()
	{
		var result = await _controller.Index();
		Assert.IsType<RedirectToActionResult>(result);
	}

	[Fact]
	public async Task EditMode_ReturnsViewResult()
	{
		var result = await _controller.EditMode();
		Assert.IsType<ViewResult>(result);
	}

	[Fact]
	public async Task ViewMode_ReturnsViewResult()
	{
		var result = await _controller.ViewMode();
		Assert.IsType<ViewResult>(result);
	}

	[Fact]
	public async Task WaitingList_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.WaitingList(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task JoinQueue_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.JoinQueue(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task JoinQueueConfirm_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.JoinQueueConfirm(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task AllowUser_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.AllowUser(Guid.NewGuid(), "");
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task ContributingUsers_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.ContributingUsers(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task RemoveUser_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.RemoveUser(Guid.NewGuid(), "");
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task InviteToCollab_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.InviteToCollab(Guid.NewGuid(), "");
		Assert.IsType<NotFoundResult>(result);
	}

	
	[Fact]
	public async Task Create_WithValidData_RedirectsToIndex()
	{
		_controller.ModelState.Clear();


		var collaboration = new Collaboration {};
		var result = await _controller.Create(collaboration, null);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
	}

	[Fact]
	public async Task Edit_WhenIdIsNull_ReturnsNotFound()
	{
		var result = await _controller.Edit(null);
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task Delete_WhenIdIsNull_ReturnsNotFound()
	{
		var result = await _controller.Delete(null);
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task DeleteConfirmed_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.DeleteConfirmed(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task Finish_WhenIdIsNull_ReturnsNotFound()
	{
		var result = await _controller.Finish(null);
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task FinishConfirmed_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.FinishConfirmed(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task RateCollaboration_WithInvalidRating_ReturnsBadRequest()
	{
		var result = await _controller.RateCollaboration(Guid.NewGuid(), 11);
		Assert.IsType<BadRequestObjectResult>(result);
	}

	[Fact]
	public async Task ArrangementPanel_WhenCollaborationNotFound_ReturnsNotFound()
	{
		var result = await _controller.ArrangementPanel(Guid.NewGuid());
		Assert.IsType<NotFoundResult>(result);
	}
}

