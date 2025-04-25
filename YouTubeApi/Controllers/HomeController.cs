using Microsoft.AspNetCore.Mvc;
using YouTubeApi.Data;
using YouTubeApi.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var videos = _context.Videos.ToList(); 
        return View(videos);
    }
}
