using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouTubeApi.Data;
using YouTubeApi.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
namespace YouTubeApi.Controllers
{
    /// <summary>
    /// UC2tsySbe9TNrI-xh2lximHA //A4
    /// UCHgn5EP5TuCMH7TTOVsMfjQ //my kuzhelinovk@gmail.com
    /// UCfuzrGBCVquxNQAAJ1ib7qg //sigma
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class YouTubeController : Controller
    {

        public YouTubeController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;
        //[HttpPost("load")]
        //public async Task<IActionResult> GetChannelsVideos([FromQuery] string channelId, [FromQuery] string? pageToken = null, [FromQuery] int maxResults = 50)
        [HttpPost("load")]
        public async Task<IActionResult> GetChannelsVideos([FromForm] string channelId, [FromForm] string? pageToken = null, [FromForm] int maxResults = 50)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCAc0C3r_XNElzvji9CnFhpzcGm7rhMCkg",
                ApplicationName = "MyYouTubeApp"
            });
            var searchReaquest = youtubeService.Search.List("snippet");
            searchReaquest.ChannelId = channelId;
            Console.WriteLine($"channelId: {channelId}");

            searchReaquest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchReaquest.MaxResults = maxResults;
            if (!string.IsNullOrEmpty(pageToken))
            {
                searchReaquest.PageToken = pageToken;
            }

            searchReaquest.Type = "video";
            var searchResponse = await searchReaquest.ExecuteAsync();

            var videoIds = searchResponse.Items.Select(item => item.Id.VideoId).ToList();
            var videoIdsString = string.Join(",", videoIds);
            //

            var videosRequest = youtubeService.Videos.List("statistics,snippet");
            videosRequest.Id = videoIdsString;
            var videosResponse = await videosRequest.ExecuteAsync();

            var videoStatic = videosResponse.Items.Select(item => new VideoDetails
            {
                Title = item.Snippet.Title,
                Link = $"https://www.youtube.com/watch?v={item.Id}",
                Thumbnail = item.Snippet.Thumbnails.Medium.Url,
                PublishedAt = item.Snippet.PublishedAtDateTimeOffset,
                ViewCount = (int)(item.Statistics.ViewCount ?? 0),
                LikeCount = (int)(item.Statistics.LikeCount ?? 0),
                CommentCount = (int)(item.Statistics.CommentCount ?? 0)
            }).OrderByDescending(video => video.PublishedAt).ToList();

            var response = new YouTubeResponse()
            {
                Video = videoStatic,
                NextPageToken = searchResponse.NextPageToken,
                PrevPageToken = searchResponse.PrevPageToken
            };

            foreach (var item in videosResponse.Items)
            {
                var video = new VideoEntity
                {
                    Title = item.Snippet.Title,
                    Link = $"https://www.youtube.com/watch?v={item.Id}",
                    Thumbnail = item.Snippet.Thumbnails.Medium?.Url,
                    PublishedAt = item.Snippet.PublishedAtDateTimeOffset,
                    ChannelId = channelId,
                    ViewCount = (int)(item.Statistics.ViewCount ?? 0),  // Явное приведение к int
                    LikeCount = (int)(item.Statistics.LikeCount ?? 0),  // Явное приведение к int
                    CommentCount = (int)(item.Statistics.CommentCount ?? 0)  // Явное приведение к int
                };
                if (!_context.Videos.Any(v => v.Link == video.Link))
                {
                    _context.Videos.Add(video);
                }
            }
            await _context.SaveChangesAsync();

            return Ok(response);
        }





        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedVideos()
        {
            var videos = await _context.Videos
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();

            return Ok(videos);
        }

        [HttpGet("analytics/view-trend/{channelId}")]
        public async Task<IActionResult> GetViewTrend(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                return BadRequest("Channel ID is required.");

            var videos = await _context.Videos
                .Where(v => v.ChannelId == channelId && v.PublishedAt != null)
                .OrderBy(v => v.PublishedAt)
                .ToListAsync();

            if (!videos.Any())
                return NotFound("No videos found for this channel.");

            var trendData = videos.Select(v => new
            {
                Date = v.PublishedAt!.Value.UtcDateTime.ToString("yyyy-MM-dd"),
                Views = v.ViewCount,
                Title = v.Title
            });

            return Ok(trendData);
        }

        [HttpGet("analytics/like-trend/{channelId}")]
        public async Task<IActionResult> GetLikeTrend(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                return BadRequest("Channel ID is required.");

            var videos = await _context.Videos
                .Where(v => v.ChannelId == channelId && v.PublishedAt != null)
                .OrderBy(v => v.PublishedAt)
                .Select(v => new {
                    Date = v.PublishedAt!.Value.UtcDateTime.ToString("yyyy-MM-dd"),
                    v.ViewCount,
                    v.LikeCount,
                    v.Title
                })
                .ToListAsync();

            if (!videos.Any())
                return NotFound("No videos found for this channel.");

            var trendData = videos.Select(v => new
            {
                Date = v.Date,
                Likes = v.LikeCount,
                Views = v.ViewCount,
                LikeRatio = v.ViewCount > 0 ? Math.Round((double)v.LikeCount / v.ViewCount * 100, 2) : 0,
                Title = v.Title
            });

            return Ok(trendData);
        }
    }
}