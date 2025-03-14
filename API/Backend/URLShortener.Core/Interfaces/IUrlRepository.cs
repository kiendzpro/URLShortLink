using System.Collections.Generic;
using System.Threading.Tasks;
using URLShortener.Core.Models;

namespace URLShortener.Core.Interfaces
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl?> GetByCodeAsync(string code);
        Task<ShortenedUrl?> GetByCodeWithClicksAsync(string code);
        Task<bool> CodeExistsAsync(string code);
        Task CreateAsync(ShortenedUrl shortenedUrl);
        Task AddClickAsync(UrlClick click);
        Task<IEnumerable<ShortenedUrl>> GetRecentUrlsAsync(int count);
        Task IncrementClickCountAsync(int id);
        Task<int> SaveChangesAsync();
    }
} 