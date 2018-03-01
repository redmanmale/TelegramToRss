using System;
using System.Threading;
using System.Threading.Tasks;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Crawler
{
    public class CrawlerManager
    {
        private readonly Crawler _crawler;
        private readonly IStorage _storage;
        private readonly CrawlingConfig _config;

        public CrawlerManager(string driverPath, IStorage storage, CrawlingConfig config)
        {
            _storage = storage;
            _config = config;
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
                        await Task.Delay(_config.ChannelPostDelay, cancellationToken);
                    }
                }

                await Task.Delay(_config.ChannelCheckPeriod, cancellationToken);
            }
        }

        public void Stop() => _crawler.Dispose();

        private async Task<bool> CheckForNewPost(Channel channel)
        {
            var newPostNumber = channel.LastNumber + 1;
            var newPost = _crawler.GetPost(Post.FormatUrl(channel.Url, newPostNumber));
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
