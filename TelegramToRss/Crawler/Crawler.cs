using System;
using System.Net.Http;
using System.Threading.Tasks;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Crawler
{
    /// <summary>
    /// Crawler to download, parse and create entites for posts.
    /// </summary>
    public class Crawler : IDisposable
    {
        private readonly HttpClient _client;

        public Crawler()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Create post entity from provided URL.
        /// </summary>
        public async Task<Post> GetPost(string url)
        {
            var pageSource = await _client.GetStringAsync(url);
            return Parser.ParsePost(pageSource);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
