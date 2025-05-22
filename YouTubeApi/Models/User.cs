using Microsoft.AspNetCore.Identity;

namespace YouTubeApi.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
        public string ChannelId { get; set; } = string.Empty;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? RegistrationTokenExpiry { get; set; }
        public string? RegistrationEmail { get; set; }
        public string? RegistrationUsername { get; set; }
        public int? RegistrationYear { get; set; }
        public string? RegistrationChannelId { get; set; }
        public string? RegistrationPassword { get; set; }
        public string? ChannelTitle { get; set; }
    }
}
