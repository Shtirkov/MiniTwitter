using Microsoft.EntityFrameworkCore;
using MiniTwitter;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;

public class FriendshipsService : IFriendshipsService
{
    private readonly TwitterContext _context;

    public FriendshipsService(TwitterContext context)
    {
        _context = context;
    }

    private async Task<Friendship?> GetFriendshipAsync(string userId, string friendId)
    {
        return await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.UserId == userId && f.FriendId == friendId) ||
                (f.UserId == friendId && f.FriendId == userId));
    }

    private Task<List<Friendship>> GetFriendsQuery(string userId)
    {
        return _context.Friendships
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.IsConfirmed)
            .Include(f => f.User)
            .Include(f => f.Friend)
            .ToListAsync();
    }

    public async Task AddAsync(Friendship friendship)
    {
        await _context.Friendships.AddAsync(friendship);
    }

    public async Task<Friendship?> CheckForPendingFriendRequest(ApplicationUser user, ApplicationUser friend)
    {
        var friendship = await GetFriendshipAsync(user.Id, friend.Id);

        if (friendship != null && !friendship.IsConfirmed)
        {
            return friendship;
        }

        return null;
    }

    public async Task<bool> CheckIfFriendshipIsRequestedAsync(ApplicationUser user, ApplicationUser friend)
    {
        return await GetFriendshipAsync(user.Id, friend.Id) != null;
    }

    public async Task<bool> CheckIfUsersAreFriendsAsync(ApplicationUser user, ApplicationUser friend)
    {
        var friendship = await GetFriendshipAsync(user.Id, friend.Id);

        if (friendship != null && friendship.IsConfirmed)
        {
            return true;
        }

        return false;
    }

    public async Task<List<Friendship>> GetFriendsAsync(ApplicationUser user)
    {
        return await GetFriendsQuery(user.Id);
    }

    public void Remove(Friendship friendship)
    {
        _context.Friendships.Remove(friendship);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Friendship>> GetPendingFriendshipRequests(ApplicationUser user)
    {
      return await _context.Friendships.Where(f => f.FriendId == user.Id && f.IsConfirmed == false).Include(f => f.User).ToListAsync();
    }
}
