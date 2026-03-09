using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Controllers;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;
using MiniTwitter.ViewModels;
using Moq;

namespace Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockTokenService = new Mock<ITokenService>();
            _controller = new AuthController(_mockAuthService.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Login_UnknownEmail_ReturnsUnauthorized()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("ghost@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            var model = new LoginRequestDto { Email = "ghost@example.com", Password = "AnyPass1!" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_UnknownEmail_DoesNotCallPasswordCheck()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("ghost@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            var model = new LoginRequestDto { Email = "ghost@example.com", Password = "AnyPass1!" };

            // Act
            await _controller.Login(model);

            // Assert — CheckUserPasswordAsync must never be called with a null user
            _mockAuthService.Verify(
                s => s.CheckUserPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", Email = "real@example.com" };

            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("real@example.com"))
                .ReturnsAsync(user);

            _mockAuthService
                .Setup(s => s.CheckUserPasswordAsync(user, "WrongPass!", true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var model = new LoginRequestDto { Email = "real@example.com", Password = "WrongPass!" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", Email = "real@example.com", UserName = "realuser" };

            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("real@example.com"))
                .ReturnsAsync(user);

            _mockAuthService
                .Setup(s => s.CheckUserPasswordAsync(user, "CorrectPass1!", true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockTokenService
                .Setup(s => s.CreateToken(user))
                .ReturnsAsync("mock.jwt.token");

            var model = new LoginRequestDto { Email = "real@example.com", Password = "CorrectPass1!" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var existingUser = new ApplicationUser { Id = "u1", Email = "taken@example.com" };

            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("taken@example.com"))
                .ReturnsAsync(existingUser);

            var model = new RegisterRequestDto { Email = "taken@example.com", Username = "newuser", Password = "Password1!" };

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_IdentityFailure_ReturnsBadRequest()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("new@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            var identityFailure = Microsoft.AspNetCore.Identity.IdentityResult.Failed(
                new Microsoft.AspNetCore.Identity.IdentityError { Description = "Password too weak." }
            );

            _mockAuthService
                .Setup(s => s.CreateUserAsync(It.IsAny<ApplicationUser>(), "WeakPass"))
                .ReturnsAsync(identityFailure);

            var model = new RegisterRequestDto { Email = "new@example.com", Username = "newuser", Password = "WeakPass" };

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_ValidInput_ReturnsOk()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.FindUserByEmailAsync("fresh@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            _mockAuthService
                .Setup(s => s.CreateUserAsync(It.IsAny<ApplicationUser>(), "Password1!"))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);

            _mockTokenService
                .Setup(s => s.CreateToken(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("mock.jwt.token");

            var model = new RegisterRequestDto { Email = "fresh@example.com", Username = "freshuser", Password = "Password1!" };

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
