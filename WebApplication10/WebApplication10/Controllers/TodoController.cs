using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication10.Models;
using WebApplication10.Models.Repositories;

namespace WebApplication10.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TodoController(ITodoRepository todoRepository, ICategoryRepository categoryRepository)
        {
            _todoRepository = todoRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: Todo
        public async Task<IActionResult> Index()
        {
            var todos = await _todoRepository.GetAllAsync();
            return View(todos);
        }

        // GET: Todo/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,IsCompleted,CategoryId")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                await _todoRepository.AddAsync(todo);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        // GET: Todo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var todo = await _todoRepository.GetByIdAsync(id.Value);
            if (todo == null)
                return NotFound();

            var categories = await _categoryRepository.GetAllAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        // POST: Todo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,CategoryId")] Todo todo)
        {
            if (id != todo.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _todoRepository.UpdateAsync(todo);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        // GET: Todo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var todo = await _todoRepository.GetByIdAsync(id.Value);
            if (todo == null)
                return NotFound();

            return View(todo);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _todoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 