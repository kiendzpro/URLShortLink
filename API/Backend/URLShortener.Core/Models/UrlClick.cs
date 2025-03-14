using System;

namespace URLShortener.Core.Models
{
    public class UrlClick
    {
        public int Id { get; set; }
        public int ShortenedUrlId { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }

        // Navigation property
        public ShortenedUrl ShortenedUrl { get; set; } = null!;
    }
} 