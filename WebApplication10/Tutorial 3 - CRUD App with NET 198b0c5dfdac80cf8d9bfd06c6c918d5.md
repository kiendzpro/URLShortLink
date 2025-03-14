# Tutorial 3 - CRUD App with .NET

Certainly! Below is a detailed step-by-step guide to develop a TodoApp using .NET 6 MVC, with LocalDB for the database and Visual Studio 2022 as the IDE. The application will include two models: `Todo` and `Category`, with full CRUD operations for both, and the views will implement data binding.

### Prerequisites

1. **Visual Studio 2022** (with the .NET 6 SDK installed)
2. **LocalDB** (this comes pre-installed with Visual Studio)
3. **Basic knowledge of MVC architecture and C#**

### Step 1: Create a New Project in Visual Studio

1. Open **Visual Studio 2022**.
2. From the main screen, click on **Create a new project**.
3. Select **ASP.NET Core Web Application** and click **Next**.
4. Name your project **TodoApp** and select the location for your project. Click **Create**.
5. In the **Create a new ASP.NET Core Web Application** dialog, choose:
    - **.NET 6.0 (Long-Term Support)**
    - **Web Application (Model-View-Controller)**
6. Ensure that the **Authentication** is set to **None** and click **Create**.

### Step 2: Install Necessary Packages

Ensure that the following packages are installed in your project:

1. **Entity Framework Core SQL Server** for LocalDB connection.
2. **Microsoft.EntityFrameworkCore.Tools** for EF Core commands.

To install these packages, follow these steps:

1. Open **Tools** > **NuGet Package Manager** > **Package Manager Console**.
2. Run the following commands in the **Package Manager Console**:

```bash
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools

```

### Step 3: Set Up Your Models

Create two models: `Todo` and `Category`.

1. Right-click the **Models** folder > **Add** > **Class**. Name the file `Todo.cs`.
    
    **Todo.cs**:
    
    ```csharp
    using System;
    using System.ComponentModel.DataAnnotations;
    
    namespace TodoApp.Models
    {
        public class Todo
        {
            public int Id { get; set; }
    
            [Required]
            [StringLength(100)]
            public string Title { get; set; }
    
            public string Description { get; set; }
    
            public bool IsCompleted { get; set; }
    
            public int CategoryId { get; set; }
            public Category Category { get; set; }
        }
    }
    
    ```
    
2. Similarly, create the `Category` model:
    
    **Category.cs**:
    
    ```csharp
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    namespace TodoApp.Models
    {
        public class Category
        {
            public int Id { get; set; }
    
            [Required]
            [StringLength(50)]
            public string Name { get; set; }
    
            public ICollection<Todo> Todos { get; set; }
        }
    }
    
    ```
    

### Step 4: Create the Database Context

1. Right-click the **Models** folder and create a new class `ApplicationDbContext.cs`.
    
    **ApplicationDbContext.cs**:
    
    ```csharp
    using Microsoft.EntityFrameworkCore;
    
    namespace TodoApp.Models
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }
    
            public DbSet<Todo> Todos { get; set; }
            public DbSet<Category> Categories { get; set; }
        }
    }
    
    ```
    

### Step 5: Configure the Database Connection

1. Open **appsettings.json** and add a connection string for LocalDB:
    
    **appsettings.json**:
    
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
      },
      // Other configurations...
    }
    
    ```
    
2. Open **Startup.cs** or **Program.cs** (depending on your project template), and add the DbContext to the dependency injection container:
    
    **Program.cs**:
    
    ```csharp
    using Microsoft.EntityFrameworkCore;
    using TodoApp.Models;
    
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
    builder.Services.AddControllersWithViews();
    
    var app = builder.Build();
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    
    app.UseRouting();
    
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
    app.Run();
    
    ```
    

### Step 6: Create the Migrations and Update the Database

1. Open **Package Manager Console** and run the following commands to generate and apply the migrations:

```bash
Add-Migration InitialCreate
Update-Database

```

This will create the database schema based on your models.

### Step 7: Create the Controllers

Now, create two controllers: `TodosController` and `CategoriesController`.

1. Right-click the **Controllers** folder > **Add** > **Controller**.
2. Choose **MVC Controller - Empty** and name the controller `TodosController`.

### TodosController (CRUD Operations)

**TodosController.cs**:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class TodosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TodosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Todos
        public async Task<IActionResult> Index()
        {
            var todos = _context.Todos.Include(t => t.Category);
            return View(await todos.ToListAsync());
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Todos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,IsCompleted,CategoryId")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        // GET: Todos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        // POST: Todos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,CategoryId")] Todo todo)
        {
            if (id != todo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todo.CategoryId);
            return View(todo);
        }

        // GET: Todos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var todo = await _context.Todos
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todo == null) return NotFound();

            return View(todo);
        }

        // POST: Todos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}

```

