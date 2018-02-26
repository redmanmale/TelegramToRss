using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DemoRss
{
    public class Program
    {
        //private string _firefoxDriverPath = @"E:\git\Projects\geckodriver-v0.19.1-win64";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .Build();
    }
}
