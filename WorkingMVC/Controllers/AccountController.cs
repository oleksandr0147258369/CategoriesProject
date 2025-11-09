using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Account;

namespace WorkingMVC.Controllers;

public class AccountController(
    UserManager<UserEntity> userManager,
    IMapper mapper,
    IImageService imageService) : Controller
{
    // Створено метод для відображення сторінки реєстрації
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    // Створено обробник реєстрації. тобто отримуємо дані з форми та додаємо користувача в базу даних, якщо такого не існує
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var user = mapper.Map<RegisterViewModel, UserEntity>(model);
        if (model.Image != null)
        {
            user.Image = await imageService.UploadImageAsync(model.Image);
        }

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Main");
        }

        foreach (var item in result.Errors)
        {
            ModelState.AddModelError(string.Empty, item.Description);
        }

        return View(model);
    }
}