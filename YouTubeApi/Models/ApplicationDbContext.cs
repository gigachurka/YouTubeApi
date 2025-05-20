using CustomIdentityApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using YouTubeApi.Models;

namespace YouTubeApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }


        public DbSet<VideoEntity> Videos { get; set; }

    }
}


        //public DbSet<User> Users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Настройка модели User
        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
        //        entity.Property(e => e.PasswordHash).IsRequired();
        //        entity.Property(e => e.Email).HasMaxLength(100);
        //        entity.HasIndex(e => e.Login).IsUnique();
        //    });
        //}
