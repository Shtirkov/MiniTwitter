using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiniTwitter;
using MiniTwitter.Entities;
using MiniTwitter.Helpers;
using MiniTwitter.Models;
using MiniTwitter.RequestModels;
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

            SeedData();
        }

        [Fact]
        public async Task AddShouldAddPost()
        {
            //Arrange
            var post = new Post
            {
                Id = 0,
                AuthorId = "-5",
                Content = "test",
                CreatedAt = DateTime.Now
            };

            //Act
            await _service.AddAsync(post);
            await _service.SaveChangesAsync();

            //Assert
            var posts = await _context.Posts.ToListAsync();
            posts.Count.Should().Be(4);
        }

        [Fact]
        public async Task EditShouldEditPost()
        {
            //Arrange
            var post = await _service.GetPostAsync(1);

            var editPostDto = new EditPostRequestDto
            {
                Content = "Edited"
            };

            //Act
            _service.Edit(post!, editPostDto);
            await _service.SaveChangesAsync();

            //Assert
           editPostDto.Content.Should().Be(post!.Content);
        }

        [Fact]
        public async Task RemoveShouldDeletePost()
        {
            //Arrange
            var post = await _context.Posts.FindAsync(1);

            //Act
            _service.Remove(post!);
            await _service.SaveChangesAsync();

            //Assert
            var posts = await _context.Posts.ToListAsync();
            posts.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetShouldReturnPost()
        {
            //Arrange
            var expected = await _context.Posts.FindAsync(1);

            //Act
            var actual = await _service.GetPostAsync(1);

            //Assert
            actual!.Id.Should().Be(expected!.Id);
            actual.Content.Should().Be(expected.Content);

        }

        [Fact]
        public async Task GetFriendsPostsShouldReturnOnlyFriendsPosts()
        {
            //Arrange
            var user = await _context.Users.FindAsync("u1");

            var friends = new List<Friendship>
            {
                new Friendship { UserId = "u1", FriendId = "u2" }
            };

            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetFriendsPosts(user!, friends, queryParams);

            result.TotalCount.Should().Be(1);
            result.Items[0].AuthorId.Should().Be("u2");
        }

        [Fact]
        public async Task GetFriendsPostsShouldBeOrderedByCreatedAtDescending()
        {
            //Arrange
            var user = await _context.Users.FindAsync("u1");

            var friends = new List<Friendship>
            {
                new Friendship { UserId = "u1", FriendId = "u2" },
                new Friendship { UserId = "u1", FriendId = "u3" }
            };

            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            //Act
            var result = await _service.GetFriendsPosts(user!, friends, queryParams);

            //Assert
            var ordered = result.Items.OrderByDescending(p => p.CreatedAt).ToList();

            for (var i = 0; i < result.TotalCount; i++)
            {
                var expected = ordered[i];
                var actual = result.Items[i];

                expected.Should().BeEquivalentTo(actual);
            }
        }

        [Fact]
        public async Task GetFriendsPostsShouldPageTheResults()
        {
            //Arrange
            var user = await _context.Users.FindAsync("u1");

            var friends = new List<Friendship>
            {
                new Friendship { UserId = "u1", FriendId = "u2" },
                new Friendship { UserId = "u1", FriendId = "u3" }
            };

            var queryParams = new QueryParams { Page = 2, PageSize = 1 };

            //Act
            var result = await _service.GetFriendsPosts(user!, friends, queryParams);

            //Assert
            result.Page.Should().Be(2);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(2);
        }

        [Fact]
        public async Task GetFriendsPostsShouldLoadCommentsWithAuthor()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var friend = await _context.Users.FindAsync("u2");

            var post = new Post { Id = 99, AuthorId = friend!.Id, Content = "Friend's post", CreatedAt = DateTime.UtcNow };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var comment = new Comment
            {
                Id = 100,
                AuthorId = user!.Id,
                Author = user,
                Content = "Test comment",
                PostId = post.Id
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var friends = new List<Friendship>
            {
                new Friendship { UserId = user.Id, FriendId = friend.Id }
            };

            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetFriendsPosts(user, friends, queryParams);

            // Assert
            var loadedPost = result.Items.First();
            loadedPost.Comments.Count.Should().Be(1);
            loadedPost.Comments.First().Content.Should().Be("Test comment");
            loadedPost.Comments.First().Author.Should().NotBeNull();
            loadedPost.Comments.First().Author!.Id.Should().Be(user.Id);
        }


        [Fact]
        public async Task GetFriendsPostsShouldReturnEmptyWhenUserHasNoFriends()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var friends = new List<Friendship>();
            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetFriendsPosts(user!, friends, queryParams);

            // Assert
            result.TotalCount.Should().Be(0);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFriendsPostsShouldReturnEmptyWhenFriendsHaveNoPosts()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");

            var friend = await _context.Users.FindAsync("u3");
            var postsByFriend = _context.Posts.Where(p => p.AuthorId == friend!.Id);
            _context.Posts.RemoveRange(postsByFriend);
            await _context.SaveChangesAsync();

            var friends = new List<Friendship>
            {
                new Friendship { UserId = "u1", FriendId = "u3" }
            };

            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetFriendsPosts(user!, friends, queryParams);

            // Assert
            result.TotalCount.Should().Be(0);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFriendsPosts_ShouldReturnAllCommentsForPost()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var friend = await _context.Users.FindAsync("u2");

            var post = new Post
            {
                Id = 200,
                AuthorId = friend!.Id,
                Content = "Friend's post with comments",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var comments = new List<Comment>
            {
                new Comment { Id = 201, AuthorId = user!.Id, Author = user, Content = "First comment", PostId = post.Id },
                new Comment { Id = 202, AuthorId = user.Id, Author = user, Content = "Second comment", PostId = post.Id }
            };

            await _context.Comments.AddRangeAsync(comments);
            await _context.SaveChangesAsync();

            var friends = new List<Friendship>
            {
                new Friendship { UserId = user.Id, FriendId = friend.Id }
            };

            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetFriendsPosts(user, friends, queryParams);

            // Assert
            var loadedPost = result.Items.First();
            loadedPost.Comments.Count.Should().Be(2);
            loadedPost.Comments.Select(c => c.Content).Should().Contain(new[] { "First comment", "Second comment" });
        }

        [Fact]
        public async Task GetPostsByUserReturnAllPostsByUser()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetPostsByUserAsync(user!.UserName!, queryParams);

            // Assert
            result.TotalCount.Should().Be(1);

            foreach (var item in result.Items)
            {
                item.AuthorId.Should().Be(user.Id);
            }
        }

        [Fact]
        public async Task GetPostsByUserShouldBeOrderedByCreatedAtDescending()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var queryParams = new QueryParams { Page = 1, PageSize = 10 };

            var post = new Post
            {
                Id = 99,
                AuthorId = user!.Id,
                Content = "Test",
                CreatedAt = DateTime.Now.AddMinutes(-60)
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPostsByUserAsync(user!.UserName!, queryParams);

            // Assert
            result.TotalCount.Should().Be(2);

            var ordered = result.Items.OrderByDescending(p => p.CreatedAt).ToList();

            for (var i = 0; i < result.TotalCount; i++)
            {
                var expected = ordered[i];
                var actual = result.Items[i];

                expected.Should().BeEquivalentTo(actual);
            }
        }

        [Fact]
        public async Task GetPostsByUserShouldPageTheResulsts()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var queryParams = new QueryParams { Page = 2, PageSize = 1 };

            var post = new Post
            {
                Id = 99,
                AuthorId = user!.Id,
                Content = "Test",
                CreatedAt = DateTime.Now.AddMinutes(-60)
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPostsByUserAsync(user!.UserName!, queryParams);

            // Assert
            result.Page.Should().Be(2);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(2);            
        }

        [Fact]
        public async Task LikeShouldIncrementLikesCount()
        {
            // Arrange
            var post = await _context.Posts.FindAsync(1);
            var user = await _context.Users.FindAsync("u1");

            //Act
            await _service.Like(post!, user!);
            await _context.SaveChangesAsync();

            //Assert
            post!.TotalLikes.Should().Be(1);
            var likes = await _context.Likes.ToListAsync();
            likes.Should().HaveCount(1);

            //Act
            await _service.Like(post!, user!);
            await _context.SaveChangesAsync();

            //Assert
            post!.TotalLikes.Should().Be(0);
            likes = await _context.Likes.ToListAsync();
            likes.Should().HaveCount(0);
        }

        private void SeedData()
        {
            _context.Users.AddRange(
                new ApplicationUser { Id = "u1", UserName = "User1" },
                new ApplicationUser { Id = "u2", UserName = "User2" },
                new ApplicationUser { Id = "u3", UserName = "User3" }
            );

            _context.Posts.AddRange(
                new Post { Id = 1, AuthorId = "u2", Content = "Post A", CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
                new Post { Id = 2, AuthorId = "u3", Content = "Post B", CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
                new Post { Id = 3, AuthorId = "u1", Content = "My own post", CreatedAt = DateTime.UtcNow }
            );

            _context.SaveChanges();
        }
    }
}