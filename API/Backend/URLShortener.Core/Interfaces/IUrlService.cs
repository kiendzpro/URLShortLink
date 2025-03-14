using System.Collections.Generic;
using System.Threading.Tasks;
using URLShortener.Core.Models;

namespace URLShortener.Core.Interfaces
{
    public interface IUrlService
    {
        Task<ShortenedUrl> ShortenUrlAsync(string originalUrl, int? expirationDays = null);
        Task<ShortenedUrl?> GetShortenedUrlAsync(string code);
        Task<ShortenedUrl?> GetShortenedUrlWithStatsAsync(string code);
        Task<UrlClick> RecordClickAsync(string code, string? ipAddress, string? userAgent, string? referrer);
        Task<IEnumerable<ShortenedUrl>> GetRecentUrlsAsync(int count = 10);
    }
} 