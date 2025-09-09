using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(6)]
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;
    }
}
