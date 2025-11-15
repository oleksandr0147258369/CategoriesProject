using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Users;

namespace WorkingMVC.Controllers
{
    public class UsersController(IUserService userService, IMapper mapper) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await userService.GetUsersAsync();
            return View(users);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await userService.GetUserEditByIdAsync(id);
            return View(user);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            await userService.UpdateUserAsync(model);
            return RedirectToAction("Index");
        }
    }
}