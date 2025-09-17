namespace MiniTwitter.ResponseModels
{
    public class FriendResponseDto
    {
        public string UserUsername { get; set; } = string.Empty;
        public string FriendUsername { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
    }

}
