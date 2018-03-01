using System;
using System.Threading;
using System.Threading.Tasks;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Crawler
{
    public class CrawlerManager
    {
        private static readonly TimeSpan MainPeriod = TimeSpan.FromSeconds(3000);
        private static readonly TimeSpan PostPeriod = TimeSpan.FromSeconds(1);

        private readonly Crawler _crawler;
        private readonly IStorage _storage;

        public CrawlerManager(string driverPath, IStorage storage)
        {
            _storage = storage;
            _crawler = new Crawler(driverPath);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var channels = await _storage.GetChannelsAsync();
                foreach (var channel in channels)
                {
                    while (await CheckForNewPost(channel))
                    {
                        await Task.Delay(PostPeriod, cancellationToken);
                    }
                }

                await Task.Delay(MainPeriod, cancellationToken);
            }
        }

        public void Stop()
        {
            _crawler.Dispose();
        }

        private async Task<bool> CheckForNewPost(Channel channel)
        {
            var newPostNumber = channel.LastNumber + 1;
            var newPost = _crawler.GetPost(BlogPost.FormatUrl(channel.Url, newPostNumber));
            if (newPost == null || newPostNumber > 10)
            {
                return false;
            }

            newPost.Channel = channel;
            newPost.ChannelId = channel.Id;
            newPost.Number = newPostNumber;

            if (string.IsNullOrWhiteSpace(newPost.Header))
            {
                newPost.Header = "Post #" + newPost.Number;
            }

            if (newPost.State == PostState.Normal)
            {
                await _storage.SavePostAsync(newPost);
            }

            channel.LastUpdate = DateTime.Now;
            channel.LastNumber = newPost.Number;

            await _storage.UpdateChannelAsync(channel);
            return true;
        }
    }
}
