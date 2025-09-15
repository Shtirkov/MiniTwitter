using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Mappers
{
    public static class FriendshipsMapper
    {
        public static FriendResponseDto ToFriendshipDto(this Friendship friendship)
        {
            return new FriendResponseDto
            {
                UserUsername = friendship.User.UserName!,
                FriendUsername = friendship.Friend.UserName!,
                IsConfirmed = friendship.IsConfirmed
            };
        }
    }
}
