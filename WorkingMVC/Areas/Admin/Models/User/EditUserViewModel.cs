namespace WorkingMVC.Areas.Admin.Models.Users;

public class EditUserViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public IFormFile NewImage { get; set; }
    public string[]? Roles { get; set; }
}