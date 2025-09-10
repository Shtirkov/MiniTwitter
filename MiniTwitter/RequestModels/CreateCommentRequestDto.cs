using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.RequestModels
{
    public class CreateCommentRequestDto
    {
        [Required]
        [MaxLength(280)]
        public string Content { get; set; } = string.Empty;
    }
}
