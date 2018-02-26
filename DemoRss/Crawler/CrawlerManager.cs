using System.Threading;
using System.Threading.Tasks;

namespace DemoRss.Crawler
{
    public class CrawlerManager
    {
        private readonly Crawler _crawler;

        public CrawlerManager(string driverPath)
        {
            _crawler = new Crawler(driverPath);
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
