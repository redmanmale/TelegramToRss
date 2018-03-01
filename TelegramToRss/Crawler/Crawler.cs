using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium.Firefox;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Crawler
{
    /// <summary>
    /// Crawler to download, parse and create entites for posts.
    /// </summary>
    public class Crawler : IDisposable
    {
        private static readonly Regex ImageUrlGetter = new Regex("'(.+)'",
                                                                 RegexOptions.Compiled |
                                                                 RegexOptions.CultureInvariant |
                                                                 RegexOptions.IgnoreCase);

        private const string NewLineDelimiter = "<br><br>";
        private static readonly Regex HeaderGetter = new Regex("(.+?)" + NewLineDelimiter,
                                                                      RegexOptions.Compiled |
                                                                      RegexOptions.CultureInvariant |
                                                                      RegexOptions.IgnoreCase);

        private const string UrlLinkFormat = "<a href=\"{0}\"><img src=\"{0}\"></img></a>";

        private readonly FirefoxDriver _driver;

        public Crawler(string firefoxDriverPath)
        {
            var options = new FirefoxOptions();
            options.AddArguments("-headless");

            _driver = new FirefoxDriver(firefoxDriverPath, options);
        }

        /// <summary>
        /// Create post entity from provided URL.
        /// </summary>
        public BlogPost GetPost(string url)
        {
            _driver.Url = url;
            _driver.Navigate();

            return ParsePost(_driver.PageSource);
        }

        /// <summary>
        /// Create post entity from html.
        /// </summary>
        private static BlogPost ParsePost(string postData)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(postData);

            var text = htmlDoc.DocumentNode
                              .SelectSingleNode("//div[@class='tgme_widget_message_text']")
                              ?.InnerHtml;

            var link = htmlDoc.DocumentNode
                              .SelectSingleNode("//a[@class='tgme_widget_message_link_preview']")
                              ?.GetAttributeValue("href", null);

            var image = htmlDoc.DocumentNode
                               .SelectSingleNode("//a[@class='tgme_widget_message_photo_wrap']")
                               ?.GetAttributeValue("style", null);

            var date = htmlDoc.DocumentNode
                              .SelectSingleNode("//a[@class='tgme_widget_message_date']")
                              ?.FirstChild
                              ?.GetAttributeValue("datetime", null);

            var isService = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='message_media_not_supported_label']") != null;
            var isDeleted = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='tgme_widget_message_error']") != null;

            var post = new BlogPost
            {
                PublishDate = DateTime.Parse(date)
            };

            if (isService)
            {
                post.State = PostState.Service;
            }
            else if (isDeleted)
            {
                post.State = PostState.Deleted;
            }
            else
            {
                var (header, body) = ExtractHeader(text);
                post.Text = string.Join(Environment.NewLine,
                                        new[] { body, ExtractImagePreviewLink(image), link }
                                            .Where(s => !string.IsNullOrWhiteSpace(s)))
                                  .Trim();
                post.Header = header?.Trim();
            }

            return post;
        }

        /// <summary>
        /// Extract header from first string of html; trim redundant tags.
        /// </summary>
        private static (string header, string body) ExtractHeader(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return (null, null);
            }

            var match = HeaderGetter.Match(raw);
            return match.Success
                ? (match.Groups[1].Value, raw.Replace(match.Groups[1].Value, "").Replace(NewLineDelimiter, Environment.NewLine))
                : (null, raw);
        }

        /// <summary>
        /// Extract image url from html and format it properly.
        /// </summary>
        private static string ExtractImagePreviewLink(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            var match = ImageUrlGetter.Match(raw);
            return match.Success ? string.Format(UrlLinkFormat, match.Groups[1].Value) : null;
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
