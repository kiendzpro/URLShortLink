using System;
using System.Collections.Generic;

namespace URLShortener.Core.Models
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public int ClickCount { get; set; } = 0;

        // Navigation property
        public ICollection<UrlClick> Clicks { get; set; } = new List<UrlClick>();
    }
} 