### CategoriesController (CRUD Operations)

**CategoriesController.cs**:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

```

### Step 8: Create Views for `Todos`

### 1. **Index.cshtml** (List of Todos)

In the **Views/Todos** folder, create a view called `Index.cshtml`.

**Views/Todos/Index.cshtml**:

```html
@model IEnumerable<TodoApp.Models.Todo>

@{
    ViewData["Title"] = "Todos";
}

<h1>@ViewData["Title"]</h1>

<p>
    <a href="@Url.Action("Create", "Todos")" class="btn btn-primary">Create New Todo</a>
</p>

<table class="table">
    <thead>
        <tr>
            <th>Title</th>
            <th>Description</th>
            <th>Category</th>
            <th>Completed</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var todo in Model)
        {
            <tr>
                <td>@todo.Title</td>
                <td>@todo.Description</td>
                <td>@todo.Category.Name</td>
                <td>@todo.IsCompleted ? "Yes" : "No"</td>
                <td>
                    <a href="@Url.Action("Edit", "Todos", new { id = todo.Id })" class="btn btn-warning">Edit</a> |
                    <a href="@Url.Action("Delete", "Todos", new { id = todo.Id })" class="btn btn-danger">Delete</a>
                </td>
            </tr>
        }
    </tbody>
</table>

```

### 2. **Create.cshtml** (Form to Create Todo)

Create the `Create.cshtml` view in the **Views/Todos** folder.

**Views/Todos/Create.cshtml**:

```html
@model TodoApp.Models.Todo

@{
    ViewData["Title"] = "Create Todo";
}

<h1>@ViewData["Title"]</h1>

<h4>Todo</h4>
<hr />

<form asp-action="Create" method="post">
    <div class="form-group">
        <label asp-for="Title" class="control-label"></label>
        <input asp-for="Title" class="form-control" />
        <span asp-validation-for="Title" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Description" class="control-label"></label>
        <textarea asp-for="Description" class="form-control"></textarea>
        <span asp-validation-for="Description" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="CategoryId" class="control-label"></label>
        <select asp-for="CategoryId" class="form-control" asp-items="ViewBag.CategoryId"></select>
        <span asp-validation-for="CategoryId" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="IsCompleted" class="control-label"></label>
        <input type="checkbox" asp-for="IsCompleted" class="form-control" />
        <span asp-validation-for="IsCompleted" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Create</button>
    <a href="@Url.Action("Index", "Todos")" class="btn btn-secondary">Cancel</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

### 3. **Edit.cshtml** (Form to Edit Todo)

Create the `Edit.cshtml` view in the **Views/Todos** folder.

**Views/Todos/Edit.cshtml**:

```html
@model TodoApp.Models.Todo

@{
    ViewData["Title"] = "Edit Todo";
}

<h1>@ViewData["Title"]</h1>

<h4>Todo</h4>
<hr />

<form asp-action="Edit" method="post">
    <div class="form-group">
        <label asp-for="Title" class="control-label"></label>
        <input asp-for="Title" class="form-control" />
        <span asp-validation-for="Title" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Description" class="control-label"></label>
        <textarea asp-for="Description" class="form-control"></textarea>
        <span asp-validation-for="Description" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="CategoryId" class="control-label"></label>
        <select asp-for="CategoryId" class="form-control" asp-items="ViewBag.CategoryId"></select>
        <span asp-validation-for="CategoryId" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="IsCompleted" class="control-label"></label>
        <input type="checkbox" asp-for="IsCompleted" class="form-control" />
        <span asp-validation-for="IsCompleted" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Save</button>
    <a href="@Url.Action("Index", "Todos")" class="btn btn-secondary">Cancel</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

### 4. **Delete.cshtml** (Confirm Deletion of Todo)

Create the `Delete.cshtml` view in the **Views/Todos** folder.

**Views/Todos/Delete.cshtml**:

```html
@model TodoApp.Models.Todo

@{
    ViewData["Title"] = "Delete Todo";
}

<h1>@ViewData["Title"]</h1>

<h4>Are you sure you want to delete this Todo?</h4>
<hr />

<div>
    <h3>@Model.Title</h3>
    <p>@Model.Description</p>
    <p>Category: @Model.Category.Name</p>
    <p>Completed: @(Model.IsCompleted ? "Yes" : "No")</p>
</div>

<form asp-action="Delete" method="post">
    <input type="hidden" asp-for="Id" />
    <button type="submit" class="btn btn-danger">Delete</button>
    <a href="@Url.Action("Index", "Todos")" class="btn btn-secondary">Cancel</a>
</form>

