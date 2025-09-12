namespace MiniTwitter.Models
{
    public class Friendship
    {
        public string UserId { get; set; } = string.Empty;

        public required ApplicationUser User { get; set; }

        public string FriendId { get; set; } = string.Empty;

        public required ApplicationUser Friend { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
