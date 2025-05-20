using Microsoft.AspNetCore.Identity;

namespace YouTubeApi.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
        public string ChannelId { get; set; } = string.Empty;
    }
}
