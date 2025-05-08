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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка модели User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.HasIndex(e => e.Login).IsUnique();
            });
        }

    }
}



