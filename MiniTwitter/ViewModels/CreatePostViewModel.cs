using MiniTwitter.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        [MaxLength(280)]
        public string Content { get; set; }
    }
}