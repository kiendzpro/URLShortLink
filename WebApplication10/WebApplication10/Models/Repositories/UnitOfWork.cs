using System.Threading.Tasks;

namespace WebApplication10.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITodoRepository _todoRepository;
        private ICategoryRepository _categoryRepository;
        private IShortenedUrlRepository _shortenedUrlRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITodoRepository Todos => _todoRepository ??= new TodoRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public IShortenedUrlRepository ShortenedUrls => _shortenedUrlRepository ??= new ShortenedUrlRepository(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

