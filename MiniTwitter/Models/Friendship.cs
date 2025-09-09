using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.Models
{
    public class Friendship
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public required ApplicationUser User { get; set; }

        [Required]
        public string FriendId { get; set; } = string.Empty;

        public required ApplicationUser Friend { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
