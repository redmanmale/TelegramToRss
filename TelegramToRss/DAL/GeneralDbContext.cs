using Microsoft.EntityFrameworkCore;

namespace Redmanmale.TelegramToRss.DAL
{
    public class GeneralDbContext : DbContext
    {
        public GeneralDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Post> BlogPosts { get; set; }

        public DbSet<Channel> Channels { get; set; }
    }
}
