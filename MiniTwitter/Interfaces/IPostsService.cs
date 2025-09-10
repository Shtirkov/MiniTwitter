using MiniTwitter.Entities;
using MiniTwitter.Models;

namespace MiniTwitter.Interfaces
{
    public interface IPostsService
    {
        public Task AddAsync(Post post);

        public Task SaveChangesAsync();

        public Task<List<Post>> GetPostsByUserAsync(string username);

        public Task<List<Post>> GetFriendsPosts(ApplicationUser user, List<Friendship> friends);

        public Task<Post?> GetPostAsync(int id);

        public void Remove(Post post);
    }
}
