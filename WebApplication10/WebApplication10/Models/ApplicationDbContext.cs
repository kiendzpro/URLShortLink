using Microsoft.EntityFrameworkCore;

namespace WebApplication10.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    }
}
