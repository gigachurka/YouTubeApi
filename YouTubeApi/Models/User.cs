//namespace YouTubeApi.Models
//{
//    using System.ComponentModel.DataAnnotations;

//    public class User
//    {
//        public int Id { get; set; }

//        [Required]
//        public string Login { get; set; }

//        [Required]
//        public string PasswordHash { get; set; }

//        [Required]
//        [EmailAddress]
//        public string Email { get; set; }
//    }

//}

using Microsoft.AspNetCore.Identity;
 
namespace CustomIdentityApp.ViewModels
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
        public string ChannelId { get; set; } = string.Empty;

    }
}
