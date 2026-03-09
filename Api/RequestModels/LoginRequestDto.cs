using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class LoginRequestDto
    {
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
