using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Mappers
{
    public static class LoginMapper
    {
        public static LoginResponseDto ToLoginDto(this ApplicationUser user, string token)
        {
            return new LoginResponseDto
            {
                Email = user.Email!,
                Username = user.UserName!,
                Token = token
            };
        }
    }
}
