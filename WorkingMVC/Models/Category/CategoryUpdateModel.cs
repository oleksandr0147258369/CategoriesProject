using System.ComponentModel.DataAnnotations;

namespace WorkingMVC.Models.Category;

public class CategoryUpdateModel
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Вкажіть назву категорії")]
    public string Name { get; set; } = string.Empty;

    public int Id { get; set; }

    public string CurrentImage { get; set; } = string.Empty;
    //Передача на сервер фото
    [Display(Name = "Фото")]
    [Required(ErrorMessage ="Оберіть фото для категорії")]
    public IFormFile? Image { get; set; }
}