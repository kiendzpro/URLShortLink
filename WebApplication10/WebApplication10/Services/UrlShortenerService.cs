using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication10.Models;
using WebApplication10.Models.Repositories;

namespace WebApplication10.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int ShortCodeLength = 6;

        public UrlShortenerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShortenedUrl> ShortenUrlAsync(string originalUrl)
        {
            // Validate URL
            if (string.IsNullOrWhiteSpace(originalUrl))
            {
                throw new ArgumentException("URL cannot be empty", nameof(originalUrl));
            }

            if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Invalid URL format", nameof(originalUrl));
            }

            // Check if URL already exists in the database
            var existingUrls = await _unitOfWork.ShortenedUrls.GetAllAsync();
            foreach (var url in existingUrls)
            {
                if (url.OriginalUrl == originalUrl)
                {
                    return url;
                }
            }

            // Generate a unique short code
            string shortCode = await GenerateUniqueShortCodeAsync(originalUrl);

            // Create and save the new shortened URL
            var shortenedUrl = new ShortenedUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.Now,
                AccessCount = 0
            };

            await _unitOfWork.ShortenedUrls.AddAsync(shortenedUrl);
            await _unitOfWork.SaveAsync();

            return shortenedUrl;
        }

        public async Task<ShortenedUrl> GetByShortCodeAsync(string shortCode)
        {
            return await _unitOfWork.ShortenedUrls.GetByShortCodeAsync(shortCode);
        }

        public async Task<bool> IncrementAccessCountAsync(string shortCode)
        {
            var shortenedUrl = await _unitOfWork.ShortenedUrls.GetByShortCodeAsync(shortCode);
            if (shortenedUrl == null)
            {
                return false;
            }

            shortenedUrl.AccessCount++;
            await _unitOfWork.ShortenedUrls.UpdateAsync(shortenedUrl);
            await _unitOfWork.SaveAsync();

            return true;
        }

        private async Task<string> GenerateUniqueShortCodeAsync(string originalUrl)
        {
            string shortCode;
            bool exists;

            do
            {
                shortCode = GenerateShortCode(originalUrl);
                exists = await _unitOfWork.ShortenedUrls.ShortCodeExistsAsync(shortCode);
            } while (exists);

            return shortCode;
        }

        private string GenerateShortCode(string originalUrl)
        {
            // Use MD5 to create a hash of the URL
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(originalUrl + DateTime.Now.Ticks.ToString());
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the hash to a base64 string and use the first 6 characters
            string base64 = Convert.ToBase64String(hashBytes)
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");

            return base64.Substring(0, Math.Min(base64.Length, ShortCodeLength));
        }
    }
} 