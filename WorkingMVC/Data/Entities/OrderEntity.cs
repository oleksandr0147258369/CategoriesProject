using System.ComponentModel.DataAnnotations.Schema;
using WorkingMVC.Data.Entities.Identity;

namespace WorkingMVC.Data.Entities;

[Table("tblOrders")]
public class OrderEntity : BaseEntity<int>
{
    [ForeignKey(nameof(OrderStatusEntity))]
    public int OrderStatusId { get; set; }
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public OrderStatusEntity? OrderStatus { get; set; }
    public UserEntity? User { get; set; }
}