using Microsoft.EntityFrameworkCore;

namespace MiniTwitter
{
    public class TwitterContext : DbContext
    {
        public TwitterContext(DbContextOptions<TwitterContext> options)
            : base(options)
        {
            
        }
    }
}
