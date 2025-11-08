using WorkingMVC.Models.Category;

namespace WorkingMVC.Interfaces;

public interface ICategoryService
{
    Task CreateAsync(CategoryCreateModel model);
    Task<List<CategoryItemModel>> GetAllAsync();
    Task<CategoryUpdateModel?> GetByIdAsync(int id);
    Task UpdateAsync(int id, CategoryUpdateModel model);
    Task DeleteAsync(int id);
}