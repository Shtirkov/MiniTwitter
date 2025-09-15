using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;

namespace MiniTwitter.Services
{
    public class FriendshipsService : IFriendshipsService
    {
        private readonly TwitterContext _context;

        public FriendshipsService(TwitterContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        public async Task AddAsync(Friendship friendship)
        {
            await _context.Friendships.AddAsync(friendship);
        }

        public async Task<Friendship?> CheckForPendingFriendRequest(ApplicationUser user, ApplicationUser friend)
        {
            return await _context
                .Friendships
                .FirstOrDefaultAsync(x => x.UserId == friend.Id && x.FriendId == user.Id && !x.IsConfirmed);
        }

        public async Task<bool> CheckIfFriendshipIsRequestedAsync(ApplicationUser user, ApplicationUser friend)
        {
            return await _context
                .Friendships
                .AnyAsync(f => (f.UserId == user.Id && f.FriendId == friend.Id) || (f.UserId == friend.Id && f.FriendId == user.Id));
        }

        public async Task<bool> CheckIfUsersAreFriendsAsync(ApplicationUser user, ApplicationUser friend)
        {
            return await _context
               .Friendships
               .AnyAsync(f => f.UserId == user.Id && f.FriendId == friend.Id && f.IsConfirmed
               || f.UserId == friend.Id && f.FriendId == user.Id && f.IsConfirmed);
        }

        public async Task<List<Friendship>> GetFriendsAsync(ApplicationUser user)
        {
            return await _context
                .Friendships
                .Where(f => (f.UserId == user.Id || f.FriendId == user.Id) && f.IsConfirmed)
                .Include(f => f.User)
                .Include(f => f.Friend)
                .ToListAsync();
        }

        public void Remove(Friendship friendship)
        {
            _context.Friendships.Remove(friendship);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
