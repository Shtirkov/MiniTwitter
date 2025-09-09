using MiniTwitter.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class CreatePostRequestDto
    {
        [Required]
        [MaxLength(280)]
        public string Content { get; set; } = string.Empty;
    }
}