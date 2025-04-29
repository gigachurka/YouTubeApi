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
    }
}
