using Microsoft.AspNetCore.Identity;
using MiniTwitter.Entities;

namespace MiniTwitter.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Post> Posts { get; set; }

        public ApplicationUser()
        {
            Posts = new List<Post>();
        }
    }
}
