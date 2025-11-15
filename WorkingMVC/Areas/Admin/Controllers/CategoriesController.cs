using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Constants;
using WorkingMVC.Models.Category;
using WorkingMVC.Interfaces;

namespace WorkingMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
// Контролер для керування категоріями
public class CategoriesController(
    ICategoryService categoryService) : Controller
{
    // Показати список усіх категорій
    public async Task<IActionResult> Index()
    {
        var list = await categoryService.GetAllAsync();
        return View(list);
    }

    // Показати форму оновлення конкретної категорії
    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var model = await categoryService.GetByIdAsync(id);
        if (model == null)
            return NotFound(); // Повернути 404, якщо категорія не знайдена
        return View(model);
    }

    // Обробити форму для оновлення категорії
    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CategoryUpdateModel model)
    {
        await categoryService.UpdateAsync(id, model); // Оновлення категорії
        return RedirectToAction("Index"); // Перенаправлення на список категорій
    }

    // Видалити категорію за id
    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteAsync(id); // Видалення категорії
        return RedirectToAction("Index");
    }
    
    // Показати форму створення нової категорії
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Обробити форму для створення нової категорії
    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Повернути форму з помилками валідації
        }
        await categoryService.CreateAsync(model); // Створення нової категорії
        return RedirectToAction(nameof(Index)); // Перенаправлення на список категорій
    }
}