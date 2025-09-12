using MiniTwitter.Entities;

namespace MiniTwitter.Models
{
    public class Comment
    {

        public int Id { get; set; }

        public int PostId { get; set; }

        public required Post Post { get; set; }

        public string AuthorId { get; set; } = string.Empty;

        public required ApplicationUser Author { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