```

### Step 9: Create Views for `Categories`

### 1. **Index.cshtml** (List of Categories)

In the **Views/Categories** folder, create a view called `Index.cshtml`.

**Views/Categories/Index.cshtml**:

```html
@model IEnumerable<TodoApp.Models.Category>

@{
    ViewData["Title"] = "Categories";
}

<h1>@ViewData["Title"]</h1>

<p>
    <a href="@Url.Action("Create", "Categories")" class="btn btn-primary">Create New Category</a>
</p>

<table class="table">
    <thead>
        <tr>
            <th>Name</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var category in Model)
        {
            <tr>
                <td>@category.Name</td>
                <td>
                    <a href="@Url.Action("Edit", "Categories", new { id = category.Id })" class="btn btn-warning">Edit</a> |
                    <a href="@Url.Action("Delete", "Categories", new { id = category.Id })" class="btn btn-danger">Delete</a>
                </td>
            </tr>
        }
    </tbody>
</table>

```

### 2. **Create.cshtml** (Form to Create Category)

Create the `Create.cshtml` view in the **Views/Categories** folder.

**Views/Categories/Create.cshtml**:

```html
@model TodoApp.Models.Category

@{
    ViewData["Title"] = "Create Category";
}

<h1>@ViewData["Title"]</h1>

<h4>Category</h4>
<hr />

<form asp-action="Create" method="post">
    <div class="form-group">
        <label asp-for="Name" class="control-label"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Create</button>
    <a href="@Url.Action("Index", "Categories")" class="btn btn-secondary">Cancel</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

### 3. **Edit.cshtml** (Form to Edit Category)

Create the `Edit.cshtml` view in the **Views/Categories** folder.

**Views/Categories/Edit.cshtml**:

```html
@model TodoApp.Models.Category

@{
    ViewData["Title"] = "Edit Category";
}

<h1>@ViewData["Title"]</h1>

<h4>Category</h4>
<hr />

<form asp-action="Edit" method="post">
    <div class="form-group">
        <label asp-for="Name" class="control-label"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Save</button>
    <a href="@Url.Action("Index", "Categories")" class="btn btn-secondary">Cancel</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

### 4. **Delete.cshtml** (Confirm Deletion of Category)

Create the `Delete.cshtml` view in the **Views/Categories** folder.

**Views/Categories/Delete.cshtml**:

```html
@model TodoApp.Models.Category

@{
    ViewData["Title"] = "Delete Category";
}

<h1>@ViewData["Title"]</h1>

<h4>Are you sure you want to delete this Category?</h4>
<hr />

<div>
    <h3>@Model.Name</h3>
</div>

<form asp-action="Delete" method="post">
    <input type="hidden" asp-for="Id" />
    <button type="submit" class="btn btn-danger">Delete</button>
    <a href="@Url.Action("Index", "Categories")" class="btn btn-secondary">Cancel</a>
</form>

```

### Step 10: Run the Application

Once all views are created, press **Ctrl + F5** (or **F5**) to run the application. You should now be able to:

1. View the list of `Todos` and `Categories`.
2. Create, edit, and delete `Todos` and `Categories`.
3. See the data binding in action for both Todo and Category models.

This completes the creation of the `TodoApp` with full CRUD operations and data binding.

---

# **Implementing Repository Pattern in TodoApp**

In this tutorial, we'll:

- Create repository interfaces: `ITodoRepository` and `ICategoryRepository`
- Implement concrete repository classes
- Register repositories with **Dependency Injection (DI)**
- Update controllers to use the repositories

---

## **Step 1: Create the Repository Interfaces**

1. Inside the `TodoApp` project, **right-click the `Models` folder**.
2. **Create a new folder** called `Repositories`.
3. Inside `Repositories`, **add a new interface** called `ITodoRepository.cs` and define it as follows:

### **ITodoRepository.cs**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetAllAsync();
        Task<Todo> GetByIdAsync(int id);
        Task AddAsync(Todo todo);
        Task UpdateAsync(Todo todo);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

```

1. **Add another interface** called `ICategoryRepository.cs`:

### **ICategoryRepository.cs**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

```

---

## **Step 2: Implement the Repository Classes**

Now that we have the interfaces, let's create concrete repository classes.

1. **Inside the `Repositories` folder**, **add a new class** called `TodoRepository.cs`:

### **TodoRepository.cs**

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ApplicationDbContext _context;

        public TodoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await _context.Todos.Include(t => t.Category).ToListAsync();
        }

        public async Task<Todo> GetByIdAsync(int id)
        {
            return await _context.Todos.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(Todo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Todo todo)
        {
            _context.Todos.Update(todo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Todos.AnyAsync(e => e.Id == id);
        }
    }
}

```

1. **Add another class** called `CategoryRepository.cs`:

### **CategoryRepository.cs**

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(e => e.Id == id);
        }
    }
}

