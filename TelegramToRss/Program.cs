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
        public static void Main(string[] args)
        {
            RunAsConsole(args);
        }

        private static void RunAsConsole(string[] args)
        {
            Console.WriteLine("Starting...");

            var enableCrawler = args.Contains("-c");
            var enableFeed = args.Contains("-f");

            CrawlerManager crawlerManager = null;
            if (enableCrawler)
            {
                var config = GetConfiguration();
                crawlerManager = new CrawlerManager(GetSeleniumDriverPath(config), CreateDbContext(config));
                Task.Run(() => crawlerManager.RunAsync(CancellationToken.None).ContinueWith(task => Console.WriteLine(task.Exception))); // TODO: Logging
            }

            IWebHost webHost = null;
            if (enableFeed)
            {
                webHost = BuildWebHost();
                webHost.Run();
            }

            Console.WriteLine("Stopping...");

            if (enableCrawler)
            {
                crawlerManager.Stop();
            }

            if (enableFeed)
            {
                webHost.StopAsync().GetAwaiter().GetResult();
            }
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
            var driverPath = config.GetSection("SeleniumDrivers")["GeckoDriverPath"];
            return driverPath;
        }

        private static IStorage CreateDbContext(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("BlogDbContext");
            return CreatePgSqlDbContext(connectionString);
        }

        private static BlogDbContext CreatePgSqlDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<BlogDbContext>();
            options.UseNpgsql(connectionString);
            return new BlogDbContext(options.Options);
        }
    }
}
