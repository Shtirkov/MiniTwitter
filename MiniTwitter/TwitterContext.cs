using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Entities;
using MiniTwitter.Models;

namespace MiniTwitter
{
    public class TwitterContext : IdentityDbContext<ApplicationUser>
    {
        public TwitterContext(DbContextOptions<TwitterContext> options)
            : base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }
    }
}
