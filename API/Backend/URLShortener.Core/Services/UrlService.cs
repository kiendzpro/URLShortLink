using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using URLShortener.Core.Interfaces;
using URLShortener.Core.Models;

namespace URLShortener.Core.Services
{
    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _urlRepository;
        private const int MAX_ATTEMPTS = 5;

        public UrlService(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public async Task<ShortenedUrl> ShortenUrlAsync(string originalUrl, int? expirationDays = null)
        {
            // Validate URL
            if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uri) || 
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("The URL is invalid. It must be an absolute URL with http or https scheme.");
            }

            // Generate a unique code
            string code = await GenerateUniqueCodeAsync();

            // Create the shortened URL
            var shortenedUrl = new ShortenedUrl
            {
                OriginalUrl = originalUrl,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expirationDays.HasValue ? DateTime.UtcNow.AddDays(expirationDays.Value) : null
            };

            // Save to database
            await _urlRepository.CreateAsync(shortenedUrl);
            await _urlRepository.SaveChangesAsync();

            return shortenedUrl;
        }

        public async Task<ShortenedUrl?> GetShortenedUrlAsync(string code)
        {
            var url = await _urlRepository.GetByCodeAsync(code);
            
            if (url == null)
                return null;
                
            if (url.ExpiresAt.HasValue && url.ExpiresAt.Value < DateTime.UtcNow)
                return null;
                
            return url;
        }

        public async Task<ShortenedUrl?> GetShortenedUrlWithStatsAsync(string code)
        {
            var url = await _urlRepository.GetByCodeWithClicksAsync(code);
            
            if (url == null)
                return null;
                
            if (url.ExpiresAt.HasValue && url.ExpiresAt.Value < DateTime.UtcNow)
                return null;
                
            return url;
        }

        public async Task<UrlClick> RecordClickAsync(string code, string? ipAddress, string? userAgent, string? referrer)
        {
            var url = await _urlRepository.GetByCodeAsync(code);
            
            if (url == null)
                throw new KeyNotFoundException($"URL with code {code} not found");
                
            if (url.ExpiresAt.HasValue && url.ExpiresAt.Value < DateTime.UtcNow)
                throw new InvalidOperationException("The URL has expired");
                
            // Increment click count
            await _urlRepository.IncrementClickCountAsync(url.Id);
            
            // Record click details
            var click = new UrlClick
            {
                ShortenedUrlId = url.Id,
                ClickedAt = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Referrer = referrer
            };
            
            await _urlRepository.AddClickAsync(click);
            await _urlRepository.SaveChangesAsync();
            
            return click;
        }

        public async Task<IEnumerable<ShortenedUrl>> GetRecentUrlsAsync(int count = 10)
        {
            return await _urlRepository.GetRecentUrlsAsync(count);
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            string code = null;
            bool codeExists = true;
            int attempts = 0;

            // Try to generate a unique code
            while (codeExists && attempts < MAX_ATTEMPTS)
            {
                code = CodeGenerator.GenerateCode();
                codeExists = await _urlRepository.CodeExistsAsync(code);
                attempts++;
            }

            // If we couldn't generate a unique code with random approach, 
            // use a deterministic approach with current timestamp
            if (codeExists)
            {
                code = CodeGenerator.GenerateCodeFromUrl(DateTime.UtcNow.Ticks.ToString());
                codeExists = await _urlRepository.CodeExistsAsync(code);
                
                if (codeExists)
                    throw new InvalidOperationException("Could not generate a unique code");
            }

            return code;
        }
    }
} 