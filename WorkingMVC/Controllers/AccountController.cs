using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Constants;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Account;
using WorkingMVC.Services;

namespace WorkingMVC.Controllers;

public class AccountController(
    UserManager<UserEntity> userManager,
    IMapper mapper,
    IImageService imageService,
    SignInManager<UserEntity> signInManager) : Controller
{
    private EmailSenderService emailSenderService;
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var res = await signInManager
                .PasswordSignInAsync(user, model.Password, false, false);
            if (res.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return Redirect("/");
            }
        }

        ModelState.AddModelError("", "Дані вазано не вірно!");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Redirect("/");
    }

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
            await userManager.AddToRoleAsync(user, Roles.User);
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Categories");
        }

        foreach (var item in result.Errors)
        {
            ModelState.AddModelError(string.Empty, item.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (model.Email.Length < 6)
            return View(model);
        Console.WriteLine("going");

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return View("ForgotPasswordConfirmation"); // Do not reveal user existence

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var link = Url.Action("ResetPassword", "Account", new { email = model.Email, code = token }, Request.Scheme);

        await emailSenderService.SendEmailAsync(model.Email, "Password Recovery", link);

        return View("ForgotPasswordConfirmation");
    }
    
    [HttpGet]
    public IActionResult ResetPassword(string email, string code)
    {
        return View(new ResetPasswordViewModel { Email = email, Code = code });
    }
    
    

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var user = await userManager.FindByEmailAsync(model.Email);
        var result = await userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
        if (result.Succeeded)
        {
            return RedirectToAction("Login", "Account");
        }
        foreach (var err in result.Errors)
            ModelState.AddModelError("", err.Description);
        
        return View(model);
    }
}
