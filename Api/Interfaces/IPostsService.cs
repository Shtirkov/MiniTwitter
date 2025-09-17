using MiniTwitter.Entities;
using MiniTwitter.Helpers;
using MiniTwitter.Models;
using MiniTwitter.RequestModels;

namespace MiniTwitter.Interfaces
{
    public interface IPostsService
    {
        public Task AddAsync(Post post);

        public Task SaveChangesAsync();

        public Task<PagedResult<Post>> GetPostsByUserAsync(string username, QueryParams queryParams);

        public Task<PagedResult<Post>> GetFriendsPosts(ApplicationUser user, List<Friendship> friends, QueryParams queryParams);

        public Task<Post?> GetPostAsync(int id);

        public Post Edit(Post post, EditPostRequestDto postRequestDto);

        public void Remove(Post post);

        public Task Like(Post post, ApplicationUser user);
    }
}
