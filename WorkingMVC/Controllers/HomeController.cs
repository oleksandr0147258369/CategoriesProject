using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Interfaces;

namespace WorkingMVC.Controllers;

public class HomeController(ICategoryService categoryService) : Controller
{
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryService.GetAllAsync();
        return View(categories);
    }
}