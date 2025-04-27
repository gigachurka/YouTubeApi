namespace YouTubeApi.Models
{
    public class ChannelRequest
    {
        public string ChannelId { get; set; }
        public string? PageToken { get; set; }
        public int? MaxResults { get; set; }
    }

}
