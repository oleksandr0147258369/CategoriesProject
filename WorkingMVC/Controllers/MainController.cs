using WorkingMVC.Data;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Data.Entities;
using WorkingMVC.Models.Category;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WorkingMVC.Interfaces;

namespace WorkingMVC.Controllers;

//.NEt 8.0 та 9.0
public class MainController(MyAppDbContext myAppDbContext,
    IConfiguration configuration,
    IMapper mapper,
    IImageService imageService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var list = await myAppDbContext.Categories
            .Where(c => !c.IsDeleted)
            .ProjectTo<CategoryItemModel>(mapper.ConfigurationProvider)
            .ToListAsync();
        return View(list);
    }

    //Для того, щоб побачити сторінку створення категорії
    [HttpGet] //Щоб побачити сторінку і внести інформацію про категорію
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet("update/{id}")]
    public IActionResult Update(int id)
    {
        var entity = myAppDbContext.Categories.Find(id);
        if (entity == null) return NotFound();
    
        var model = mapper.Map<CategoryUpdateModel>(entity);
        return View(model);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CategoryUpdateModel model)
    {

        var entity = myAppDbContext.Categories.Find(id);
        if (entity == null) 
            return NotFound();

        // Always update Name
        entity.Name = model.Name;
        // Only process image if the user uploaded one
        if (model.Image != null)
        {
            var newFileName = await imageService.UploadImageAsync(model.Image);
            entity.Image = newFileName;
        }

        await myAppDbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [HttpGet("delete/{id}")]
    public IActionResult Delete(int id)
    {
        var category = myAppDbContext.Categories.Find(id);

        if (category == null)
            return NotFound();

        // Soft-delete (recommended)
        category.IsDeleted = true;
        myAppDbContext.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost] //Збереження даних
    public async Task<IActionResult> Create(CategoryCreateModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model); // якщо модель не валідна викидаємо дані назад,
            //Щоб користувач знав, що він невірно вніс
        }

        var name = model.Name.Trim().ToLower();
        var entity = myAppDbContext.Categories
            .SingleOrDefault(c => c.Name.ToLower() == name);

        if (entity != null)
        {
            ModelState.AddModelError("", "У нас проблеми Хюстон" +
                $"Така категорія уже є {name}");
            return View(model);
        }

        entity = new CategoryEntity
        {
            Name = model.Name
        };
        var dirImageName = configuration.GetValue<string>("DirImageName");
        if (model.Image != null)
        {
            entity.Image = await imageService.UploadImageAsync(model.Image);
            // //Guid - генерує випадкову величну, яка не можу повторитися
            // var fileName = Guid.NewGuid().ToString()+".jpg";
            // var pathSave = Path.Combine(Directory.GetCurrentDirectory(), 
            //     dirImageName ?? "images", fileName);
            // using var stream = new FileStream(pathSave, FileMode.Create);
            // model.Image.CopyTo(stream); //Зберігаємо фото, яке на приходить у папку.
            // entity.Image = fileName;
        }
        myAppDbContext.Categories.Add(entity);
        myAppDbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
