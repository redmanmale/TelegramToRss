using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Redmanmale.TelegramToRss.DAL
{
    public class Storage : IStorage, IDisposable
    {
        private readonly GeneralDbContext _context;

        public Storage(GeneralDbContext context)
        {
            _context = context;
        }

        public Task<List<Channel>> GetChannelsAsync()
        {
            try
            {
                return _context.Channels.OrderBy(d => d.Id).ToListAsync();
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
                return _context.Channels.SingleOrDefaultAsync(a => a.Id == channelId);
            }
            catch (Exception exc)
            {
                // TODO: Logging
                Console.WriteLine(exc);
                throw new DbException($"Error occured when reading channel: {channelId}", exc);
            }
        }

        public Task<List<Post>> GetPostsAfterDateAsync(DateTime dateTime)
        {
            try
            {
                return _context.BlogPosts.Where(b => b.PublishDate > dateTime)
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

        public async Task SavePostAsync(Post post)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    await _context.BlogPosts.AddAsync(post);
                    await _context.SaveChangesAsync();
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
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    _context.Channels.Attach(channel);
                    _context.Entry(channel).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
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
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    await _context.Channels.AddAsync(channel);
                    await _context.SaveChangesAsync();
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
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var channel = await _context.Channels.AsNoTracking().SingleOrDefaultAsync(d => d.Id == channelId);
                    if (channel != null)
                    {
                        _context.Channels.Remove(channel);
                        await _context.SaveChangesAsync();
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

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
