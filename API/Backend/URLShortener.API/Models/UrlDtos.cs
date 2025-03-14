using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace URLShortener.API.Models
{
    public class ShortenUrlRequestDto
    {
        [Required]
        [Url]
        public string Url { get; set; }
        
        public int? ExpirationDays { get; set; }
    }

    public class UrlResponseDto
    {
        public string OriginalUrl { get; set; }
        public string ShortUrl { get; set; }
        public string Code { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class UrlStatsDto
    {
        public string OriginalUrl { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int ClickCount { get; set; }
        public List<ClickDto> Clicks { get; set; }
    }

    public class ClickDto
    {
        public DateTime ClickedAt { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
    }
} 