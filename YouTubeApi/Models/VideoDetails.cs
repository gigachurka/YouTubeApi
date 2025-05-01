namespace YouTubeApi.Models
{
    public class VideoDetails
    {
        public string? Title { get; set; }
        public string? Link { get; set; }

        public string? Thumbnail { get; set; }
        public DateTimeOffset? PublishedAt  { get; set; }
        public string ChannelId { get; internal set; }                            
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
