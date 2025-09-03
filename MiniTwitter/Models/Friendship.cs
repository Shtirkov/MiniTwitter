namespace MiniTwitter.Models
{
    public class Friendship
    {
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; }

        public string FriendId { get; set; } = string.Empty;

        public ApplicationUser Friend { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
