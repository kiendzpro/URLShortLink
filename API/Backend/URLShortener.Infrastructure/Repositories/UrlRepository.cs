using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URLShortener.Core.Interfaces;
using URLShortener.Core.Models;
using URLShortener.Infrastructure.Data;

namespace URLShortener.Infrastructure.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _context;

        public UrlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShortenedUrl?> GetByCodeAsync(string code)
        {
            return await _context.ShortenedUrls
                .FirstOrDefaultAsync(u => u.Code == code);
        }

        public async Task<ShortenedUrl?> GetByCodeWithClicksAsync(string code)
        {
            return await _context.ShortenedUrls
                .Include(u => u.Clicks)
                .FirstOrDefaultAsync(u => u.Code == code);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.ShortenedUrls
                .AnyAsync(u => u.Code == code);
        }

        public async Task CreateAsync(ShortenedUrl shortenedUrl)
        {
            await _context.ShortenedUrls.AddAsync(shortenedUrl);
        }

        public async Task AddClickAsync(UrlClick click)
        {
            await _context.UrlClicks.AddAsync(click);
        }

        public async Task<IEnumerable<ShortenedUrl>> GetRecentUrlsAsync(int count)
        {
            return await _context.ShortenedUrls
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task IncrementClickCountAsync(int id)
        {
            var url = await _context.ShortenedUrls.FindAsync(id);
            if (url != null)
            {
                url.ClickCount++;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
} 