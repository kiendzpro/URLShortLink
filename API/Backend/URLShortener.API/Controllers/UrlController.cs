using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using URLShortener.API.Models;
using URLShortener.Core.Interfaces;

namespace URLShortener.API.Controllers
{
    [ApiController]
    [Route("api/urls")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly ILogger<UrlController> _logger;

        public UrlController(IUrlService urlService, ILogger<UrlController> logger)
        {
            _urlService = urlService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UrlResponseDto>> ShortenUrl([FromBody] ShortenUrlRequestDto request)
        {
            try
            {
                var result = await _urlService.ShortenUrlAsync(request.Url, request.ExpirationDays);
                
                // Construct the shortened URL based on the current request
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var shortUrl = $"{baseUrl}/{result.Code}";
                
                return Ok(new UrlResponseDto
                {
                    OriginalUrl = result.OriginalUrl,
                    ShortUrl = shortUrl,
                    Code = result.Code,
                    ExpiresAt = result.ExpiresAt
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid URL submitted");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error shortening URL");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        [HttpGet("{code}/stats")]
        public async Task<ActionResult<UrlStatsDto>> GetUrlStats(string code)
        {
            try
            {
                var result = await _urlService.GetShortenedUrlWithStatsAsync(code);
                
                if (result == null)
                {
                    return NotFound(new { error = "URL not found or expired" });
                }
                
                var stats = new UrlStatsDto
                {
                    OriginalUrl = result.OriginalUrl,
                    Code = result.Code,
                    CreatedAt = result.CreatedAt,
                    ExpiresAt = result.ExpiresAt,
                    ClickCount = result.ClickCount,
                    Clicks = result.Clicks?.Select(click => new ClickDto
                    {
                        ClickedAt = click.ClickedAt,
                        IpAddress = click.IpAddress ?? string.Empty,
                        UserAgent = click.UserAgent ?? string.Empty,
                        Referrer = click.Referrer ?? string.Empty
                    }).ToList() ?? new List<ClickDto>()
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving URL stats");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<UrlResponseDto>>> GetRecentUrls([FromQuery] int count = 10)
        {
            try
            {
                var results = await _urlService.GetRecentUrlsAsync(count);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                var response = results.Select(url => new UrlResponseDto
                {
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = $"{baseUrl}/{url.Code}",
                    Code = url.Code,
                    ExpiresAt = url.ExpiresAt
                });
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent URLs");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<UrlResponseDto>> GetOriginalUrl(string code)
        {
            try
            {
                var result = await _urlService.GetShortenedUrlAsync(code);
                
                if (result == null)
                {
                    return NotFound(new { error = "URL not found or expired" });
                }
                
                // Construct the shortened URL based on the current request
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var shortUrl = $"{baseUrl}/{result.Code}";
                
                return Ok(new UrlResponseDto
                {
                    OriginalUrl = result.OriginalUrl,
                    ShortUrl = shortUrl,
                    Code = result.Code,
                    ExpiresAt = result.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving URL");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }
    }
} 