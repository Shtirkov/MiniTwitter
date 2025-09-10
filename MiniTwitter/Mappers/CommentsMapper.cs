using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Mappers
{
    public static class CommentsMapper
    {
        public static CommentResponseDto ToCommentDto(this Comment comment)
        {
            return new CommentResponseDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                Author = comment.Author.UserName!,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
