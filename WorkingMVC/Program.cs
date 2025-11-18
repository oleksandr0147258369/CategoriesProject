using Microsoft.AspNetCore.Identity;
using WorkingMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WorkingMVC.Constants;
using WorkingMVC.Data.Entities;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Account;
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

// додаємо ролі та початкові дані
using (var scoped = app.Services.CreateScope())
{
    var db = scoped.ServiceProvider.GetRequiredService<MyAppDbContext>();
    var roleManager = scoped.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
    var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

    await db.Database.MigrateAsync();

    if (!roleManager.Roles.Any())
    {
        string[] roles = { Roles.Admin, Roles.User };
        foreach (var role in roles)
        {
            var result = await roleManager.CreateAsync(new RoleEntity(role));
            if (result.Succeeded)
            {
                Console.WriteLine($"Role '{role}' created");
            }
        }
    }

    if (!userManager.Users.Any())
    {
        var admin = new UserEntity
        {
            UserName = "admin@admin.com",
            FirstName = "Admin",
            LastName = "Adminenko",
            Image = "ficu3vr1.vsq.webp",
            Email = "admin@admin.com"
        };

        var user = new UserEntity
        {
            UserName = "ivanpetrov@gmail.com",
            FirstName = "Ivan",
            LastName = "Petrov",
            Email = "ivanpetrov@gmail.com",
            Image = "83db6fb0-2b47-4115-a5ae-937c07009969.jpg"
        };

        await userManager.CreateAsync(admin, "admin12345");
        await userManager.AddToRoleAsync(admin, Roles.Admin);

        await userManager.CreateAsync(user, "ivan12345");
        await userManager.AddToRoleAsync(user, Roles.User);
    }

    if (!db.OrderStatuses.Any())
    {
        var statuses = new List<OrderStatusEntity>
        {
            new OrderStatusEntity { Name = "Нове" },
            new OrderStatusEntity { Name = "Очікує оплати" },
            new OrderStatusEntity { Name = "Оплачено" },
            new OrderStatusEntity { Name = "В обробці" },
            new OrderStatusEntity { Name = "Готується до відправки" },
            new OrderStatusEntity { Name = "Відправлено" },
            new OrderStatusEntity { Name = "У дорозі" },
            new OrderStatusEntity { Name = "Доставлено" },
            new OrderStatusEntity { Name = "Завершено" },
            new OrderStatusEntity { Name = "Скасовано (вручну)" },
            new OrderStatusEntity { Name = "Скасовано (автоматично)" },
            new OrderStatusEntity { Name = "Повернення" },
            new OrderStatusEntity { Name = "В обробці повернення" }
        };

        await db.OrderStatuses.AddRangeAsync(statuses);
        await db.SaveChangesAsync();
    }

    if (!db.Categories.Any())
    {
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Name = "Fish" },
            new CategoryEntity { Name = "Vegetables" },
            new CategoryEntity { Name = "Fruits" }
        };

        await db.Categories.AddRangeAsync(categories);
        await db.SaveChangesAsync();
    }

    if (!db.Products.Any())
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity
            {
                CategoryId = db.Categories.First(c => c.Name == "Fish").Id,
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Salmon",
                Description = "Fresh Atlantic salmon",
                Price = 123
            },
            new ProductEntity
            {
                CategoryId = db.Categories.First(c => c.Name == "Vegetables").Id,
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Tomato",
                Description = "Organic tomatoes",
                Price = 25
            },
            new ProductEntity
            {
                CategoryId = db.Categories.First(c => c.Name == "Fruits").Id,
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Apple",
                Description = "Red apples",
                Price = 18
            }
        };

        await db.Products.AddRangeAsync(products);
        await db.SaveChangesAsync();
    }
    if (!db.ProductImages.Any())
    {
        var productImages = new List<ProductImageEntity>
        {
            new ProductImageEntity
            {
                ProductId = db.Products.First(p => p.Name == "Salmon").Id,
                Name = "eyJvYXV0aCI6eyJjbGllbnRfaWQiOiJmcm9udGlmeS1maW5kZXIifSwicGF0aCI6ImloaC1oZWFsdGhjYXJlLWJlcmhhZFwvZmlsZVwvSGhleHdSaUVCYWJ0b1dFRWpUM1EuanBnIn0-ihh-healthcare-berhad-6Zk6UuetaajSDB-43bdLAoamTKKBCqQFMfjY38nWPbk.webp",
                Priority = 1
            },
            new ProductImageEntity
            {
                ProductId = db.Products.First(p => p.Name == "Tomato").Id,
                Name = "Tomato_je.jpg",
                Priority = 1
            },
            new ProductImageEntity
            {
                ProductId = db.Products.First(p => p.Name == "Apple").Id,
                Name = "apple.png",
                Priority = 1
            }
        };

        await db.ProductImages.AddRangeAsync(productImages);
        await db.SaveChangesAsync();
    }
    if (!db.Orders.Any())
    {
        var orders = new List<OrderEntity>
        {
            new OrderEntity
            {
                UserId = (await userManager.FindByEmailAsync("admin@admin.com")).Id,
                OrderStatusId = 1,
                DateCreated = DateTime.UtcNow
            },
            new OrderEntity
            {
                UserId = (await userManager.FindByNameAsync("ivanpetrov@gmail.com")).Id,
                OrderStatusId = 2,
                DateCreated = DateTime.UtcNow
            }
        };

        await db.Orders.AddRangeAsync(orders);
        await db.SaveChangesAsync();
    }
    
    if (!db.OrderItems.Any())
    {
        var orderItems = new List<OrderItemEntity>
        {
            new OrderItemEntity
            {
                OrderId = 4,
                ProductId = db.Products.First(p => p.Name == "Tomato").Id,
                Count = 2,
                PriceBuy = 123
            },
            new OrderItemEntity
            {
                OrderId = 5,
                ProductId = db.Products.First(p => p.Name == "Apple").Id,
                Count = 5,
                PriceBuy = 18
            }
        };

        await db.OrderItems.AddRangeAsync(orderItems);
        await db.SaveChangesAsync();
    }
    if (!db.Carts.Any())
    {
        var carts = new List<CartEntity>
        {
            new CartEntity
            {
                UserId = (await userManager.FindByNameAsync("admin@admin.com")).Id,
                ProductId = db.Products.First(p => p.Name == "Salmon").Id,
                Quantity = 1
            },
            new CartEntity
            {
                UserId = (await userManager.FindByNameAsync("ivanpetrov@gmail.com")).Id,
                ProductId = db.Products.First(p => p.Name == "Apple").Id,
                Quantity = 3
            }
        };

        await db.Carts.AddRangeAsync(carts);
        await db.SaveChangesAsync();
    }


}

// запускаємо вебдодаток
app.Run();
