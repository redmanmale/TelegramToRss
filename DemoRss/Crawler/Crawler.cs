using System;
using DemoRss.DAL;
using HtmlAgilityPack;
using OpenQA.Selenium.Firefox;

namespace DemoRss.Crawler
{
    public class Crawler
    {
        private readonly FirefoxDriver _driver;

        public Crawler(string firefoxDriverPath)
        {
            var options = new FirefoxOptions();
            options.AddArguments("-headless");

            _driver = new FirefoxDriver(firefoxDriverPath, options);
        }

        public BlogPost GetPost(string url)
        {
            _driver.Url = url;
            _driver.Navigate();

            return ParsePost(_driver.PageSource);
        }

        private static BlogPost ParsePost(string postData)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(postData);

            var text = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='tgme_widget_message_text']")?.InnerHtml;
            var imageLink = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='tgme_widget_message_link_preview']")?.GetAttributeValue("href", null);
            var date = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='tgme_widget_message_date']")?.FirstChild?.GetAttributeValue("datetime", null);

            var post = new BlogPost
            {
                PublishDate = DateTime.Parse(date),
                Text = text + "<br><br>" + imageLink
            };

            return post;
        }
    }
}
