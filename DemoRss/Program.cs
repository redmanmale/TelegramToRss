using System;
using System.Threading;
using System.Threading.Tasks;
using DemoRss.Crawler;
using DemoRss.DAL;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DemoRss
{
    public static class Program
    {
        private static readonly EventWaitHandle ExitEvent = new AutoResetEvent(false);

        public static void Main(string[] args)
        {
            RunAsConsole(args);
        }

        private static void RunAsConsole(string[] args)
        {
            Console.WriteLine("Starting...");
            Console.CancelKeyPress += OnCancelKeyPress;

            var config = GetConfiguration();
            var crawlerManager = new CrawlerManager(GetSeleniumDriverPath(config), CreateDbContext(config));
            Task.Run(() => crawlerManager.RunAsync(CancellationToken.None).ContinueWith(task => Console.WriteLine(task.Exception))); // TODO: Logging

            var webHost = BuildWebHost(args);
            //Task.Run(() => webHost.RunAsync());
            webHost.Run();

            Console.WriteLine("Started.");
            ExitEvent.WaitOne();

            Console.WriteLine("Stopping...");

            crawlerManager.Stop();
            webHost.StopAsync().GetAwaiter().GetResult();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExitEvent.Set();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
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
