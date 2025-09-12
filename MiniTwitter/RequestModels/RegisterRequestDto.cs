using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Username must not be at least 6 characters.")]
        [MaxLength(255, ErrorMessage = "Username must not be more 255 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Please use valid email address.")]
        [MaxLength(255, ErrorMessage = "Email must be at lost 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255, ErrorMessage = "Password must not be more 280 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
