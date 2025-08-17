using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength (255)]
        public string Password { get; set; }
    }
}
