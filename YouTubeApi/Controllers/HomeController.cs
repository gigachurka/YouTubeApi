using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTubeApi.Data;
using YouTubeApi.Models;

namespace YouTubeApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
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

            return View(videos);
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

        public async Task<IActionResult> Analytics(string? channelId)
        {
            IQueryable<VideoEntity> query = _context.Videos;

            if (!string.IsNullOrEmpty(channelId))
            {
                query = query.Where(v => v.ChannelId == channelId);
            }

            var videos = await query
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync();

            var analyticsData = new
            {
                TotalViews = videos.Sum(v => v.ViewCount),
                TotalLikes = videos.Sum(v => v.LikeCount),
                TotalComments = videos.Sum(v => v.CommentCount),
                VideoCount = videos.Count,
                Videos = videos
            };

            return View(analyticsData);
        }
    }
}
