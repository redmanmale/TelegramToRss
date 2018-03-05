﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Redmanmale.TelegramToRss.Crawler;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss
{
    public static class ConfigurationManager
    {
        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        public static string GetSeleniumDriverPath(IConfiguration config)
        {
            var driverPath = config.GetSection("SeleniumDrivers")?["GeckoDriverPath"];
            if (string.IsNullOrWhiteSpace(driverPath))
            {
                throw new ArgumentNullException("Config: SeleniumDrivers -> GeckoDriverPath");
            }

            return driverPath;
        }

        public static Storage CreateStorage(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("GeneralDbContext");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("Config: ConnectionStrings -> GeneralDbContext");
            }

            return new Storage(CreatePgSqlDbContext(connectionString));
        }

        private static GeneralDbContext CreatePgSqlDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<GeneralDbContext>();
            options.UseNpgsql(connectionString);
            return new GeneralDbContext(options.Options);
        }

        public static CrawlingConfig CreateCrawlingConfig(IConfiguration config)
        {
            var channelCheckPeriodStr = config.GetSection("Delays")?["ChannelCheckPeriod"];
            if (string.IsNullOrWhiteSpace(channelCheckPeriodStr) ||
                !TimeSpan.TryParse(channelCheckPeriodStr, out var channelCheckPeriod))
            {
                throw new ArgumentException("Config: Delays -> ChannelCheckPeriod");
            }

            var channelPostDelayStr = config.GetSection("Delays")?["ChannelPostDelay"];
            if (string.IsNullOrWhiteSpace(channelPostDelayStr) ||
                !TimeSpan.TryParse(channelPostDelayStr, out var channelPostDelay))
            {
                throw new ArgumentException("Config: Delays -> ChannelPostDelay");
            }

            return new CrawlingConfig(channelCheckPeriod, channelPostDelay);
        }
    }
}