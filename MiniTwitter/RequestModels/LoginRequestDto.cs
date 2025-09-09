﻿using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;
    }
}
