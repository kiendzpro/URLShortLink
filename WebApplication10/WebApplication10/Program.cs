using Microsoft.EntityFrameworkCore;
using WebApplication10.Models;
using WebApplication10.Models.Repositories;
using WebApplication10.Services;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and unit of work
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IShortenedUrlRepository, ShortenedUrlRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Add custom route for short URLs
app.MapControllerRoute(
    name: "shortUrl",
    pattern: "{shortCode:length(6)}",
    defaults: new { controller = "UrlShortener", action = "RedirectToOriginal" });

// Add API routes
app.MapControllerRoute(
    name: "apiShorten",
    pattern: "api/shorten",
    defaults: new { controller = "UrlShortener", action = "ShortenUrlApi" });

app.MapControllerRoute(
    name: "apiGetUrl",
    pattern: "api/{shortCode:length(6)}",
    defaults: new { controller = "UrlShortener", action = "GetUrlInfoApi" });

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UrlShortener}/{action=Index}/{id?}");

app.Run();

