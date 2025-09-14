using MiniTwitter.Models;

namespace MiniTwitter.Interfaces
{
    public interface ITokenService
    {
        public Task<string> CreateToken(ApplicationUser user);
    }
}
