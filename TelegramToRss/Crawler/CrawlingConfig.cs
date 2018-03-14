using System;

namespace Redmanmale.TelegramToRss.Crawler
{
    public class CrawlingConfig
    {
        public TimeSpan ChannelCheckPeriod { get; }

        public TimeSpan ChannelPostDelay { get; }

        public bool ForceCleanup { get; }

        public CrawlingConfig(TimeSpan channelCheckPeriod, TimeSpan channelPostDelay, bool forceCleanup)
        {
            ChannelCheckPeriod = channelCheckPeriod;
            ChannelPostDelay = channelPostDelay;
            ForceCleanup = forceCleanup;
        }
    }
}
