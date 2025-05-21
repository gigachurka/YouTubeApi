using System.Collections.Generic;
using YouTubeApi.Models;

namespace YouTubeApi.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<VideoEntity> Videos { get; set; }
    }
} 