using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkingMVC.Data.Entities;

[Table("tblOrderStatuses")]
public class OrderStatusEntity : BaseEntity<int>
{
    [StringLength(255)]
    public string Name { get; set; }
}