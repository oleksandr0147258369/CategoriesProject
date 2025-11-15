using WorkingMVC.Areas.Admin.Models.Users;
using WorkingMVC.Data.Entities.Identity;

namespace WorkingMVC.Interfaces;

public interface IUserService
{
    Task<List<UserItemModel>> GetUsersAsync();
    Task<UserEntity> GetUserByIdAsync(int id);
    Task<EditUserViewModel> GetUserEditByIdAsync(int id);
    Task UpdateUserAsync(EditUserViewModel model);
}