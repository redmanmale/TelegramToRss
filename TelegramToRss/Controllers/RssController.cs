﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Redmanmale.TelegramToRss.DAL;
using WilderMinds.RssSyndication;

namespace Redmanmale.TelegramToRss.Controllers
{
    [Route("[controller]")]
    public class RssController : Controller
    {
        private const string ContentTypeXml = "application/xml";

        private readonly IStorage _storage;

        public RssController(IStorage storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Post> posts;

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
                    Link = new Uri(post.GetPermalink()),
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
