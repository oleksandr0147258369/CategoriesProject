using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Models.Users;

namespace WorkingMVC.Interfaces;

public interface IUserService
{
    Task<List<UserItemModel>> GetUsersAsync();
    Task<UserEntity> GetUserByIdAsync(int id);
    Task<EditUserViewModel> GetUserEditByIdAsync(int id);
    Task UpdateUserAsync(EditUserViewModel model);
}