using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication10.Models;

namespace WebApplication10.Models.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetAllAsync();
        Task<Todo> GetByIdAsync(int id);
        Task<Todo> AddAsync(Todo todo);
        Task<Todo> UpdateAsync(Todo todo);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
