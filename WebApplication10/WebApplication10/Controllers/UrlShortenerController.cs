using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using WebApplication10.Models;
using WebApplication10.Services;

namespace WebApplication10.Controllers
{
    public class UrlShortenerController : Controller
    {
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        private string _baseUrl;

        public UrlShortenerController(
            IUrlShortenerService urlShortenerService,
            ICacheService cacheService,
            IConfiguration configuration)
        {
            _urlShortenerService = urlShortenerService;
            _cacheService = cacheService;
            _configuration = configuration;
            _baseUrl = configuration["UrlShortener:BaseUrl"] ?? "https://localhost:7215"; // Mặc định
        }

        private string GetBaseUrl()
        {
            // Chỉ gọi Request khi xử lý action, không phải trong constructor
            if (Request != null && string.IsNullOrEmpty(_baseUrl))
            {
                _baseUrl = $"{Request.Scheme}://{Request.Host}";
            }
            return _baseUrl;
        }

        public IActionResult Index()
        {
            return View(new UrlShortenRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShortenUrl(UrlShortenRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var shortenedUrl = await _urlShortenerService.ShortenUrlAsync(model.Url);
                ViewBag.ShortenedUrl = $"{GetBaseUrl()}/{shortenedUrl.ShortCode}";
                return View("Index", new UrlShortenRequest());
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Url", ex.Message);
                return View("Index", model);
            }
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            string cacheKey = $"url_{shortCode}";
            
            var originalUrl = await _cacheService.GetOrCreateAsync<string>(
                cacheKey,
                async () =>
                {
                    var shortenedUrl = await _urlShortenerService.GetByShortCodeAsync(shortCode);
                    if (shortenedUrl != null)
                    {
                        await _urlShortenerService.IncrementAccessCountAsync(shortCode);
                        return shortenedUrl.OriginalUrl;
                    }
                    return null;
                },
                TimeSpan.FromMinutes(30));

            if (string.IsNullOrEmpty(originalUrl))
            {
                return NotFound();
            }

            return Redirect(originalUrl);
        }

        [HttpGet("api/shorten")]
        public async Task<IActionResult> ShortenUrlApi([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { error = "URL is required" });
            }

            try
            {
                var shortenedUrl = await _urlShortenerService.ShortenUrlAsync(url);
                return Ok(new
                {
                    originalUrl = shortenedUrl.OriginalUrl,
                    shortUrl = $"{GetBaseUrl()}/{shortenedUrl.ShortCode}",
                    shortCode = shortenedUrl.ShortCode,
                    createdAt = shortenedUrl.CreatedAt
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("api/{shortCode}")]
        public async Task<IActionResult> GetUrlInfoApi(string shortCode)
        {
            var shortenedUrl = await _urlShortenerService.GetByShortCodeAsync(shortCode);
            if (shortenedUrl == null)
            {
                return NotFound(new { error = "URL not found" });
            }

            return Ok(new
            {
                originalUrl = shortenedUrl.OriginalUrl,
                shortUrl = $"{GetBaseUrl()}/{shortenedUrl.ShortCode}",
                shortCode = shortenedUrl.ShortCode,
                createdAt = shortenedUrl.CreatedAt,
                accessCount = shortenedUrl.AccessCount
            });
        }
    }
} 