using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redmanmale.TelegramToRss.DAL
{
    public class BlogPost
    {
        /// <summary>
        /// Id from DB.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Id from website.
        /// </summary>
        [Required]
        public long Number { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Post header; if doesn't exist template 'Post #N' is used.
        /// </summary>
        [Required]
        public string Header { get; set; }

        /// <summary>
        /// Post body: text, preview image, link.
        /// </summary>
        [Required]
        public string Text { get; set; }

        [Required]
        public long ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public Channel Channel { get; set; }

        /// <summary>
        /// Post type; only normal posts went to DB.
        /// </summary>
        [Required]
        public PostState State { get; set; }

        /// <summary>
        /// Permalink URL of the post.
        /// </summary>
        public string GetUrl() => FormatUrl(Channel.Url, Number);

        /// <summary>
        /// Generate URL of the post for provided data.
        /// </summary>
        public static string FormatUrl(string channelUri, long postId) => channelUri + postId + "?embed=1";
    }

    public enum PostState
    {
        /// <summary>
        /// Regular post; it goes to DB.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Service post; it doesn't go to DB.
        /// </summary>
        Service = 1,

        /// <summary>
        /// Deleted post; no info about it; it doesn't go to DB.
        /// </summary>
        Deleted = 2
    }

    public class Channel
    {
        /// <summary>
        /// Id from DB.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Channel name; it's used as author name in feed.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Root URL of the channel with slash at the end.
        /// </summary>
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// Last update of the channel.
        /// </summary>
        [Required]
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Number of latest post in the channel.
        /// </summary>
        [Required]
        public long LastNumber { get; set; }
    }
}
