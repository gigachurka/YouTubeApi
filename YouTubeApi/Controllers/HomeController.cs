using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTubeApi.Data;
using YouTubeApi.Models;
using Microsoft.AspNetCore.Identity;
using YouTubeApi.ViewModels;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Linq;

namespace YouTubeApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? channelId)
        {
            IQueryable<VideoEntity> query = _context.Videos;

            if (!string.IsNullOrEmpty(channelId))
            {
                query = query.Where(v => v.ChannelId == channelId);
            }

            var videos = await query
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();

            User? currentUser = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                currentUser = await _userManager.GetUserAsync(User);
            }

            var model = new IndexViewModel
            {
                Videos = videos,
                CurrentUser = currentUser
            };

            return View(model);
        }
        /// <summary>
        /// Auth + Index
        /// </summary>
        /// <returns></returns>
        /*
        public IActionResult Index()
        {
            return RedirectToAction("Auth");
        }
        */

        public IActionResult Auth()
        {
            return View(); // ищет Views/Home/Auth.cshtml
        }
        public async Task<IActionResult> Search(string? channelId)
        {
            //List <VideoEntity> videos =new List<VideoEntity>();
            IQueryable<VideoEntity> query = _context.Videos;

            if (!string.IsNullOrEmpty(channelId))
            {
                query = query.Where(v => v.ChannelId == channelId);
            }

            var videos = await query
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();
            return View(videos);
            
        }

        public async Task<IActionResult> Profile(string id)
        {
            User user = null;

            if (!string.IsNullOrEmpty(id))
                user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null && User.Identity != null && User.Identity.IsAuthenticated)
                user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Index");

            long subscriberCount = 0;
            {
                var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new Google.Apis.Services.BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyCAc0C3r_XNElzvji9CnFhpzcGm7rhMCkg",
                    ApplicationName = "MyYouTubeApp"
                });
                var channelRequest = youtubeService.Channels.List("snippet,statistics");
                channelRequest.Id = user.ChannelId;
                var channelResponse = await channelRequest.ExecuteAsync();
                var channel = channelResponse.Items.FirstOrDefault();
                string? channelTitle = channel?.Snippet?.Title;
                if (!string.IsNullOrEmpty(channelTitle))
                {
                    user.ChannelTitle = channelTitle;
                }
                if (channel?.Statistics?.SubscriberCount != null)
                {
                    subscriberCount = (long)channel.Statistics.SubscriberCount.Value;
                    user.SubscriberCount = subscriberCount;
                }
                await _context.SaveChangesAsync();
            }

            var videos = await _context.Videos
                .Where(v => v.ChannelId == user.ChannelId)
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();
            var avgViews = videos.Count > 0 ? videos.Average(v => v.ViewCount) : 0;
            var avgLikes = videos.Count > 0 ? videos.Average(v => v.LikeCount) : 0;
            var engagement = videos.Sum(v => v.ViewCount) > 0 ? ((double)(videos.Sum(v => v.LikeCount) + videos.Sum(v => v.CommentCount)) / videos.Sum(v => v.ViewCount)) * 100 : 0;
            var videoCount = videos.Count;
            var createdAt = videos.OrderBy(v => v.PublishedAt).FirstOrDefault()?.PublishedAt;
            string badge = "";
            if (videos.Sum(v => v.ViewCount) > 1000000) badge = "Миллионник";
            else if (engagement > 10) badge = "Вовлечённый канал";
            else if (videoCount > 100) badge = "Ветеран YouTube";
            else badge = "Новичок";

            var model = new YouTubeApi.ViewModels.ProfileViewModel
            {
                User = user,
                Videos = videos,
                TotalViews = videos.Sum(v => v.ViewCount),
                TotalLikes = videos.Sum(v => v.LikeCount),
                TotalComments = videos.Sum(v => v.CommentCount),
                ChannelTitle = user.ChannelTitle ?? user.ChannelId ?? "Неизвестный канал",
                AvgViews = avgViews,
                AvgLikes = avgLikes,
                EngagementRate = engagement,
                VideoCount = videoCount,
                ChannelCreatedAt = createdAt,
                Badge = badge,
                SubscriberCount = user.SubscriberCount
            };
            return View(model);
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index");
            return RedirectToAction("Profile", new { id = user.Id });
        }

        // Новый экшен для подробного просмотра канала и его видео
        public async Task<IActionResult> ChannelDetails(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
                return RedirectToAction("Search");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCAc0C3r_XNElzvji9CnFhpzcGm7rhMCkg",
                ApplicationName = "MyYouTubeApp"
            });
            var channelRequest = youtubeService.Channels.List("snippet,statistics");
            channelRequest.Id = channelId;
            var channelResponse = await channelRequest.ExecuteAsync();
            var channel = channelResponse.Items.FirstOrDefault();

            var videos = await _context.Videos
                .Where(v => v.ChannelId == channelId)
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();

            var model = new YouTubeApi.ViewModels.ProfileViewModel
            {
                ChannelTitle = channel?.Snippet?.Title ?? channelId,
                SubscriberCount = channel?.Statistics?.SubscriberCount != null ? (long)channel.Statistics.SubscriberCount : 0,
                Videos = videos,
                ChannelCreatedAt = channel?.Snippet?.PublishedAt,
                VideoCount = videos.Count
            };
            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
