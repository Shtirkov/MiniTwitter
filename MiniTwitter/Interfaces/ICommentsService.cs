using MiniTwitter.Models;
using MiniTwitter.RequestModels;

namespace MiniTwitter.Interfaces
{
    public interface ICommentsService
    {
        public Task AddAsync(Comment comment);

        public Task SaveChangesAsync();

        public void Remove(Comment comment);

        public Task<Comment?> GetAsync(int id);

        public Comment Edit(Comment comment, EditCommentRequestDto commentDto);
    }
}
