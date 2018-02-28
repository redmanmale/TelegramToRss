using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoRss.DAL
{
    public interface IStorage
    {
        Task<List<Channel>> GetChannelsAsync();

        Task<List<BlogPost>> GetPostsAfterDateAsync(DateTime dateTime);

        Task SavePostAsync(BlogPost post);

        Task UpdateChannelAsync(Channel channel);
    }
}
