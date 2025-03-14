using System;
using System.Threading.Tasks;

namespace WebApplication10.Models.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITodoRepository Todos { get; }
        ICategoryRepository Categories { get; }
        IShortenedUrlRepository ShortenedUrls { get; }
        Task<int> SaveAsync();
    }
}

