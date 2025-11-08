using WorkingMVC.Data;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Data.Entities;
using WorkingMVC.Models.Category;
using AutoMapper;
using WorkingMVC.Interfaces;

namespace WorkingMVC.Controllers;

public class MainController(
    ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var list = await categoryService.GetAllAsync();
        return View(list);
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var model = await categoryService.GetByIdAsync(id);
        if (model == null)
            return NotFound();
        return View(model);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CategoryUpdateModel model)
    {
        await categoryService.UpdateAsync(id, model);
        return RedirectToAction("Index");
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        await categoryService.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }
}