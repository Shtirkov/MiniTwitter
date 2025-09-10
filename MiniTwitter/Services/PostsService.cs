using Microsoft.EntityFrameworkCore;
using MiniTwitter.Entities;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;

namespace MiniTwitter.Services
{
    public class PostsService : IPostsService
    {
        private readonly TwitterContext _context;

        public PostsService(TwitterContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
        }

        public async Task<List<Post>> GetFriendsPosts(ApplicationUser user, List<Friendship> friends)
        {
            var friendNames = friends.Select(f => f.UserId == user.Id ? f.FriendId : f.UserId);

            return await _context
                .Posts
                .Where(p => friendNames.Contains(p.AuthorId) && p.AuthorId != user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post?> GetPostAsync(int id)
        {
            return await _context
                .Posts
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetPostsByUserAsync(string username)
        {
            return await _context
                         .Posts
                         .Include(p => p.Comments)
                         .ThenInclude(c => c.Author)
                         .Where(p => p.Author!.UserName == username)
                         .OrderByDescending(p => p.CreatedAt)
                         .ToListAsync();
        }

        public void Remove(Post post)
        {
            _context.Posts.Remove(post);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
