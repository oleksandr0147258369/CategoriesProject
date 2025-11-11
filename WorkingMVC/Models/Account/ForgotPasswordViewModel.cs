using System.ComponentModel.DataAnnotations;

namespace WorkingMVC.Models.Account;

public class ForgotPasswordViewModel
{
    public string Email { get; set; }
    public string Code { get; set; }
    [Display(Name = "New password")]
    public string NewPassword { get; set; }
}