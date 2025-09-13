using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Interfaces;
using MiniTwitter.Mappers;
using MiniTwitter.Models;
using MiniTwitter.ViewModels;

namespace MiniTwitter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _authService.FindUserByEmailAsync(model.Email);
            if (existing != null)
            {
                return BadRequest(new { message = "Email already in use." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _authService.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = _tokenService.CreateToken(user);
                return Ok(user.ToRegisterDto(token));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.FindUserByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var passwordCheck = await _authService.CheckUserPasswordAsync(user, model.Password, true);

            if (!passwordCheck.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            await _authService.SignInAsync(user, false);

            var token = _tokenService.CreateToken(user);

            return Ok(user.ToLoginDto(token));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (!_authService.IsSignedIn(User))
            {
                return Unauthorized(new { message = "User not signed in" });
            }

            await _authService.SignOutAsync();
            return Ok(new { message = "Signed out!" });
        }
    }
}
