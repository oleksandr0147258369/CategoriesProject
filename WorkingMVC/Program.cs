using Microsoft.AspNetCore.Identity;
using WorkingMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Interfaces;
using WorkingMVC.Repositories;
using WorkingMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси
builder.Services.AddDbContext<MyAppDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<MyAppDbContext>()
    .AddDefaultTokenProviders();
// Компілюємо проєкт
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.MapAreaControllerRoute(
    name: "Admin", 
    areaName: "Admin", 
    pattern: "Admin/{controller=Dashboards}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Categories}/{action=Index}/{id?}").WithStaticAssets();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}")
    .WithStaticAssets();

var dirImageName = builder.Configuration.GetValue<string>("DirImageName") ?? "test";

// Console.WriteLine("Image dir {0}", dirImageName);
var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName);
Directory.CreateDirectory(dirImageName);
// Використовуємо готові файли які не змінюються (.css, .js, .html ...)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dirImageName}"
});

// додаємо ролі для користувача, якщо таких в базі не існує
using (var scoped = app.Services.CreateScope())
{
    var myAppDbContext = scoped.ServiceProvider.GetRequiredService<MyAppDbContext>();
    var roleManager = scoped.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
    myAppDbContext.Database.Migrate();

    if (!roleManager.Roles.Any())
    {
        string[] roles = { "Admin",   "Users" };
        foreach (var role in roles)
        {
            var result = roleManager.CreateAsync(new RoleEntity(role)).Result;
            if (result.Succeeded)
            {
                Console.WriteLine("Role Created");
            }
        }
    }
}
// запускаємо вебдодаток
app.Run();
