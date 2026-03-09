using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Mappers
{
    public static class AuthMapper
    {
        public static AuthResponseDto ToAuthDto(this ApplicationUser user, string token)
        {
            return new AuthResponseDto
            {
                Email = user.Email ?? throw new InvalidOperationException(GlobalConstants.UserEmailNullErrorMessage),
                Username = user.UserName ?? throw new InvalidOperationException(GlobalConstants.UserUsernameNullErrorMessage),
                Token = token
            };
        }
    }
}
