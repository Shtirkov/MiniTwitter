using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please use valid email address.")]
        [MaxLength(255, ErrorMessage = "Email must be at lost 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255, ErrorMessage = "Password must not be more 280 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
