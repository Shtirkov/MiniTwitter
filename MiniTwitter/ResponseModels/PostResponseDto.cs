namespace MiniTwitter.ResponseModels
{
    public class PostResponseDto
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public required string Author { get; set; } = string.Empty;

        public int Likes { get; set; }

        public DateTime CreatedAt{ get; set; }

        public ICollection<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
    }
}
