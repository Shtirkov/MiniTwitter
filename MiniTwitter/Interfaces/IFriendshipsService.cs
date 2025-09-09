using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Interfaces
{
    public interface IFriendshipsService
    {
        public Task<bool> CheckIfFriendshipIsRequestedAsync(ApplicationUser user, ApplicationUser friend);

        public Task<bool> CheckIfUsersAreFriendsAsync(ApplicationUser user, ApplicationUser friend);

        public Task AddAsync(Friendship friendship);

        public Task SaveChangesAsync();

        public Task<Friendship?> CheckForPendingFriendRequest(ApplicationUser user, ApplicationUser friend);

        public void Remove(Friendship friendship);

        public Task<List<FriendResponseDto>> GetFriendsAsync(ApplicationUser user);
    }
}
