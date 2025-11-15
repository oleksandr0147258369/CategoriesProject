using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkingMVC.Data;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Users;

namespace WorkingMVC.Services;

public class UserService(MyAppDbContext context, IMapper mapper, UserManager<UserEntity> userManager, IImageService imageService) : IUserService
{
    public async Task<List<UserItemModel>> GetUsersAsync()
    {
        var users = await context.Users
            .Include(u => u.UserRoles)                // include junction table
            .ThenInclude(ur => ur.Role)          // include the Role entity
            .ToListAsync();
        var mappedUsers = mapper.Map<List<UserItemModel>>(users);
        foreach (var user in mappedUsers)
        {
            Console.WriteLine(user);
            Console.WriteLine(string.Join(" ", user.Roles));
        }
        return mappedUsers;
    }

    public async Task<UserEntity> GetUserByIdAsync(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user;
    }
    public async Task<EditUserViewModel> GetUserEditByIdAsync(int id)
    {
        var users = await context.Users
            .Include(u => u.UserRoles)        
            .ThenInclude(ur => ur.Role)       
            .ToListAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        var model = mapper.Map<EditUserViewModel>(user);
        return model;
    }

    public async Task UpdateUserAsync(EditUserViewModel model)
    {
        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (user == null)
            throw new Exception("User not found");

        // 1. Update basic data
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.UserName = model.Email;

        if (model.NewImage != null)
        {
            user.Image = await imageService.UploadImageAsync(model.NewImage);
        }

        // 2. Update roles
        var currentRoles = user.UserRoles.Select(r => r.Role.Name).ToList();
        var selectedRoles = model.Roles?.ToList() ?? new List<string>();

        // Roles to remove
        var rolesToRemove = currentRoles.Except(selectedRoles).ToList();
        foreach (var roleName in rolesToRemove)
        {
            var role = await context.Roles.FirstAsync(r => r.Name == roleName);
            var userRole = user.UserRoles.First(ur => ur.RoleId == role.Id);
            context.UserRoles.Remove(userRole);
        }

        // Roles to add
        var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
        foreach (var roleName in rolesToAdd)
        {
            var role = await context.Roles.FirstAsync(r => r.Name == roleName);
            user.UserRoles.Add(new UserRoleEntity
            {
                RoleId = role.Id,
                UserId = user.Id
            });
        }

        await context.SaveChangesAsync();
    }

}