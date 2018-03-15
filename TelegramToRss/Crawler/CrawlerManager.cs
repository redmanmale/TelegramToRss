using System;
using System.Threading;
using System.Threading.Tasks;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Crawler
{
    public class CrawlerManager
    {
        private static readonly long[] _steps = { 1, 5, 10, 50 };

        private readonly Crawler _crawler;
        private readonly IStorage _storage;
        private readonly CrawlingConfig _config;

        public CrawlerManager(IStorage storage, CrawlingConfig config)
        {
            _storage = storage;
            _config = config;
            _crawler = new Crawler();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var channels = await _storage.GetChannelsToCheckAsync(DateTime.Now.Add(-_config.ChannelCheckPeriod));
                foreach (var channel in channels)
                {
                    await CheckChannel(channel, cancellationToken);
                }

                // TODO: configure delay between channel update checks
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }

        private async Task CheckChannel(Channel channel, CancellationToken cancellationToken)
        {
            while (await CheckForNewPost(channel))
            {
                await Task.Delay(_config.ChannelPostDelay, cancellationToken);
            }

            channel.LastCheck = DateTime.Now;
            await _storage.UpdateChannelAsync(channel);
        }

        private async Task<bool> CheckForNewPost(Channel channel)
        {
            var newPostNumber = channel.LastNumber + 1;
            var newPost = await _crawler.GetPost(Post.FormatUrl(channel.Url, newPostNumber));

            // TODO: for debug
            if (newPostNumber > 20)
            {
                return false;
            }

            // Because sometimes there are missing posts and we have to check if there are next ones.
            if (newPost == null && !await CheckIfNextPostsExist(channel.Url, newPostNumber))
            {
                return false;
            }

            if (newPost != null)
            {
                newPost.Channel = channel;
                newPost.ChannelId = channel.Id;
                newPost.Number = newPostNumber;

                if (string.IsNullOrWhiteSpace(newPost.Header))
                {
                    newPost.Header = "Post #" + newPost.Number;
                }

                if (newPost.State == PostState.Normal)
                {
                    channel.LastPost = newPost.PublishDate;
                    await _storage.SavePostAsync(newPost);
                }
            }

            channel.LastNumber = newPostNumber;
            return true;
        }

        /// <summary>
        /// Check if there are posts after current one.
        /// </summary>
        private async Task<bool> CheckIfNextPostsExist(string url, long newPostNumber)
        {
            foreach (var step in _steps)
            {
                var post = await _crawler.GetPost(Post.FormatUrl(url, newPostNumber + step));
                if (post != null)
                {
                    return true;
                }
            }

            return false;
        }

        public void Stop() => _crawler?.Dispose();
    }
}
