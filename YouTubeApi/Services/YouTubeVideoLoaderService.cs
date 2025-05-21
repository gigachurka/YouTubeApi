using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YouTubeApi.Models;
using YouTubeApi.Data;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeApi.Services
{
    public class YouTubeVideoLoaderService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _apiKey = "AIzaSyCAc0C3r_XNElzvji9CnFhpzcGm7rhMCkg"; // TODO: вынести в конфиг

        public YouTubeVideoLoaderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LoadVideosByChannelIdAsync(string channelId, int maxResults = 50)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = _apiKey,
                ApplicationName = "MyYouTubeApp"
            });

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.ChannelId = channelId;
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchRequest.MaxResults = maxResults;

            var searchResponse = await searchRequest.ExecuteAsync();
            var videoIds = searchResponse.Items.Select(item => item.Id.VideoId).ToList();
            var videoIdsString = string.Join(",", videoIds);

            var videosRequest = youtubeService.Videos.List("statistics,snippet");
            videosRequest.Id = videoIdsString;
            var videosResponse = await videosRequest.ExecuteAsync();

            foreach (var item in videosResponse.Items)
            {
                var video = new VideoEntity
                {
                    Title = item.Snippet.Title,
                    Link = $"https://www.youtube.com/watch?v={item.Id}",
                    Thumbnail = item.Snippet.Thumbnails.Medium?.Url,
                    PublishedAt = item.Snippet.PublishedAtDateTimeOffset,
                    ChannelId = channelId,
                    ViewCount = (int)(item.Statistics.ViewCount ?? 0),
                    LikeCount = (int)(item.Statistics.LikeCount ?? 0),
                    CommentCount = (int)(item.Statistics.CommentCount ?? 0)
                };
                if (!_context.Videos.Any(v => v.Link == video.Link))
                {
                    _context.Videos.Add(video);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
} 