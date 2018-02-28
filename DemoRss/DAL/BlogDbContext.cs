using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DemoRss.DAL
{
    public class BlogDbContext : DbContext, IStorage
    {
        public BlogDbContext(DbContextOptions options) : base(options) { }

        private DbSet<BlogPost> BlogPosts { get; set; }

        private DbSet<Channel> Channels { get; set; }

        public Task<List<Channel>> GetChannelsAsync()
        {
            return Channels.ToListAsync();
        }

        public Task<List<BlogPost>> GetPostsAfterDateAsync(DateTime dateTime)
        {
            return BlogPosts.Where(b => b.PublishDate > dateTime)
                            .Include(a => a.Channel)
                            .ToListAsync();
        }

        public async Task SavePostAsync(BlogPost post)
        {
            try
            {
                using (var transaction = Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    await BlogPosts.AddAsync(post);
                    await SaveChangesAsync();
                    transaction.Commit();
                }
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw;
            }
        }

        public async Task UpdateChannelAsync(Channel channel)
        {
            try
            {
                using (var transaction = Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    Channels.Attach(channel);
                    Entry(channel).State = EntityState.Modified;

                    await SaveChangesAsync();
                    transaction.Commit();
                }
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw;
            }
        }
    }
}
