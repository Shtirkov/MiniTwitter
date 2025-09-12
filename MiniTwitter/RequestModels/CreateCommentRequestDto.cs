using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.RequestModels
{
    public class CreateCommentRequestDto
    {
        [Required]
        [MaxLength(280, ErrorMessage = "Content must not be more 280 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
