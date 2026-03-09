using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Username must be at least 6 characters.")]
        [MaxLength(255, ErrorMessage = "Username must not be more than 255 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Please use valid email address.")]
        [MaxLength(255, ErrorMessage = "Email must not be more than 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [MaxLength(255, ErrorMessage = "Password must not be more than 255 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
