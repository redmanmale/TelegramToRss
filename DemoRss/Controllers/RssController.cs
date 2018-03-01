using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DemoRss.DAL;
using Microsoft.AspNetCore.Mvc;
using WilderMinds.RssSyndication;

namespace DemoRss.Controllers
{
    [Route("[controller]")]
    public class RssController : Controller
    {
        private const string ContentTypeXml = "application/xml";

        private readonly IStorage _storage;

        public RssController(BlogDbContext storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<BlogPost> posts;

            try
            {
                posts = await _storage.GetPostsAfterDateAsync(DateTime.Now.AddDays(-1000));
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            var feed = new Feed
            {
                Title = "Username's feed",
                Copyright = "",
                Description = "",
                Link = new Uri("https://foobar")
            };

            foreach (var post in posts)
            {
                var item = new Item
                {
                    Title = post.Header,
                    Body = post.Text,
                    Link = new Uri(post.GetUrl()),
                    PublishDate = post.PublishDate,
                    Author = new Author { Name = post.Channel.Name }
                };

                feed.Items.Add(item);
            }

            return FormatRssResponse(feed);
        }

        private ContentResult FormatRssResponse(Feed rss) => Content(rss.Serialize(), ContentTypeXml);
    }
}
