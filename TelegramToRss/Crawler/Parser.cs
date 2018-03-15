using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Crawler
{
    public static class Parser
    {
        private const int MaxHeaderLength = 50;
        private const string UrlLinkFormat = "<a href=\"{0}\"><img src=\"{0}\"></img></a>";
        private const string NewLineDelimiter = "<br><br>";

        private static readonly Regex HeaderGetter = new Regex("(.+?)" + NewLineDelimiter,
                                                               RegexOptions.Compiled |
                                                               RegexOptions.CultureInvariant |
                                                               RegexOptions.IgnoreCase);

        private static readonly Regex ImageUrlGetter = new Regex("'(.+)'",
                                                                 RegexOptions.Compiled |
                                                                 RegexOptions.CultureInvariant |
                                                                 RegexOptions.IgnoreCase);

        /// <summary>
        /// Create post entity from html.
        /// </summary>
        public static Post ParsePost(string postData)
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

            if (date == null)
            {
                return null;
            }

            var post = new Post
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

                var postText = new[] { body, ExtractImagePreviewLink(image), link }
                    .Where(s => !string.IsNullOrWhiteSpace(s));

                post.Text = string.Join(Environment.NewLine, postText).Trim();
                post.Header = header?.Trim();
            }

            return post;
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

        /// <summary>
        /// Extract header from first string of html; strip redundant tags.
        /// </summary>
        private static (string header, string body) ExtractHeader(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return (null, null);
            }

            var match = HeaderGetter.Match(raw);

            string header;
            if (match.Success && (header = match.Groups[1].Value).Length < MaxHeaderLength)
            {
                return (StripTags(header), PrepareBody(raw, header));
            }

            return (null, raw);
        }

        /// <summary>
        /// Remove header from body and replace line breaks
        /// </summary>
        private static string PrepareBody(string body, string header)
        {
            return body.Replace(header, string.Empty).Replace(NewLineDelimiter, Environment.NewLine);
        }

        /// <summary>
        /// Remove all tags from string
        /// </summary>
        private static string StripTags(string source)
        {
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var ch in source)
            {
                if (ch == '<')
                {
                    inside = true;
                    continue;
                }

                if (ch == '>')
                {
                    inside = false;
                    continue;
                }

                if (inside)
                {
                    continue;
                }

                array[arrayIndex] = ch;
                arrayIndex++;
            }

            return new string(array, 0, arrayIndex);
        }
    }
}
