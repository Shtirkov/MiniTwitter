using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.RequestModels
{
    public class EditCommentRequestDto
    {
        [Required]
        [MaxLength(280)]
        public string Content { get; set; } = string.Empty;
    }
}
