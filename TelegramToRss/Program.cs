using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Redmanmale.TelegramToRss.Crawler;
using Redmanmale.TelegramToRss.DAL;

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
                var config = GetConfiguration();
                crawlerManager = new CrawlerManager(GetSeleniumDriverPath(config),
                                                    CreateDbContext(config),
                                                    CreateCrawlingConfig(config));

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

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private static string GetSeleniumDriverPath(IConfiguration config)
        {
            var driverPath = config.GetSection("SeleniumDrivers")?["GeckoDriverPath"];
            if (string.IsNullOrWhiteSpace(driverPath))
            {
                throw new ArgumentNullException("Config: SeleniumDrivers -> GeckoDriverPath");
            }

            return driverPath;
        }

        private static IStorage CreateDbContext(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("GeneralDbContext");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("Config: ConnectionStrings -> GeneralDbContext");
            }

            return CreatePgSqlDbContext(connectionString);
        }

        private static GeneralDbContext CreatePgSqlDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<GeneralDbContext>();
            options.UseNpgsql(connectionString);
            return new GeneralDbContext(options.Options);
        }

        private static CrawlingConfig CreateCrawlingConfig(IConfiguration config)
        {
            var channelCheckPeriodStr = config.GetSection("Delays")?["ChannelCheckPeriod"];
            if (string.IsNullOrWhiteSpace(channelCheckPeriodStr) ||
                TimeSpan.TryParse(channelCheckPeriodStr, out var channelCheckPeriod))
            {
                throw new ArgumentException("Config: Delays -> ChannelCheckPeriod");
            }

            var channelPostDelayStr = config.GetSection("Delays")?["ChannelPostDelay"];
            if (string.IsNullOrWhiteSpace(channelPostDelayStr) ||
                TimeSpan.TryParse(channelPostDelayStr, out var channelPostDelay))
            {
                throw new ArgumentException("Config: Delays -> ChannelPostDelay");
            }

            return new CrawlingConfig(channelCheckPeriod, channelPostDelay);
        }
    }
}
