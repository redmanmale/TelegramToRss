using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Redmanmale.TelegramToRss.DAL;

namespace Redmanmale.TelegramToRss.Controllers
{
    [Route("[controller]")]
    public class ChannelController
    {
        private readonly IStorage _storage;

        public ChannelController(BlogDbContext storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public Task<List<Channel>> Get()
        {
            try
            {
                return _storage.GetChannelsAsync();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }
        }

        [HttpGet]
        [Route("{channelId}")]
        public Task<Channel> GetChannel([FromQuery] long channelId)
        {
            try
            {
                return _storage.GetChannelAsync(channelId);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }
        }

        [HttpPost]
        public Task Add([FromBody] Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException(nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(channel.Name))
            {
                throw new ArgumentNullException(nameof(channel.Name));
            }

            if (string.IsNullOrWhiteSpace(channel.Url))
            {
                throw new ArgumentNullException(nameof(channel.Url));
            }

            channel.LastNumber = 0;
            channel.LastUpdate = DateTime.Now;

            try
            {
                return _storage.AddChannelAsync(channel);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return Task.CompletedTask;
            }
        }

        [HttpDelete]
        [Route("{channelId}")]
        public Task Delete(long channelId)
        {
            try
            {
                return _storage.DeleteChannelAsync(channelId);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return Task.CompletedTask;
            }
        }
    }
}
