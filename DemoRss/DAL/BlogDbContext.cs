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
            try
            {
                return Channels.OrderBy(d => d.Id).ToListAsync();
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw new DbException("Error occured when reading channels", exc);
            }
        }

        public Task<Channel> GetChannelAsync(long channelId)
        {
            try
            {
                return Channels.SingleOrDefaultAsync(a => a.Id == channelId);
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw new DbException($"Error occured when reading channel: {channelId}", exc);
            }
        }

        public Task<List<BlogPost>> GetPostsAfterDateAsync(DateTime dateTime)
        {
            try
            {
                return BlogPosts.Where(b => b.PublishDate > dateTime)
                                .OrderBy(d => d.Id)
                                .Include(a => a.Channel)
                                .ToListAsync();
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw new DbException("Error occured when reading posts", exc);
            }
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
                throw new DbException($"Error occured when saving post: {post}", exc);
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
                throw new DbException($"Error occured when updating channel: {channel}", exc);
            }
        }

        public async Task AddChannelAsync(Channel channel)
        {
            try
            {
                using (var transaction = Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    await Channels.AddAsync(channel);
                    await SaveChangesAsync();
                    transaction.Commit();
                }
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw new DbException($"Error occured when adding channel: {channel}", exc);
            }
        }

        public async Task DeleteChannelAsync(long channelId)
        {
            try
            {
                using (var transaction = Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var channel = await Channels.AsNoTracking().SingleOrDefaultAsync(d => d.Id == channelId);
                    if (channel != null)
                    {
                        Channels.Remove(channel);
                        await SaveChangesAsync();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw new DbException($"Error occured when deleting channel: {channelId}", exc);
            }
        }
    }
}
