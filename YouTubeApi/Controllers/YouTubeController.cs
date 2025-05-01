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

            // Получаем все VideoIds
            var videoIds = searchResponse.Items.Select(item => item.Id.VideoId).ToList();
            var videoIdsString = string.Join(",", videoIds);
            //

            // Делаем отдельный запрос для получения статистики
            var videosRequest = youtubeService.Videos.List("statistics,snippet");
            videosRequest.Id = videoIdsString;
            var videosResponse = await videosRequest.ExecuteAsync();
            //

            //var videoList = searchResponse.Items.Select(item => new VideoDetails
            //{
            //    Title = item.Snippet.Title,
            //    Link = $"https://www.youtube.com/watch?v={item.Id.VideoId}",
            //    Thumbnail = item.Snippet.Thumbnails.Medium.Url,
            //    PublishedAt = item.Snippet.PublishedAtDateTimeOffset
            //}).OrderByDescending(video => video.PublishedAt).ToList();

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

            // Используем videoStatic в ответе
            var response = new YouTubeResponse()
            {
                Video = videoStatic,
                NextPageToken = searchResponse.NextPageToken,
                PrevPageToken = searchResponse.PrevPageToken
            };

            //foreach (var item in searchResponse.Items)
            //{
            //    var video = new VideoEntity
            //    {
            //        Title = item.Snippet.Title,
            //        Link = $"https://www.youtube.com/watch?v={item.Id.VideoId}",
            //        Thumbnail = item.Snippet.Thumbnails.Medium.Url,
            //        PublishedAt = item.Snippet.PublishedAtDateTimeOffset,
            //        ChannelId = channelId,
            //    };

            //    if (!_context.Videos.Any(v => v.Link == video.Link))
            //    {
            //        _context.Videos.Add(video);
            //    }
            //}


            //
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
            //
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

    }
}