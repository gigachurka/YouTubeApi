using Microsoft.EntityFrameworkCore;
using YouTubeApi.Models;

namespace YouTubeApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<VideoEntity> Videos { get; set; }
    }
}



