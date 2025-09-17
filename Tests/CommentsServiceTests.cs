using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiniTwitter;
using MiniTwitter.Entities;
using MiniTwitter.Models;
using MiniTwitter.RequestModels;

namespace Tests
{
    public class CommentsServiceTests
    {
        private readonly TwitterContext _context;
        private readonly CommentsService _service;

        public CommentsServiceTests()
        {
            var options = new DbContextOptionsBuilder<TwitterContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new TwitterContext(options);
            _service = new CommentsService(_context);

            SeedData();
        }

        [Fact]
        public async Task AddAsyncShouldAddComment()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var post = await _context.Posts.FindAsync(1);
            var comment = new Comment { Id = 99, AuthorId = user!.Id, PostId = post!.Id, Content = "New Comment" };

            // Act
            await _service.AddAsync(comment);
            await _service.SaveChangesAsync();

            // Assert
            var exists = await _context.Comments.AnyAsync(c => c.Id == 99);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task EditShouldUpdateCommentContent()
        {
            // Arrange
            var comment = await _context.Comments.FindAsync(1);
            var dto = new EditCommentRequestDto { Content = "Edited Content" };

            // Act
            var result = _service.Edit(comment!, dto);
            await _service.SaveChangesAsync();

            // Assert
            result.Content.Should().Be("Edited Content");

            var updated = await _context.Comments.FindAsync(1);
            updated!.Content.Should().Be("Edited Content");
        }

        [Fact]
        public async Task GetAsyncShouldReturnCommentWithAuthor()
        {
            // Arrange
            var existingId = 1;

            // Act
            var result = await _service.GetAsync(existingId);

            // Assert
            result.Should().NotBeNull();
            result!.Author.Should().NotBeNull();
            result.Author!.Id.Should().Be("u1");
        }

        [Fact]
        public async Task GetAsyncShouldReturnNullWhenCommentDoesNotExist()
        {
            // Arrange
            var nonExistentId = 12345;

            // Act
            var result = await _service.GetAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveShouldDeleteComment()
        {
            // Arrange
            var comment = await _context.Comments.FindAsync(1);

            // Act
            _service.Remove(comment!);
            await _service.SaveChangesAsync();

            // Assert
            var exists = await _context.Comments.AnyAsync(c => c.Id == 1);
            exists.Should().BeFalse();
        }

        private void SeedData()
        {
            var user1 = new ApplicationUser { Id = "u1", UserName = "User1" };
            var post1 = new Post { Id = 1, AuthorId = "u1", Content = "Hello World", CreatedAt = DateTime.UtcNow };

            _context.Users.Add(user1);
            _context.Posts.Add(post1);

            _context.Comments.AddRange(
                new Comment { Id = 1, AuthorId = "u1", Content = "First comment", PostId = 1 },
                new Comment { Id = 2, AuthorId = "u1", Content = "Second comment", PostId = 1 }
            );

            _context.SaveChanges();
        }
    }
}
