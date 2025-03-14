using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication10.Models.Repositories
{
    public class ShortenedUrlRepository : IShortenedUrlRepository
    {
        private readonly ApplicationDbContext _context;

        public ShortenedUrlRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShortenedUrl>> GetAllAsync()
        {
            return await _context.ShortenedUrls.ToListAsync();
        }

        public async Task<ShortenedUrl> GetByIdAsync(int id)
        {
            return await _context.ShortenedUrls.FindAsync(id);
        }

        public async Task<ShortenedUrl> GetByShortCodeAsync(string shortCode)
        {
            return await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }

        public async Task AddAsync(ShortenedUrl shortenedUrl)
        {
            await _context.ShortenedUrls.AddAsync(shortenedUrl);
        }

        public async Task UpdateAsync(ShortenedUrl shortenedUrl)
        {
            _context.Entry(shortenedUrl).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var shortenedUrl = await _context.ShortenedUrls.FindAsync(id);
            if (shortenedUrl != null)
            {
                _context.ShortenedUrls.Remove(shortenedUrl);
            }
            await Task.CompletedTask;
        }

        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode);
        }
    }
} 