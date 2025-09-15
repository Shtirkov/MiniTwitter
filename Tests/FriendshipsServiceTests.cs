using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiniTwitter;
using MiniTwitter.Models;
using MiniTwitter.Services;

namespace Tests
{
    public class FriendshipsServiceTests
    {
        private readonly TwitterContext _context;
        private readonly FriendshipsService _service;

        public FriendshipsServiceTests()
        {
            var options = new DbContextOptionsBuilder<TwitterContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new TwitterContext(options);
            _service = new FriendshipsService(_context, null!);

            SeedData();
        }

        [Fact]
        public async Task AddAsyncShouldAddFriendship()
        {
            // Arrange
            var friendship = new Friendship { UserId = "u1", FriendId = "u3", IsConfirmed = false };

            // Act
            await _service.AddAsync(friendship);
            await _service.SaveChangesAsync();

            // Assert
            (await _context.Friendships.AnyAsync(f => f.UserId == "u1" && f.FriendId == "u3"))
                .Should().BeTrue();
        }

        [Fact]
        public async Task CheckForPendingFriendRequestShouldReturnPendingRequest()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u3");
            var friend = await _context.Users.FindAsync("u2");

            // Act
            var pending = await _service.CheckForPendingFriendRequest(user!, friend!);

            // Assert
            pending.Should().NotBeNull();
            pending!.UserId.Should().Be("u2");
            pending.FriendId.Should().Be("u3");
            pending.IsConfirmed.Should().BeFalse();
        }

        [Fact]
        public async Task CheckForPendingFriendRequestShouldReturnNullWhenNoPending()
        {
            // Arrange
            var user = await _context.Users.FindAsync("u1");
            var friend = await _context.Users.FindAsync("u2");

            // Act
            var pending = await _service.CheckForPendingFriendRequest(user!, friend!);

            // Assert
            pending.Should().BeNull();
        }

        [Fact]
        public async Task CheckIfFriendshipIsRequestedAsyncShouldReturnTrueWhenExists()
        {
            // Arrange
            var u1 = await _context.Users.FindAsync("u1");
            var u2 = await _context.Users.FindAsync("u2");

            // Act
            var result = await _service.CheckIfFriendshipIsRequestedAsync(u1!, u2!);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIfFriendshipIsRequestedAsyncShouldReturnFalseWhenNotExists()
        {
            // Arrange
            var u1 = await _context.Users.FindAsync("u1");
            var u3 = await _context.Users.FindAsync("u3");

            // Act
            var result = await _service.CheckIfFriendshipIsRequestedAsync(u1!, u3!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CheckIfUsersAreFriendsAsyncShouldReturnTrueWhenConfirmed()
        {
            // Arrange
            var u1 = await _context.Users.FindAsync("u1");
            var u2 = await _context.Users.FindAsync("u2");

            // Act
            var result = await _service.CheckIfUsersAreFriendsAsync(u1!, u2!);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIfUsersAreFriendsAsyncShouldReturnFalseWhenPending()
        {
            // Arrange
            var u2 = await _context.Users.FindAsync("u2");
            var u3 = await _context.Users.FindAsync("u3");

            // Act
            var result = await _service.CheckIfUsersAreFriendsAsync(u2!, u3!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CheckIfUsersAreFriendsAsyncShouldReturnTrueWhenFriendshipIsBidirectional()
        {
            // Arrange
            var u1 = await _context.Users.FindAsync("u1");
            var u2 = await _context.Users.FindAsync("u2");

            var friendship = new Friendship { UserId = "u2", FriendId = "u1", IsConfirmed = true };
            await _service.AddAsync(friendship);
            await _service.SaveChangesAsync();

            // Act
            var result1 = await _service.CheckIfUsersAreFriendsAsync(u1!, u2!);
            var result2 = await _service.CheckIfUsersAreFriendsAsync(u2!, u1!);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Fact]
        public async Task GetFriendsAsyncShouldReturnFriends_WhenConfirmed()
        {
            // Arrange
            var u1 = await _context.Users.FindAsync("u1");

            // Act
            var friends = await _service.GetFriendsAsync(u1!);

            // Assert
            friends.Should().ContainSingle();
            friends.First().FriendId.Should().Be("u2");
        }

        [Fact]
        public async Task GetFriendsAsyncShouldReturnEmptyWhenNoFriends()
        {
            // Arrange
            var u3 = await _context.Users.FindAsync("u3");

            // Act
            var friends = await _service.GetFriendsAsync(u3!);

            // Assert
            friends.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveShouldRemoveFriendship()
        {
            // Arrange
            var friendship = await _context.Friendships.FirstAsync(f => f.UserId == "u1" && f.FriendId == "u2");

            // Act
            _service.Remove(friendship);
            await _service.SaveChangesAsync();

            // Assert
            (await _context.Friendships.AnyAsync(f => f.UserId == "u1" && f.FriendId == "u2"))
                .Should().BeFalse();
        }

        [Fact]
        public async Task AddAsyncShouldAllowSelfFriendshipButWeMightForbid()
        {
            // Arrange
            var selfFriendship = new Friendship { UserId = "u1", FriendId = "u1", IsConfirmed = true };

            // Act
            await _service.AddAsync(selfFriendship);
            await _service.SaveChangesAsync();

            // Assert
            (await _context.Friendships.AnyAsync(f => f.UserId == "u1" && f.FriendId == "u1"))
                .Should().BeTrue();
        }

        private void SeedData()
        {
            var u1 = new ApplicationUser { Id = "u1", UserName = "User1" };
            var u2 = new ApplicationUser { Id = "u2", UserName = "User2" };
            var u3 = new ApplicationUser { Id = "u3", UserName = "User3" };

            _context.Users.AddRange(u1, u2, u3);

            _context.Friendships.AddRange(
                new Friendship { UserId = "u1", FriendId = "u2", IsConfirmed = true },
                new Friendship { UserId = "u2", FriendId = "u3", IsConfirmed = false } // pending
            );

            _context.SaveChanges();
        }
    }
}