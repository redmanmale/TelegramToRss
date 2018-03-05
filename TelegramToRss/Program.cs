using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Redmanmale.TelegramToRss.Crawler;

namespace Redmanmale.TelegramToRss
{
    public static class Program
    {
        private static readonly EventWaitHandle ExitEvent = new AutoResetEvent(false);
        private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            RunAsConsole(args);
        }

        private static void RunAsConsole(string[] args)
        {
            Console.WriteLine("Starting...");
            Console.CancelKeyPress += OnCancelKeyPress;

            var enableCrawler = args.Contains("-c");
            var enableFeed = args.Contains("-f");

            CrawlerManager crawlerManager = null;

            if (enableCrawler)
            {
                var config = ConfigurationManager.GetConfiguration();
                crawlerManager = new CrawlerManager(ConfigurationManager.GetSeleniumDriverPath(config),
                                                    ConfigurationManager.CreateStorage(config),
                                                    ConfigurationManager.CreateCrawlingConfig(config));

                Task.Run(() => crawlerManager
                               .RunAsync(CancellationToken.Token)
                               .ContinueWith(task => Console.WriteLine(task.Exception), CancellationToken.Token),
                         CancellationToken.Token);
            }

            IWebHost webHost = null;
            if (enableFeed)
            {
                webHost = BuildWebHost();

                Console.WriteLine("Started.");
                webHost.Run();
            }
            else
            {
                Console.WriteLine("Started.");
                ExitEvent.WaitOne();
            }

            Console.WriteLine("Stopping...");

            if (enableCrawler)
            {
                crawlerManager.Stop();
            }

            if (enableFeed)
            {
                webHost.StopAsync(CancellationToken.Token).GetAwaiter().GetResult();
            }
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExitEvent.Set();
            CancellationToken.Cancel();
        }

        private static IWebHost BuildWebHost() =>
            WebHost.CreateDefaultBuilder()
                   .UseStartup<Startup>()
                   .Build();
    }
}