```

---

## **Step 3: Register Repositories in Dependency Injection (DI)**

1. Open `Program.cs` and **modify it to register repositories**:

### **Updated Program.cs**

```csharp
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;
using TodoApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories in DI
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

```

---

## **Step 4: Update Controllers to Use Repositories**

Now, let's modify the `TodosController` and `CategoriesController` to use the repositories instead of directly accessing the `ApplicationDbContext`.

### **Updated TodosController.cs**

```csharp
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Repositories;
using System.Threading.Tasks;

namespace TodoApp.Controllers
{
    public class TodosController : Controller
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TodosController(ITodoRepository todoRepository, ICategoryRepository categoryRepository)
        {
            _todoRepository = todoRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _todoRepository.GetAllAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Todo todo)
        {
            if (ModelState.IsValid)
            {
                await _todoRepository.AddAsync(todo);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }
    }
}

```

### **Updated CategoriesController.cs**

```csharp
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Repositories;
using System.Threading.Tasks;

namespace TodoApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _categoryRepository.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}

```

---

# **Refactoring with Unit of Work Pattern**

### **Steps to Implement**

1. Create an `IUnitOfWork` interface.
2. Implement the `UnitOfWork` class.
3. Modify the repositories to work with `UnitOfWork`.
4. Update the controllers to use `UnitOfWork`.

---

## **Step 1: Create `IUnitOfWork` Interface**

Inside the `Repositories` folder, create a new interface called `IUnitOfWork.cs`:

### **IUnitOfWork.cs**

```csharp
csharp
CopyEdit
using System;
using System.Threading.Tasks;

namespace TodoApp.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITodoRepository Todos { get; }
        ICategoryRepository Categories { get; }
        Task<int> SaveAsync();
    }
}

```

Here, `IUnitOfWork` exposes repositories and a `SaveAsync()` method to commit changes.

---

## **Step 2: Implement the `UnitOfWork` Class**

Now, create a class `UnitOfWork.cs` inside the `Repositories` folder:

### **UnitOfWork.cs**

```csharp
csharp
CopyEdit
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITodoRepository _todoRepository;
        private ICategoryRepository _categoryRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITodoRepository Todos => _todoRepository ??= new TodoRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);

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

```

### **What changed?**

✅ `UnitOfWork` manages repositories.

✅ It ensures that `SaveAsync()` commits all changes at once.

✅ The `Dispose()` method cleans up the database context when done.

---

## **Step 3: Update the Repository Implementations**

Now, modify `TodoRepository.cs` and `CategoryRepository.cs` to **remove** `SaveChangesAsync()` calls. The Unit of Work will handle saving changes.

### **Updated `TodoRepository.cs`**

```csharp
csharp
CopyEdit
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ApplicationDbContext _context;

        public TodoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await _context.Todos.Include(t => t.Category).ToListAsync();
        }

        public async Task<Todo> GetByIdAsync(int id)
        {
            return await _context.Todos.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
        }

        public void Add(Todo todo)
        {
            _context.Todos.Add(todo);
        }

        public void Update(Todo todo)
        {
            _context.Todos.Update(todo);
        }

        public void Delete(Todo todo)
        {
            _context.Todos.Remove(todo);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Todos.AnyAsync(e => e.Id == id);
        }
    }
}

```

### **Updated `ICategoryRepository.cs`**

```csharp
csharp
CopyEdit
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);
        Task<bool> ExistsAsync(int id);
    }
}

```

### **Updated `CategoryRepository.cs`**

```csharp
csharp
CopyEdit
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(e => e.Id == id);
        }
    }
}

```

### **What changed?**

❌ Removed `await _context.SaveChangesAsync()` calls

✅ Now, `Add()`, `Update()`, and `Delete()` simply modify the **DbContext**

✅ The **Unit of Work** will handle saving changes

---

## **Step 4: Register `UnitOfWork` in Dependency Injection**

Open `Program.cs` and update **DI registrations**:

### **Updated `Program.cs`**

```csharp
csharp
CopyEdit
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;
using TodoApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

```

---

## **Step 5: Refactor Controllers to Use Unit of Work**

Now, modify `TodosController.cs` and `CategoriesController.cs` to **use `IUnitOfWork` instead of individual repositories**.

### **Updated `TodosController.cs`**

```csharp
csharp
CopyEdit
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Repositories;
using System.Threading.Tasks;

namespace TodoApp.Controllers
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
            return View(await _unitOfWork.Todos.GetAllAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Todo todo)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Todos.Add(todo);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }
    }
}

```

### **Updated `CategoriesController.cs`**

```csharp
csharp
CopyEdit
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Repositories;
using System.Threading.Tasks;

namespace TodoApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.Categories.GetAllAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Add(category);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}

```