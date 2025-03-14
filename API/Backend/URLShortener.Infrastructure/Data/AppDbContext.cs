using Microsoft.EntityFrameworkCore;
using URLShortener.Core.Models;

namespace URLShortener.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public DbSet<UrlClick> UrlClicks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortenedUrl>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalUrl).IsRequired().HasMaxLength(2048);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasIndex(e => e.Code).IsUnique();
            });

            modelBuilder.Entity<UrlClick>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClickedAt).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Referrer).HasMaxLength(2048);

                entity.HasOne(e => e.ShortenedUrl)
                    .WithMany(s => s.Clicks)
                    .HasForeignKey(e => e.ShortenedUrlId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 