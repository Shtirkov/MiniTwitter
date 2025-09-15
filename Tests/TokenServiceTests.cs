using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniTwitter.Models;
using MiniTwitter.Services;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tests
{
    public class TokenServiceTests
    {
        private readonly TokenService _service;
        private readonly IConfiguration _config;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public TokenServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string?>
        {
            { "Api:Issuer", "http://localhost:5064" },
            { "Api:Audience", "http://localhost:5064" },
            { "Api:SigningKey", "h1gw8d1by3g1d8c13ydcb1n8y3tge168y3gdb1iy3g8163fgr861b3yd163gf163fb8y163fg183gf68" }
        };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            _service = new TokenService(_config, _mockUserManager.Object);
        }

        [Fact]
        public async Task CreateTokenShouldIncludeUserClaims()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1", UserName = "testuser", Email = "test@example.com" };
            _mockUserManager
                .Setup(m => m.GetSecurityStampAsync(user))
                .ReturnsAsync("test-security-stamp");

            // Act
            var token = await _service.CreateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == "u1");
            jwt.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == "testuser");
            jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == "test@example.com");
            jwt.Claims.Should().Contain(c => c.Type == "SecurityStamp" && c.Value == "test-security-stamp");
            jwt.Claims.Should().Contain(c => c.Type == "jti");
        }

        [Fact]
        public async Task CreateTokenShouldContainIssuerAudienceAndExpiry()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u2", UserName = "user2", Email = "user2@example.com" };
            _mockUserManager
                .Setup(m => m.GetSecurityStampAsync(user))
                .ReturnsAsync("stamp-2");

            // Act
            var token = await _service.CreateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Issuer.Should().Be("http://localhost:5064");
            jwt.Audiences.Should().Contain("http://localhost:5064");
            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateTokenShouldBeValidatedWithSigningKey()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u3", UserName = "user3", Email = "user3@example.com" };
            _mockUserManager
                .Setup(m => m.GetSecurityStampAsync(user))
                .ReturnsAsync("stamp-3");

            var token = await _service.CreateToken(user);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Api:Issuer"],
                ValidAudience = _config["Api:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Api:SigningKey"]!))
            };

            var handler = new JwtSecurityTokenHandler();

            // Act
            var principal = handler.ValidateToken(token, validationParams, out var validatedToken);

            // Assert
            principal.Identity!.IsAuthenticated.Should().BeTrue();
            validatedToken.Should().BeOfType<JwtSecurityToken>();
        }
    }
}