using System.ComponentModel.DataAnnotations;

namespace WorkingMVC.Models.Account;

public class ResetPasswordViewModel
{
    public string Email { get; set; }
    public string Code { get; set; }
    
    [Display(Name = "New password")]
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
}