using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Models.Account;

public class HeaderUserViewComponent : ViewComponent
{
    private readonly UserManager<UserEntity> _userManager;

    public HeaderUserViewComponent(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity.IsAuthenticated)
            return View(new HeaderUserViewModel {FirstName = "", SecondName = "", Image = ""});

        Console.WriteLine(User.Identity.Name);
        Console.WriteLine(User.Identity.IsAuthenticated);
        var user = await _userManager.FindByEmailAsync(User.Identity.Name);

        var model = new HeaderUserViewModel
        {
            FirstName = user.FirstName,
            SecondName = user.LastName,
            Image = user.Image
        };

        return View(model);
    }
}