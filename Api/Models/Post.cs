using MiniTwitter.Models;

namespace MiniTwitter.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string AuthorId { get; set; } = string.Empty;

        public ApplicationUser? Author { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public int TotalLikes => Likes.Count;
    }
}
