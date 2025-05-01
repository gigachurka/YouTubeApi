using System.ComponentModel.DataAnnotations;

namespace YouTubeApi.Models
{
    /// <summary>
    /// тут нужно будет добавить больше инф о видео
    /// </summary>
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

        public long ViewCount { get; set; }
        public long LikeCount { get; set; }
        public long CommentCount { get; set; }
    }
}

