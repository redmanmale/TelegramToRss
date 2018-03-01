using System;

namespace Redmanmale.TelegramToRss.Crawler
{
    public class CrawlingConfig
    {
        public TimeSpan ChannelCheckPeriod { get; }

        public TimeSpan ChannelPostDelay { get; }

        public CrawlingConfig(TimeSpan channelCheckPeriod, TimeSpan channelPostDelay)
        {
            ChannelCheckPeriod = channelCheckPeriod;
            ChannelPostDelay = channelPostDelay;
        }
    }
}
