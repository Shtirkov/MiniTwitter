using Microsoft.AspNetCore.Identity;
using MiniTwitter.Models;
using System.Security.Claims;

namespace MiniTwitter.Interfaces
{
    public interface IAuthService
    {
        public Task<ApplicationUser?> FindUserByEmailAsync(string email);

        public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);

        public Task<SignInResult> CheckUserPasswordAsync(ApplicationUser user, string password, bool lockoutOnFailure);

        public Task SignInAsync(ApplicationUser user, bool isPersistent);

        public bool IsSignedIn(ClaimsPrincipal principal);

        public Task SignOutAsync(ApplicationUser user);

        public Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);

        public Task<ApplicationUser?> FindUserByUsernameAsync(string username);
    }
}
