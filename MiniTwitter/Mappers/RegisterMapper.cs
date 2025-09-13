using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Mappers
{
    public static class RegisterMapper
    {
        public static RegisterResponseDto ToRegisterDto(this ApplicationUser user, string token)
        {
            return new RegisterResponseDto
            {
                Email = user.Email!,
                Username = user.UserName!,
                Token = token
            };
        }
    }
}
