using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class CreatePostRequestDto
    {
        [Required]
        [MaxLength(280, ErrorMessage = "Content must not be more than 280 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}