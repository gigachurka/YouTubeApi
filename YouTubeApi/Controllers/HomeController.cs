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

            // Если ChannelTitle пустой, получаем его из YouTube API
            if (string.IsNullOrEmpty(user.ChannelTitle) && !string.IsNullOrEmpty(user.ChannelId))
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyCAc0C3r_XNElzvji9CnFhpzcGm7rhMCkg",
                    ApplicationName = "MyYouTubeApp"
                });
                var channelRequest = youtubeService.Channels.List("snippet");
                channelRequest.Id = user.ChannelId;
                var channelResponse = await channelRequest.ExecuteAsync();
                string? channelTitle = channelResponse.Items.FirstOrDefault()?.Snippet?.Title;
                if (!string.IsNullOrEmpty(channelTitle))
                {
                    user.ChannelTitle = channelTitle;
                    await _context.SaveChangesAsync();
                }
            }

            var videos = await _context.Videos
                .Where(v => v.ChannelId == user.ChannelId)
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();

            var model = new YouTubeApi.ViewModels.ProfileViewModel
            {
                User = user,
                Videos = videos,
                TotalViews = videos.Sum(v => v.ViewCount),
                TotalLikes = videos.Sum(v => v.LikeCount),
                TotalComments = videos.Sum(v => v.CommentCount),
                ChannelTitle = user.ChannelTitle ?? user.ChannelId ?? "Неизвестный канал"
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
    }
}

        //public async Task<IActionResult> Analytics(string? channelId)
        //{
        //    IQueryable<VideoEntity> query = _context.Videos;

        //    if (!string.IsNullOrEmpty(channelId))
        //    {
        //        query = query.Where(v => v.ChannelId == channelId);
        //    }

        //    var videos = await query
        //        .OrderByDescending(v => v.PublishedAt)
        //        .ToListAsync();

        //    var analyticsData = new
        //    {
        //        TotalViews = videos.Sum(v => v.ViewCount),
        //        TotalLikes = videos.Sum(v => v.LikeCount),
        //        TotalComments = videos.Sum(v => v.CommentCount),
        //        VideoCount = videos.Count,
        //        Videos = videos
        //    };

        //    return View(analyticsData);
        //}