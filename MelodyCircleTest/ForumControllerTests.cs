﻿using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using SendGrid.Helpers.Mail;


namespace MelodyCircleTest
{
	public class ForumControllerTests : IClassFixture<ApplicationDbContextFixture>
	{
		private ApplicationDbContext _context;
		private ForumController _controller;

		public ForumControllerTests(ApplicationDbContextFixture applicationDbContextFixture)
		{
			_context = applicationDbContextFixture.DbContext;
			_controller = new ForumController(_context);
		}

		//[Fact]
		//public async Task Index_ReturnsViewResult_WithAListOfForumPosts()
		//{
		//	var mockSet = new Mock<DbSet<ForumPost>>();
		//	_mockContext.Setup(m => m.ForumPost).Returns(mockSet.Object);
		//	var result = await _controller.Posts();
		//	var viewResult = Assert.IsType<ViewResult>(result);
		//	Assert.IsAssignableFrom<IEnumerable<ForumPost>>(viewResult.Model);
		//}

		[Fact]
		public void Create_Get_ReturnsViewResult()
		{
			var result = _controller.Create();
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task Create_PostValidModel_RedirectsToIndex()
		{
			var result = await _controller.Create(new ForumPost { Title = "New Post", Content = "Content" });
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact]
		public async Task Edit_Get_WhenIdIsNull_ReturnsNotFoundResult()
		{
			var result = await _controller.Edit(id: null);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Edit_Get_WhenIdIsValid_ReturnsViewResult()
		{
			Guid id = Guid.NewGuid();

			var fp = new ForumPost { Id = id, Comments = new List<Comment>(), IsClosed = false, Content = "adsa", Title = "asdad" };
			_context.ForumPost.Add(fp);
			_context.SaveChangesAsync();

			var result = await _controller.Edit(fp.Id);
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task Edit_Post_WhenModelStateIsValid_RedirectsToIndex()
		{
			Guid id = Guid.NewGuid();

			var fp = new ForumPost { Id = id, Comments = new List<Comment>(), IsClosed = false, Content = "adsa", Title = "asdad" };
			_context.ForumPost.Add(fp);
			_context.SaveChangesAsync();

			var result = await _controller.Edit(fp.Id, new ForumPost());
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task Delete_WhenIdIsNull_ReturnsNotFoundResult()
		{
			var result = await _controller.Delete(id: null);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Delete_WhenIdIsValid_ReturnsViewResult()
		{
			Guid id = Guid.NewGuid();

			var fp = new ForumPost { Id = id, Comments = new List<Comment>(), IsClosed = false, Content = "adsa", Title = "asdad"};
			_context.ForumPost.Add(fp);
			_context.SaveChangesAsync();

			var result = await _controller.Delete(fp.Id);
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task DeleteConfirmed_WhenCalled_RedirectsToIndex()
		{
			Guid id = Guid.NewGuid();

			var fp = new ForumPost { Id = id, Comments = new List<Comment>(), IsClosed = false, Content = "adsa", Title = "asdad" };
			_context.ForumPost.Add(fp);
			_context.SaveChangesAsync();

			var result = await _controller.DeleteConfirmed(fp.Id);
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		private static Mock<DbSet<T>> CreateMockSet<T>(List<T> sourceList) where T : class
		{
			var queryable = sourceList.AsQueryable();
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
			return mockSet;
		}
	}
}
