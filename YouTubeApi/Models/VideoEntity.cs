using System.ComponentModel.DataAnnotations;

namespace YouTubeApi.Models
{
    public class VideoEntity
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string ChannelId { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Link { get; set; } = string.Empty;

        public string? Thumbnail { get; set; }

        public DateTimeOffset? PublishedAt { get; set; }
    }
}

