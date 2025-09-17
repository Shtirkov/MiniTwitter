using MiniTwitter.Entities;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Mappers
{
    public static class PostsMapper
    {
        public static PostResponseDto ToPostDto(this Post post)
        {
            return new PostResponseDto
            {
                Id = post.Id,
                Author = post.Author.UserName!,
                Content = post.Content,
                Comments = post.Comments.Select(c => c.ToCommentDto()).ToList(),
                Likes = post.TotalLikes,
                CreatedAt = post.CreatedAt
            };
        }
    }
}
