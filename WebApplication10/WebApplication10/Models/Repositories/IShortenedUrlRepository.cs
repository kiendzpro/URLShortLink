using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication10.Models.Repositories
{
    public interface IShortenedUrlRepository
    {
        Task<IEnumerable<ShortenedUrl>> GetAllAsync();
        Task<ShortenedUrl> GetByIdAsync(int id);
        Task<ShortenedUrl> GetByShortCodeAsync(string shortCode);
        Task AddAsync(ShortenedUrl shortenedUrl);
        Task UpdateAsync(ShortenedUrl shortenedUrl);
        Task DeleteAsync(int id);
        Task<bool> ShortCodeExistsAsync(string shortCode);
    }
} 