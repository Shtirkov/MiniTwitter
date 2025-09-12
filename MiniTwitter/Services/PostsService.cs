using Microsoft.EntityFrameworkCore;
using MiniTwitter.Entities;
using MiniTwitter.Helpers;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<PagedResult<Post>> GetFriendsPosts(ApplicationUser user, List<Friendship> friends, QueryParams queryParams)
        {
            var friendNames = friends.Select(f => f.UserId == user.Id ? f.FriendId : f.UserId);

            var query = _context
                .Posts
                .Where(p => friendNames.Contains(p.AuthorId) && p.AuthorId != user.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .ToListAsync();

            return new PagedResult<Post>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<Post?> GetPostAsync(int id)
        {
            return await _context
                .Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedResult<Post>> GetPostsByUserAsync(string username, QueryParams queryParams)
        {
            var query = _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .Where(p => p.Author!.UserName == username)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Include(p => p.Comments)
                .ThenInclude (c => c.Author)
                .ToListAsync();

            return new PagedResult<Post>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
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
