using MelodyCircle.Controllers;
using MelodyCircle.Data;
using MelodyCircle.Models;
using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MelodyCircleTest
{
    /* Guilherme Bernardino, Rodrigo Nogueira */
    public class SearchControllerTests : IClassFixture<ApplicationDbContextFixture>
    {
        private ApplicationDbContext _context;
        private SearchController _controller;

        public SearchControllerTests(ApplicationDbContextFixture applicationDbContextFixture)
        {
            _context = applicationDbContextFixture.DbContext;
            _controller = new SearchController(_context);
        }

        [Fact]
        public async Task Search_WithSearchTermAndNoFilter_ShouldReturnSearchResultsView()
        {
            // Arrange
            var search = new Search { SearchTerm = "Test", SearchType = SearchType.None };

            // Act
            var result = await _controller.Search(search);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("SearchResults", viewResult.ViewName);
            Assert.IsAssignableFrom<SearchResultViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Search_WithSearchTermAndUserFilter_ShouldReturnUserSearchResultView()
        {
            // Arrange
            var search = new Search { SearchTerm = "Test", SearchType = SearchType.User };

            // Act
            var result = await _controller.Search(search);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("UserSearchResult", viewResult.ViewName);
            Assert.IsType<List<User>>(viewResult.Model);
        }

        [Fact]
        public async Task Search_WithSearchTermAndTutorialFilter_ShouldReturnTutorialSearchResultView()
        {
            // Arrange
            var search = new Search { SearchTerm = "Test", SearchType = SearchType.Tutorial };

            // Act
            var result = await _controller.Search(search);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("TutorialSearchResult", viewResult.ViewName);
            Assert.IsType<List<Tutorial>>(viewResult.Model);
        }

        [Fact]
        public async Task Search_WithSearchTermAndCollaborationFilter_ShouldReturnCollaborationSearchResultView()
        {
            // Arrange
            var search = new Search { SearchTerm = "Test", SearchType = SearchType.Collaboration };

            // Act
            var result = await _controller.Search(search);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CollaborationSearchResult", viewResult.ViewName);
            Assert.IsType<List<Collaboration>>(viewResult.Model);
        }

        [Fact]
        public async Task Search_WithNoSearchTerm_ShouldReturnIndexView()
        {
            // Arrange
            var search = new Search { SearchTerm = "", SearchType = SearchType.None };

            // Act
            var result = await _controller.Search(search);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }

    }
}
