using Microsoft.EntityFrameworkCore;
using MiniTwitter;
using MiniTwitter.Entities;
using MiniTwitter.Services;

namespace Tests
{
    public class PostsServiceTests
    {
        private readonly TwitterContext _context;
        private readonly PostsService _service;

        public PostsServiceTests()
        {
            var options = new DbContextOptionsBuilder<TwitterContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TwitterContext(options);
            _service = new PostsService(_context);
        }

        [Fact]
        public async Task CreatePostShouldAddPost()
        {
            //Arrange
            var post = new Post
            {
                AuthorId = "TestUser",
                Content = "Test content",
                Author = null
            };

            //Act
            await _service.AddAsync(post);
            await _service.SaveChangesAsync();

            //Assert
            var posts = await _context.Posts.ToListAsync();
            Assert.True(posts.Count == 1);
            Assert.Equal("Test content", posts[0].Content);
        }
    }
}
