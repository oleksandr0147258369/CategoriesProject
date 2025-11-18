using Microsoft.AspNetCore.Identity;

namespace WorkingMVC.Data.Entities.Identity;

public class UserEntity : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Image { get; set; } = null!;
    
    public ICollection<CartEntity>?  CartProducts { get; set; }
    
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
}