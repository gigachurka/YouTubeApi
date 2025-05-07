namespace YouTubeApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
