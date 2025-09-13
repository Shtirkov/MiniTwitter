using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.RequestModels
{
    public class EditPostRequestDto
    {
        [Required]
        [MaxLength(280, ErrorMessage = "Content must not be more than 280 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
