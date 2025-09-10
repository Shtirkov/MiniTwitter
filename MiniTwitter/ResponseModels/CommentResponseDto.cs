namespace MiniTwitter.ResponseModels
{
    public class CommentResponseDto
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public string Author { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
