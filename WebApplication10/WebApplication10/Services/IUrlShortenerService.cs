using System.Threading.Tasks;
using WebApplication10.Models;

namespace WebApplication10.Services
{
    public interface IUrlShortenerService
    {
        Task<ShortenedUrl> ShortenUrlAsync(string originalUrl);
        Task<ShortenedUrl> GetByShortCodeAsync(string shortCode);
        Task<bool> IncrementAccessCountAsync(string shortCode);
    }
} 