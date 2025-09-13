using MiniTwitter.Models;

namespace MiniTwitter.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(ApplicationUser user);
    }
}
