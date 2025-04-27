using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTubeApi.Data;
using YouTubeApi.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var videos = await _context.Videos
            .OrderByDescending(v => v.PublishedAt)
            .ToListAsync();
        return View(videos);
    }

    [HttpPost]
    public async Task<IActionResult> LoadChannelVideos(string channelId)
    {
        try
        {
            var client = new HttpClient();
            var formData = new MultipartFormDataContent(); //FormData, не json
            formData.Add(new StringContent(channelId), "channelId");

            var response = await client.PostAsync("https://localhost:7259/api/youtube/load", formData);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка при запросе: {response.StatusCode}");
            }

            var videos = await _context.Videos
                        .Where(v => v.ChannelId == channelId) // Фильтрую по Id YouTube канала
                        .OrderByDescending(v => v.PublishedAt)
                        .ToListAsync();

            return View("Index", videos);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Index", new List<VideoEntity>());
        }
    }
}
