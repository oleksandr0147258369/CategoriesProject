using System.ComponentModel.DataAnnotations.Schema;
using WorkingMVC.Data.Entities.Identity;

namespace WorkingMVC.Data.Entities;

[Table("tblCarts")]
public class CartEntity
{
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public int Quantity { get; set; }
    public virtual ProductEntity Product { get; set; }
    public virtual UserEntity User { get; set; }
} 