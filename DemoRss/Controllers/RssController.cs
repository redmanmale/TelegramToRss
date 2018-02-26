using System;
using System.Linq;
using System.Threading.Tasks;
using DemoRss.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WilderMinds.RssSyndication;

namespace DemoRss.Controllers
{
    [Route("[controller]")]
    public class RssController : Controller
    {
        private const string ContentTypeXml = "application/xml";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (var context = new BlogDbContext())
            {
                var posts = await context.BlogPosts.Where(b => b.PublishDate > DateTime.Now.AddDays(-1)).ToListAsync();

                var feed = new Feed
                {
                    Title = "Shawn Wildermuth's Blog",
                    Description = "My Favorite Rants and Raves",
                    Link = new Uri("http://wildermuth.com/feed"),
                    Copyright = "(c) 2016"
                };

                foreach (var post in posts)
                {
                    var item = new Item
                    {
                        Title = "TODO",
                        Body = post.Text,
                        Link = new Uri("https://TODO"),
                        PublishDate = post.PublishDate,
                        Author = new Author { Name = post.Channel.Name }
                    };

                    feed.Items.Add(item);
                }

                return FormatRssResponse(feed);
            }
        }

        private ContentResult FormatRssResponse(Feed rss) => Content(rss.Serialize(), ContentTypeXml);
    }
}
