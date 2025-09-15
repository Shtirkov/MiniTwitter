using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MiniTwitter.Models;
using MiniTwitter.Services;
using System.Security.Claims;

namespace Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null
            );

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null
            );

            _service = new AuthService(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task CheckUserPasswordAsyncShouldReturnSignInResult()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", Email = "test@example.com" };
            _mockSignInManager
                .Setup(s => s.CheckPasswordSignInAsync(user, "Password123", false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _service.CheckUserPasswordAsync(user, "Password123", false);

            // Assert
            result.Should().Be(SignInResult.Success);
        }

        [Fact]
        public async Task CreateUserAsyncShouldReturnIdentityResult()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", Email = "new@example.com" };
            _mockUserManager
                .Setup(m => m.CreateAsync(user, "Password123"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.CreateUserAsync(user, "Password123");

            // Assert
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task FindUserByEmailAsyncShouldReturnUser()
        {
            // Arrange
            var expectedUser = new ApplicationUser { Id = "u1", Email = "find@example.com" };
            _mockUserManager
                .Setup(m => m.FindByEmailAsync("find@example.com"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.FindUserByEmailAsync("find@example.com");

            // Assert
            result.Should().Be(expectedUser);
        }

        [Fact]
        public async Task GetUserAsyncShouldReturnUserFromClaimsPrincipal()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            var expectedUser = new ApplicationUser { Id = "u1", Email = "claims@example.com" };
            _mockUserManager
                .Setup(m => m.GetUserAsync(principal))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserAsync(principal);

            // Assert
            result.Should().Be(expectedUser);
        }

        [Fact]
        public void IsSignedInShouldReturnTrueWhenSignedIn()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            _mockSignInManager
                .Setup(s => s.IsSignedIn(principal))
                .Returns(true);

            // Act
            var result = _service.IsSignedIn(principal);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SignInAsyncShouldCallSignInManager()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", Email = "signin@example.com" };

            // Act
            await _service.SignInAsync(user, false);

            // Assert
            _mockSignInManager.Verify(s => s.SignInAsync(user, false, null), Times.Once);
        }

        [Fact]
        public async Task SignOutAsyncShouldUpdateSecurityStamp()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", Email = "signout@example.com" };

            // Act
            await _service.SignOutAsync(user);

            // Assert
            _mockUserManager.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once);
        }

        [Fact]
        public async Task FindUserByUsernameAsyncShouldReturnUser()
        {
            // Arrange
            var expectedUser = new ApplicationUser { Id = "u1", UserName = "testuser" };
            _mockUserManager
                .Setup(m => m.FindByNameAsync("testuser"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.FindUserByUsernameAsync("testuser");

            // Assert
            result.Should().Be(expectedUser);
        }
    }
}
