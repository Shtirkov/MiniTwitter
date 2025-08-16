using Microsoft.EntityFrameworkCore;

namespace MiniTwitter.Models
{
    public class TwitterContext : DbContext
    {
        public TwitterContext(DbContextOptions<TwitterContext> options)
            : base(options)
        {
            
        }
    }
}
