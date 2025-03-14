using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication10.Models;
using WebApplication10.Models.Repositories;

namespace WebApplication10.Controllers
{
    public class TodosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TodosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var todos = await _unitOfWork.Todos.GetAllAsync();
            return View(todos);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Todo todo)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Todos.AddAsync(todo);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var todo = await _unitOfWork.Todos.GetByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.Todos.UpdateAsync(todo);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _unitOfWork.Todos.GetByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _unitOfWork.Todos.GetByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            await _unitOfWork.Todos.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

