using System.Collections.Generic;
using YouTubeApi.Models;

namespace YouTubeApi.ViewModels
{
    public class IndexViewModel
    {
        public List<VideoEntity> Videos { get; set; } = new();
        public User? CurrentUser { get; set; }
    }
} 