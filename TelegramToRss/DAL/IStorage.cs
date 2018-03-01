using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redmanmale.TelegramToRss.DAL
{
    public interface IStorage
    {
        Task<List<Channel>> GetChannelsAsync();

        Task<Channel> GetChannelAsync(long channelId);

        Task<List<BlogPost>> GetPostsAfterDateAsync(DateTime dateTime);

        Task SavePostAsync(BlogPost post);

        Task AddChannelAsync(Channel channel);

        Task UpdateChannelAsync(Channel channel);

        Task DeleteChannelAsync(long channelId);
    }
}
