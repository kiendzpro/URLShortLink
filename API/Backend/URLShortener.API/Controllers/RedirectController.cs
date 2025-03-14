using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using URLShortener.Core.Interfaces;

namespace URLShortener.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly ILogger<RedirectController> _logger;

        public RedirectController(IUrlService urlService, ILogger<RedirectController> logger)
        {
            _urlService = urlService;
            _logger = logger;
        }

        [HttpGet("{code}")]
        public new async Task<IActionResult> Redirect(string code)
        {
            try
            {
                // Get the shortened URL
                var url = await _urlService.GetShortenedUrlAsync(code);
                
                if (url == null)
                {
                    // URL not found or expired
                    return RedirectPermanent("/not-found");
                }
                
                // Record the click
                await _urlService.RecordClickAsync(
                    code,
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString(),
                    Request.Headers["Referer"].ToString()
                );
                
                return RedirectPermanent(url.OriginalUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redirecting to URL");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
} 